using ArtMarket.Application.DTOs;
using ArtMarket.Application.Interfaces;
using ArtMarket.Domain.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.ML.Transforms.OneHotEncodingEstimator;

namespace ArtMarket.Infrastructure.MachineLearning;

public class MlPricePredictionEngine : IPricePredictionEngine
{
    private readonly MLContext _mlContext;
    private ITransformer? _trainedModel;
    private readonly object _lock = new();

    public MlPricePredictionEngine()
    {
        _mlContext = new MLContext(seed: 42);
    }

    public void Train(IEnumerable<ArtworkRecord> trainingData)
    {
        var inputs = trainingData.Select(x => new MlArtworkInput
        {
            ArtistTier = x.ArtistTier ?? "Emerging",
            Medium = x.Medium ?? "Oil",
            WidthCm = (float)x.WidthCm,
            HeightCm = (float)x.HeightCm,
            CreationYear = (float)x.CreationYear,
            ArtistPastSalesCount = (float)x.ArtistPastSalesCount,
            Label = (float)x.ActualPriceUsd
        }).ToList();

        if (inputs.Count < 5) return;

        var dataView = _mlContext.Data.LoadFromEnumerable(inputs);

        var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding(
                "ArtistTierEncoded", nameof(MlArtworkInput.ArtistTier), OutputKind.Indicator)
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding(
                "MediumEncoded", nameof(MlArtworkInput.Medium), OutputKind.Indicator))
            .Append(_mlContext.Transforms.Concatenate("RawFeatures",
                "ArtistTierEncoded",
                "MediumEncoded",
                nameof(MlArtworkInput.WidthCm),
                nameof(MlArtworkInput.HeightCm),
                nameof(MlArtworkInput.CreationYear),
                nameof(MlArtworkInput.ArtistPastSalesCount)))
            .Append(_mlContext.Transforms.NormalizeMinMax("Features", "RawFeatures"))
            .Append(_mlContext.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 2));

        lock (_lock)
        {
            _trainedModel = pipeline.Fit(dataView);
        }
    }

    public decimal Predict(PredictionRequestDto request)
    {
        lock (_lock)
        {
            if (_trainedModel == null)
                return EstimateFallbackPrice(request);

            var engine = _mlContext.Model.CreatePredictionEngine<MlArtworkInput, MlArtworkOutput>(_trainedModel);

            var input = new MlArtworkInput
            {
                ArtistTier = request.ArtistTier,
                Medium = request.Medium,
                WidthCm = (float)request.WidthCm,
                HeightCm = (float)request.HeightCm,
                CreationYear = (float)request.CreationYear,
                ArtistPastSalesCount = (float)request.ArtistPastSalesCount
            };

            var prediction = engine.Predict(input);

            if (float.IsNaN(prediction.Score) || prediction.Score <= 0)
            {
                return EstimateFallbackPrice(request);
            }

            return Math.Round((decimal)prediction.Score, 2);
        }
    }

    private static decimal EstimateFallbackPrice(PredictionRequestDto req)
    {
        decimal basePrice = req.ArtistTier switch
        {
            "Blue-Chip" => 35000m,
            "Mid-Career" => 8500m,
            _ => 1500m
        };

        decimal sizeFactor = (decimal)(req.WidthCm * req.HeightCm) * 1.8m;
        decimal salesBonus = req.ArtistPastSalesCount * 250m;
        return Math.Round(basePrice + sizeFactor + salesBonus, 2);
    }

    private class MlArtworkInput
    {
        public string ArtistTier { get; set; } = string.Empty;
        public string Medium { get; set; } = string.Empty;
        public float WidthCm { get; set; }
        public float HeightCm { get; set; }
        public float CreationYear { get; set; }
        public float ArtistPastSalesCount { get; set; }
        public float Label { get; set; }
    }

    private class MlArtworkOutput
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
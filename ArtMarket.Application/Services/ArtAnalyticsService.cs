using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using ArtMarket.Application.DTOs;
using ArtMarket.Application.Interfaces;
using ArtMarket.Domain.Entities;
using ArtMarket.Domain.Interfaces;

namespace ArtMarket.Application.Services;

public class ArtAnalyticsService : IArtAnalyticsService
{
    private readonly IArtworkRepository _repository;
    private readonly IPricePredictionEngine _predictionEngine;

    public ArtAnalyticsService(
        IArtworkRepository repository,
        IPricePredictionEngine predictionEngine)
    {
        _repository = repository;
        _predictionEngine = predictionEngine;
    }

    public async Task<MarketAnalyticsDto> GetDashboardAnalyticsAsync()
    {
        var allRecords = await _repository.GetAllAsync();

        if (!allRecords.Any())
            return new MarketAnalyticsDto();

        var yearlyTrends = allRecords
            .GroupBy(x => x.SaleYear)
            .OrderBy(g => g.Key)
            .Select(g => new YearlyTrendDto
            {
                Year = g.Key,
                AvgPrice = Math.Round(g.Average(x => x.ActualPriceUsd), 2),
                SalesCount = g.Count()
            })
            .ToList();

        var mediumStats = allRecords
            .GroupBy(x => x.Medium)
            .Select(g => new MediumStatDto
            {
                Medium = g.Key,
                AvgPrice = Math.Round(g.Average(x => x.ActualPriceUsd), 2),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return new MarketAnalyticsDto
        {
            TotalArtworks = allRecords.Count,
            AveragePriceUsd = Math.Round(allRecords.Average(x => x.ActualPriceUsd), 2),
            MaxPriceUsd = allRecords.Max(x => x.ActualPriceUsd),
            YearlyTrends = yearlyTrends,
            MediumStats = mediumStats
        };
    }

    public async Task<decimal> PredictPriceAsync(PredictionRequestDto request)
    {
        var allRecords = await _repository.GetAllAsync();
        if (allRecords.Count > 5)
        {
            _predictionEngine.Train(allRecords);
        }

        var predicted = _predictionEngine.Predict(request);

        var newRecord = new ArtworkRecord
        {
            Title = "Custom Valuation Lot",
            ArtistName = $"Tier: {request.ArtistTier}",
            ArtistTier = request.ArtistTier,
            Medium = request.Medium,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            CreationYear = request.CreationYear,
            SaleYear = DateTime.UtcNow.Year,
            ArtistPastSalesCount = request.ArtistPastSalesCount,
            ActualPriceUsd = predicted,
            PredictedPriceUsd = predicted
        };

        await _repository.AddAsync(newRecord);
        return predicted;
    }

    public async Task<int> ImportFromCsvAsync(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        var records = new List<ArtworkRecord>();

        var headerLine = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(headerLine))
            return 0;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = line.Split(',');
            if (cols.Length < 10) continue;

            if (float.TryParse(cols[4], NumberStyles.Any, CultureInfo.InvariantCulture, out float width) &&
                float.TryParse(cols[5], NumberStyles.Any, CultureInfo.InvariantCulture, out float height) &&
                int.TryParse(cols[6], out int creationYear) &&
                int.TryParse(cols[7], out int saleYear) &&
                int.TryParse(cols[8], out int pastSales) &&
                decimal.TryParse(cols[9], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                records.Add(new ArtworkRecord
                {
                    Title = cols[0].Trim('"'),
                    ArtistName = cols[1].Trim('"'),
                    ArtistTier = cols[2].Trim('"'),
                    Medium = cols[3].Trim('"'),
                    WidthCm = width,
                    HeightCm = height,
                    CreationYear = creationYear,
                    SaleYear = saleYear,
                    ArtistPastSalesCount = pastSales,
                    ActualPriceUsd = price
                });
            }
        }

        if (records.Any())
        {
            await _repository.AddRangeAsync(records);
            _predictionEngine.Train(records);
        }

        return records.Count;
    }

    public async Task<List<ArtworkRecord>> GetRecentArtworksAsync(int count = 50)
    {
        return await _repository.GetRecentAsync(count);
    }
}

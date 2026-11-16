using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArtMarket.Application.DTOs;
using ArtMarket.Domain.Entities;

namespace ArtMarket.Application.Interfaces;

public interface IPricePredictionEngine
{
    void Train(IEnumerable<ArtworkRecord> trainingData);
    decimal Predict(PredictionRequestDto request);
}

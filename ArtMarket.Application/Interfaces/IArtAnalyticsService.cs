using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArtMarket.Application.DTOs;
using ArtMarket.Domain.Entities;

namespace ArtMarket.Application.Interfaces;

public interface IArtAnalyticsService
{
    Task<MarketAnalyticsDto> GetDashboardAnalyticsAsync();
    Task<decimal> PredictPriceAsync(PredictionRequestDto request);
    Task<int> ImportFromCsvAsync(Stream csvStream);
    Task<List<ArtworkRecord>> GetRecentArtworksAsync(int count = 50);
}

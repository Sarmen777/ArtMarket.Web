using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtMarket.Application.DTOs;

public class PredictionRequestDto
{
    public string ArtistTier { get; set; } = "Mid-Career";
    public string Medium { get; set; } = "Oil";
    public float WidthCm { get; set; } = 60;
    public float HeightCm { get; set; } = 80;
    public int CreationYear { get; set; } = 2015;
    public int ArtistPastSalesCount { get; set; } = 10;
}

public class MarketAnalyticsDto
{
    public int TotalArtworks { get; set; }
    public decimal AveragePriceUsd { get; set; }
    public decimal MaxPriceUsd { get; set; }
    public List<YearlyTrendDto> YearlyTrends { get; set; } = new();
    public List<MediumStatDto> MediumStats { get; set; } = new();
}

public class YearlyTrendDto
{
    public int Year { get; set; }
    public decimal AvgPrice { get; set; }
    public int SalesCount { get; set; }
}

public class MediumStatDto
{
    public string Medium { get; set; } = string.Empty;
    public decimal AvgPrice { get; set; }
    public int Count { get; set; }
}
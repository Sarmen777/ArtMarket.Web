using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtMarket.Domain.Entities;

public class ArtworkRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;

   
    public string ArtistTier { get; set; } = string.Empty;

    public string Medium { get; set; } = string.Empty;

    public float WidthCm { get; set; }
    public float HeightCm { get; set; }

    public int CreationYear { get; set; }
    public int SaleYear { get; set; }

    public int ArtistPastSalesCount { get; set; }

    public decimal ActualPriceUsd { get; set; }
    public decimal? PredictedPriceUsd { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

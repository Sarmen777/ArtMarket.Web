using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArtMarket.Domain.Entities;

namespace ArtMarket.Domain.Interfaces;

public interface IArtworkRepository
{
    Task<List<ArtworkRecord>> GetAllAsync();

    Task<List<ArtworkRecord>> GetRecentAsync(int count);

    Task AddAsync(ArtworkRecord artwork);

    Task AddRangeAsync(IEnumerable<ArtworkRecord> artworks);

    Task<int> GetCountAsync();
}
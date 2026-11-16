using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArtMarket.Domain.Entities;
using ArtMarket.Domain.Interfaces;
using ArtMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtMarket.Infrastructure.Repositories;

public class ArtworkRepository : IArtworkRepository
{
    private readonly ArtMarketDbContext _context;

    public ArtworkRepository(ArtMarketDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArtworkRecord>> GetAllAsync()
    {
        return await _context.Artworks
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<ArtworkRecord>> GetRecentAsync(int count)
    {
        return await _context.Artworks
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(ArtworkRecord artwork)
    {
        await _context.Artworks.AddAsync(artwork);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<ArtworkRecord> artworks)
    {
        await _context.Artworks.AddRangeAsync(artworks);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Artworks.CountAsync();
    }
}

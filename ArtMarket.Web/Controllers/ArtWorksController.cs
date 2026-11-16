using ArtMarket.Application.DTOs;
using ArtMarket.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArtMarket.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtworksController : ControllerBase
{
    private readonly IArtAnalyticsService _analyticsService;

    public ArtworksController(IArtAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    
    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var result = await _analyticsService.GetDashboardAnalyticsAsync();
        return Ok(result);
    }

  
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50)
    {
        var result = await _analyticsService.GetRecentArtworksAsync(count);
        return Ok(result);
    }

    
    [HttpPost("predict")]
    public async Task<IActionResult> PredictPrice([FromBody] PredictionRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var predictedPrice = await _analyticsService.PredictPriceAsync(request);
        return Ok(new { EstimatedPriceUsd = predictedPrice });
    }

  
    [HttpPost("import-csv")]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty or not provided.");

        using var stream = file.OpenReadStream();
        int importedCount = await _analyticsService.ImportFromCsvAsync(stream);

        return Ok(new { Message = $"Successfully imported {importedCount} records." });
    }
}
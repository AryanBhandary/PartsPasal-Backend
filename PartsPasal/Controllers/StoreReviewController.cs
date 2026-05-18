using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPasal.Application.DTOs.Customer;
using PartsPasal.Application.Interfaces;
using PartsPasal.Domain.Enums;

namespace PartsPasal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreReviewController : ControllerBase
{
    private readonly IStoreReviewService _storeReviewService;

    public StoreReviewController(IStoreReviewService storeReviewService)
    {
        _storeReviewService = storeReviewService;
    }

    [Authorize(Roles = nameof(UserRole.Customer))]
    [HttpPost]
    public async Task<IActionResult> SubmitReview([FromBody] CreateStoreReviewDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "Request body is required." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdText) || !int.TryParse(userIdText, out var userId))
        {
            return Unauthorized("User ID not found or invalid in token.");
        }

        var reviewId = await _storeReviewService.SubmitReviewAsync(userId, dto);

        return Ok(new
        {
            message = "Store review submitted successfully.",
            reviewId
        });
    }

    [Authorize(Roles = "Staff,Admin")]
    [HttpGet("average")]
    public async Task<IActionResult> GetAverageRating()
    {
        var average = await _storeReviewService.GetAverageRatingAsync();
        return Ok(new { averageRating = average });
    }
}

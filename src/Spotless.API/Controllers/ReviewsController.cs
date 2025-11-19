using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Review;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Spotless.Infrastructure.Identity.ApplicationUser> _userManager;

        public ReviewsController(IMediator mediator, Microsoft.AspNetCore.Identity.UserManager<Spotless.Infrastructure.Identity.ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var identityUserId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var identityUser = await _userManager.FindByIdAsync(userIdString);
            if (identityUser == null || !identityUser.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var command = new Spotless.Application.Features.Reviews.Commands.CreateReview.CreateReviewCommand(identityUser.CustomerId.Value, dto);

            var review = await _mediator.Send(command);

            return CreatedAtAction(nameof(CreateReview), new { id = review.Id }, review.Id);
        }

        [AllowAnonymous]
        [HttpGet("driver/{driverId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ReviewDto>))]
        public async Task<IActionResult> GetReviewsByDriver(Guid driverId)
        {
            var query = new Spotless.Application.Features.Reviews.Queries.GetReviewByDriver.GetReviewsByDriverQuery(driverId);

            var reviews = await _mediator.Send(query);

            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<ReviewDto>))]
        public async Task<IActionResult> GetReviewsByCustomer()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var identityUserId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var identityUser = await _userManager.FindByIdAsync(userIdString);
            if (identityUser == null || !identityUser.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var query = new Spotless.Application.Features.Reviews.Queries.GetReviewsByCustomer.GetReviewsByCustomerQuery(identityUser.CustomerId.Value);

            var reviews = await _mediator.Send(query);

            return Ok(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Responses.PagedResponse<ReviewDto>))]
        public async Task<IActionResult> ListAllReviews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
        {
            var query = new Spotless.Application.Features.Reviews.Queries.ListAllReviews.GetAllReviewsQuery(null)
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
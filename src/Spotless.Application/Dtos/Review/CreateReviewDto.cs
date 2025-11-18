namespace Spotless.Application.Dtos.Review
{

    public record CreateReviewDto(
        Guid OrderId,
        int Rating,
        string? Comment
    );
}
namespace Spotless.Application.Dtos.Service
{
    public record ServiceDto(
        Guid Id,
        Guid CategoryId,
        string CategoryName,
        string Name,
        string Description,
        string CategoryPrice);
}

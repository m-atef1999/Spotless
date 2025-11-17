using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Dtos.Driver
{

    public record RegisterDriverRequest
    {

        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; init; } = string.Empty;


        [Required]
        public string Name { get; init; } = string.Empty;


        [Required]
        [Phone]
        public string Phone { get; init; } = string.Empty;

        [Required]
        public string VehicleInfo { get; init; } = string.Empty;
    }
}
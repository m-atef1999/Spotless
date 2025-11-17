using MediatR;
using Spotless.Application.Dtos.Authentication;
using Spotless.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Customers
{
    public record RegisterCustomerCommand(

        [EmailAddress] string Email,
        [Required] string Password,


        [Required] string Name,
        string? Phone,
        CustomerType Type,


        [Required] string Street,
        [Required] string City,
        [Required] string Country,
        string? ZipCode

    ) : IRequest<AuthResult>;
}
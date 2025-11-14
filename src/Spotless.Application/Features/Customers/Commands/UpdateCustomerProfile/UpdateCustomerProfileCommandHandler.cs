using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;


        var customer = await _unitOfWork.Customers.GetByIdAsync(request.UserId);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer profile not found for user ID: {request.UserId}");
        }

        var newAddress = new Address(
            dto.Street,
            dto.City,
            dto.Country,
            dto.ZipCode
        );


        customer.UpdateProfile(
            dto.Name,
            dto.Phone,
            newAddress
        );

        await _unitOfWork.Customers.UpdateAsync(customer);

        await _unitOfWork.CommitAsync();

        return Unit.Value;
    }
}
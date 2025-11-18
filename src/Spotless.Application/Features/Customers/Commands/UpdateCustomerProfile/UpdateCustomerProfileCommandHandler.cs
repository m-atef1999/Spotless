using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile
{
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


            string newName = dto.Name ?? customer.Name;
            string? newPhone = dto.Phone ?? customer.Phone;


            Address originalAddress = customer.Address;


            if (dto.Street != null || dto.City != null || dto.Country != null || dto.ZipCode != null)
            {
                var newAddress = new Address(

                    street: dto.Street ?? originalAddress.Street,
                    city: dto.City ?? originalAddress.City,
                    country: dto.Country ?? originalAddress.Country,
                    zipCode: dto.ZipCode ?? originalAddress.ZipCode
                );


                originalAddress = newAddress;
            }


            customer.UpdateProfile(
                newName,
                newPhone,
                originalAddress
            );


            await _unitOfWork.Customers.UpdateAsync(customer);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
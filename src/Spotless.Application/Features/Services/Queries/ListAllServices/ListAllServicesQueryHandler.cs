using MediatR;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Services.Queries.ListAllServices
{

    public class ListServicesQueryHandler(IUnitOfWork unitOfWork, IServiceMapper serviceMapper, CachedServiceService cachedServiceService) : IRequestHandler<ListServicesQuery, PagedResponse<ServiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IServiceMapper _serviceMapper = serviceMapper;
        private readonly CachedServiceService _cachedServiceService = cachedServiceService;

        public async Task<PagedResponse<ServiceDto>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.NameSearchTerm))
            {
                var cachedServices = await _cachedServiceService.GetAllServicesAsync();
                var pagedCached = cachedServices.Skip(request.Skip).Take(request.PageSize);
                return new PagedResponse<ServiceDto>([.. pagedCached], cachedServices.Count(), request.PageNumber, request.PageSize);
            }

            var filterExpression = BuildFilterExpression(request);
            var totalRecords = await _unitOfWork.Services.CountAsync(filterExpression);
            var services = await _unitOfWork.Services.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,
                include: null,
                orderBy: q => q.OrderBy(s => s.Name)
            );

            var serviceDtos = _serviceMapper.MapToDto(services).ToList();
            return new PagedResponse<ServiceDto>(serviceDtos, totalRecords, request.PageNumber, request.PageSize);
        }


        private Expression<Func<Service, bool>> BuildFilterExpression(ListServicesQuery request)
        {
            var raw = request.NameSearchTerm?.Trim();
            if (string.IsNullOrEmpty(raw))
                return service => true;

            // split by whitespace to support multi-word search (handles spaces and tokens in Arabic too)
            var tokens = raw.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            // Build expression: service.Name != null && tokens[0] in Name && tokens[1] in Name && ...
            ParameterExpression param = Expression.Parameter(typeof(Service), "service");
            Expression? body = null;

            var nameProp = Expression.Property(param, nameof(Service.Name));
            var notNull = Expression.NotEqual(nameProp, Expression.Constant(null, typeof(string)));

            foreach (var token in tokens)
            {
                var tokenConst = Expression.Constant(token, typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
                var containsCall = Expression.Call(nameProp, containsMethod!, tokenConst);

                var clause = Expression.AndAlso(notNull, containsCall);
                body = body == null ? clause : Expression.AndAlso(body, clause);
            }

            var lambda = Expression.Lambda<Func<Service, bool>>(body ?? Expression.Constant(true), param);
            return lambda;
        }
    }
}
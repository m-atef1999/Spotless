using MediatR;
using Microsoft.EntityFrameworkCore;
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
                include: q => q.Include(s => s.Category),
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

            var tokens = raw.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            ParameterExpression param = Expression.Parameter(typeof(Service), "service");
            Expression? body = null;

            var nameProp = Expression.Property(param, nameof(Service.Name));
            var categoryProp = Expression.Property(param, nameof(Service.Category));
            var categoryNameProp = Expression.Property(categoryProp, nameof(Category.Name));

            var notNullName = Expression.NotEqual(nameProp, Expression.Constant(null, typeof(string)));
            var notNullCategory = Expression.NotEqual(categoryProp, Expression.Constant(null, typeof(Category)));
            var notNullCategoryName = Expression.NotEqual(categoryNameProp, Expression.Constant(null, typeof(string)));

            foreach (var token in tokens)
            {
                var tokenConst = Expression.Constant(token, typeof(string));
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

                // Name.Contains(token)
                var nameContains = Expression.Call(nameProp, containsMethod!, tokenConst);
                var nameClause = Expression.AndAlso(notNullName, nameContains);

                // Category.Name.Contains(token)
                var categoryContains = Expression.Call(categoryNameProp, containsMethod!, tokenConst);
                var categoryClause = Expression.AndAlso(notNullCategory, Expression.AndAlso(notNullCategoryName, categoryContains));

                // (Name.Contains(token) OR Category.Name.Contains(token))
                var tokenClause = Expression.OrElse(nameClause, categoryClause);

                body = body == null ? tokenClause : Expression.AndAlso(body, tokenClause);
            }

            var lambda = Expression.Lambda<Func<Service, bool>>(body ?? Expression.Constant(true), param);
            return lambda;
        }
    }
}

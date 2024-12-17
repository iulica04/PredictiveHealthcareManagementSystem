using Application.Utils;
using Domain.Common;
using Domain.Enums;
using MediatR;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class GetFilteredQuery<T, TDto> : IRequest<Result<PagedResult<TDto>>>
    {
        public UserType Type { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public Expression<Func<T, bool>>? Filter { get; set; }
    }
}

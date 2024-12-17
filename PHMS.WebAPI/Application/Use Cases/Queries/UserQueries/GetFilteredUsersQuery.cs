using Domain.Enums;

namespace Application.Use_Cases.Queries.UserQueries
{
    public class GetFilteredUsersQuery
    {
        public UserType Type { get; set; }
        public required int Page { get; set; }
        public required int PageSize { get; set; }
        public string? Rank { get; set; }
        public string? Specialization { get; set; }
        public string? Hospital { get; set; }
    }
}

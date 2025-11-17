namespace Spotless.Application.Dtos.Responses
{

    public record PagedResponse<T>
    {

        public IReadOnlyList<T> Items { get; init; } = new List<T>();


        public int PageNumber { get; init; }

        public int PageSize { get; init; }

        public long TotalRecords { get; init; }


        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);


        public bool HasNextPage => PageNumber < TotalPages;

        public bool HasPreviousPage => PageNumber > 1;

        public PagedResponse() { }

        public PagedResponse(IEnumerable<T> items, long totalRecords, int pageNumber, int pageSize)
        {
            Items = items?.ToList() ?? new List<T>();
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
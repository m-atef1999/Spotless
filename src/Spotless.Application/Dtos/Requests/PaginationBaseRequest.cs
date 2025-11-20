namespace Spotless.Application.Dtos.Requests
{
    public record PaginationBaseRequest
    {
        // Note: These are fallback defaults if IOptions is not available
        // In practice, these should come from IOptions<PaginationSettings>
        private static readonly int FallbackMaxPageSize = 100;
        private static readonly int FallbackDefaultPageNumber = 1;
        private static readonly int FallbackDefaultPageSize = 25;

        private int _pageNumber = FallbackDefaultPageNumber;
        private int _pageSize = FallbackDefaultPageSize;

        // These properties can be set from IOptions<PaginationSettings> via a helper method
        public int MaxPageSize { get; init; } = FallbackMaxPageSize;
        public int DefaultPageNumber { get; init; } = FallbackDefaultPageNumber;
        public int DefaultPageSize { get; init; } = FallbackDefaultPageSize;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value > 0) ? value : DefaultPageNumber;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value > MaxPageSize)
                    _pageSize = MaxPageSize;
                else if (value < 1)
                    _pageSize = DefaultPageSize;
                else
                    _pageSize = value;
            }
        }

        public int Skip => (PageNumber - 1) * PageSize;
    }
}

namespace Spotless.Application.Dtos.Requests
{
    public record PaginationBaseRequest
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 25;

        private int _pageNumber = DefaultPageNumber;
        private int _pageSize = DefaultPageSize;

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
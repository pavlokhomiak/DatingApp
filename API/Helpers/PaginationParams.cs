namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        // default page number
        public int PageNumber { get; set; } = 1;
        // default page size
        private int _pageSize = 10;
        // page size
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
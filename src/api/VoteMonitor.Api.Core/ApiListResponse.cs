using System.Collections.Generic;

namespace VoteMonitor.Api.Core
{
    public static class Constants
    {
        public const int DEFAULT_PAGE_SIZE = 20;
        public const int DEFAULT_PAGE = 1;
    }
    public class PagingModel
    {
        protected int _page;
        protected int _pageSize;

        public int Page
        {
            get { return _page; }
            set { _page = value < 1 ? Constants.DEFAULT_PAGE : value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value < 1 ? Constants.DEFAULT_PAGE_SIZE : value; }
        }
    }

    public class PagingResponseModel : PagingModel
    {
        protected int _totalItems;

        public int TotalItems
        {
            get { return _totalItems; }
            set { _totalItems = value; }
        }

        public int TotalPages
        {
            get { return 1 + (_totalItems - 1) / _pageSize; }
        }
    }
    public class ApiResponse<T>
    where T : class
    {
        public T Data { get; set; }
    }

    public class ApiListResponse<T> : PagingResponseModel
    {
        public List<T> Data { get; set; }
    }
}

using System.Collections.Generic;

namespace VoteMonitor.Api.Core
{
    public class ApiListResponse<T> : PagingResponseModel
    {
        public List<T> Data { get; set; }
    }
}

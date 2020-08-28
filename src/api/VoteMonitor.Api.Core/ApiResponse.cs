namespace VoteMonitor.Api.Core
{
    public class ApiResponse<T>
        where T : class
    {
        public T Data { get; set; }
    }
}
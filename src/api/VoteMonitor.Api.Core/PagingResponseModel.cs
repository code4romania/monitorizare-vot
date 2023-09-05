namespace VoteMonitor.Api.Core;

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
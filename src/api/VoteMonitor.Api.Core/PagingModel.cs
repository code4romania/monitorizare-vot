using Microsoft.AspNetCore.Mvc;

namespace VoteMonitor.Api.Core;

public class PagingModel
{
    protected int _page;
    protected int _pageSize;

    [FromQuery]
    public int Page
    {
        get => _page < 1 ? PagingDefaultsConstants.DEFAULT_PAGE : _page;
        set => _page = value < 1 ? PagingDefaultsConstants.DEFAULT_PAGE : value;
    }


    [FromQuery]
    public int PageSize
    {
        get => _pageSize < 1 ? PagingDefaultsConstants.DEFAULT_PAGE_SIZE : _pageSize;
        set => _pageSize = value < 1 ? PagingDefaultsConstants.DEFAULT_PAGE_SIZE : value;
    }
}

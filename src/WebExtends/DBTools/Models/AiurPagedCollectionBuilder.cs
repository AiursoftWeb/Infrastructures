using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Handler.Interfaces;
using Aiursoft.Handler.Models;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.DBTools.Models;

public static class AiurPagedCollectionBuilder
{
    public static async Task<AiurPagedCollection<T>> BuildAsync<T>(
        IOrderedQueryable<T> query,
        IPageable pager,
        ErrorType code,
        string message)
    {
        var items = await query.Page(pager).ToListAsync();
        return new AiurPagedCollection<T>(items)
        {
            TotalCount = await query.CountAsync(),
            CurrentPage = pager.PageNumber,
            CurrentPageSize = pager.PageSize,
            Code = code,
            Message = message
        };
    }
}
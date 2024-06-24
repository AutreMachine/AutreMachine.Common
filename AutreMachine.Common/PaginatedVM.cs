using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public interface IPaginatedVM<T>
    {
        int TotalPages { get; set; }
    }

    /// <summary>
    /// The viewModel allows to pass PaginatedList through API and REST endpoints.
    /// Otherwise, the fields TotalPages in PaginatedList can not be sent in JSON.
    /// That's why it is better to user PaginatedViewModel in Blazor pages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedVM<T> : IPaginatedVM<T>
    {
        public int PageIndex { get; set; }
        
        public int TotalPages { get; set; }

        public PaginatedVM()
        { }

         public List<T>? List {get; set;}

        

        public PaginatedVM(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (List == null )
                List = new List<T>();

            List.AddRange(items);
        }
        public PaginatedVM(List<T> items, int totalPages, int pageIndex)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;

            if (List == null )
                List = new List<T>();

            List.AddRange(items);
        }
  
        

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedVM<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedVM<T>(items, count, pageIndex, pageSize);
        }

        public static PaginatedVM<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedVM<T>(items, count, pageIndex, pageSize);
        }

         
    }

}

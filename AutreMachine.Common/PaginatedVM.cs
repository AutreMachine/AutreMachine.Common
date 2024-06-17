﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public interface IPaginatedVM<T>
    {
        int? TotalPages { get; set; }
    }

    /// <summary>
    /// The viewModel allows to pass PaginatedList through API and REST endpoints.
    /// Otherwise, the fields TotalPages in PaginatedList can not be sent in JSON.
    /// That's why it is better to user PaginatedViewModel in Blazor pages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedVM<T> : IPaginatedVM<T>
    {
        PaginatedList<T>? _list;

        public PaginatedList<T>? List
        {
            get => _list;
            set { _list = value; TotalPages = _list?.TotalPages; }
        }

        public int? TotalPages { get; set; }

        public PaginatedVM()
        {
        }
        public PaginatedVM(PaginatedList<T> list)
        {
            _list = list;
            TotalPages = _list?.TotalPages;
        }
        public static async Task<PaginatedVM<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var list = await PaginatedList<T>.CreateAsync(source, pageIndex, pageSize);
            return new PaginatedVM<T>(list);
        }

        public static PaginatedVM<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var list = PaginatedList<T>.Create(source, pageIndex, pageSize);
            return new PaginatedVM<T>(list);

        }
    }

}
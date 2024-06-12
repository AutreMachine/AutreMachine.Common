﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
        /// <summary>
    /// The viewModel allows to pass PaginatedList through API and REST endpoints.
    /// Otherwise, the fields TotalPages in PaginatedList can not be sent in JSON.
    /// That's why it is better to user PaginatedViewModel in Blazor pages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedViewModel<T>
    {
        PaginatedList<T>? _list;
        public PaginatedList<T>? List { get => _list;
            set { _list = value; TotalPages = _list?.TotalPages; } }

        public int? TotalPages { get; set; }

        public PaginatedViewModel()
        {
        }
        public PaginatedViewModel(PaginatedList<T> list)
        {
            _list = list;
        }
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            return await PaginatedList<T>.CreateAsync(source, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return PaginatedList<T>.Create(source, pageIndex, pageSize);
        }
    }

}

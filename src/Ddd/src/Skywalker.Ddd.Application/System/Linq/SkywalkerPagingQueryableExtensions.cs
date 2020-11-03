using Skywalker;
using Skywalker.Application.Dtos.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core;
using System.Text;

namespace System.Linq
{
    public static class SkywalkerPagingQueryableExtensions
    {
        /// <summary>
        /// Used for paging with an <see cref="IPagedResultRequest"/> object.
        /// </summary>
        /// <param name="query">Queryable to apply paging</param>
        /// <param name="pagedResultRequest">An object implements <see cref="IPagedResultRequest"/> interface</param>
        public static IQueryable<T> Page<T>([NotNull] this IQueryable<T> query, IPagedResultRequest pagedResultRequest)
        {
            Check.NotNull(query, nameof(query));

            return query.Page(pagedResultRequest.SkipCount, pagedResultRequest.MaxResultCount);
        }
    }
}

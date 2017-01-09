using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dcp.Utility.Services.Data
{
    public interface IHqlQueryFilter
    {
        IHqlQuery ExecuteFilter(IHqlQuery query);
    }

    public static class HqlQueryFilterHelper
    {
        public static IHqlQuery ApplyFilters(this IHqlQuery query, params IHqlQueryFilter[] filters)
        {
            return ApplyFilters(query, filters.AsEnumerable());
        }

        public static IHqlQuery ApplyFilters(this IHqlQuery query, IEnumerable<IHqlQueryFilter> filters)
        {
            return filters.Aggregate(query, (current, filter) => filter.ExecuteFilter(current));
        }
    }
}
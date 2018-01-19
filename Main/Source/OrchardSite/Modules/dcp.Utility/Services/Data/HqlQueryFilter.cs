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

    public static class HqlQueryHelper
    {
        public static IHqlQuery ApplyFilters(this IHqlQuery query, params IHqlQueryFilter[] filters)
        {
            return ApplyFilters(query, filters.AsEnumerable());
        }

        public static IHqlQuery ApplyFilters(this IHqlQuery query, IEnumerable<IHqlQueryFilter> filters)
        {
            return filters.Aggregate(query, (current, filter) => filter.ExecuteFilter(current));
        }

        public static IHqlQuery ApplyOrders(this IHqlQuery query, params IHqlQueryOrder[] orders)
        {
            return ApplyOrders(query, orders.AsEnumerable());
        }

        public static IHqlQuery ApplyOrders(this IHqlQuery query, IEnumerable<IHqlQueryOrder> orders)
        {
            return orders.Aggregate(query, (current, filter) => filter.ExecuteOrderAsc(current));
        }
    }

    public interface IHqlQueryOrder
    {
        IHqlQuery ExecuteOrderAsc(IHqlQuery query);
    }
}
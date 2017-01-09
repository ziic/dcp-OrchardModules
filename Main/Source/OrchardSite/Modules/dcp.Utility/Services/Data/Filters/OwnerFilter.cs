using dcp.Utility.Services;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dcp.Utility.Services.Data.Filters
{
    public class OwnerFilter : IHqlQueryFilter
    {
        private int _userId;

        public OwnerFilter(int userId)
        {
            _userId = userId;
        }
        
        public IHqlQuery ExecuteFilter(IHqlQuery query)
        {           

            query = query.Where(a => a.ContentPartRecord<CommonPartRecord>(),
                p => p.Eq("OwnerId", _userId));
            
            return query;
        }
    }
}
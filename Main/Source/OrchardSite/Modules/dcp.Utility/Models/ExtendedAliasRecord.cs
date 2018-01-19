using Orchard.Alias.Records;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dcp.Utility.Models
{
    public class ExtendedAliasRecord
    {
        public virtual int Id { get; set; }
        public virtual string RouteName { get; set; }
        public virtual AliasRecord AliasRecord { get; set; }
    }
}
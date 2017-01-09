using Orchard.Alias.Records;
using Orchard.Environment.Extensions;

namespace dcp.Utility.Models
{
    [OrchardFeature("dcp.Routing")]
    public class ExtendedAliasRecord
    {
        public virtual int Id { get; set; }
        public virtual string RouteName { get; set; }
        public virtual AliasRecord AliasRecord { get; set; }
    }
}
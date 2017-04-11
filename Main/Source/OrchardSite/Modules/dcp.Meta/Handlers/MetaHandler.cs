using dcp.Meta.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace dcp.Meta.Handlers
{
    public class MetaHandler : ContentHandler
    {
        public MetaHandler(IRepository<MetaRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}
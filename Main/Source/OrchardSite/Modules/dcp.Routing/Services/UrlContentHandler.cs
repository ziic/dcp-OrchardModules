using System.Collections.Generic;
using dcp.Routing.Models;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;

namespace dcp.Routing.Services
{
    [OrchardFeature("dcp.Routing.UrlUpdating")]
    public class UrlContentHandler : ContentHandler
    {
        private readonly Dictionary<int, string> _updates = new Dictionary<int, string>();

        public UrlContentHandler(IRoutingAppService routingAppService)
        {
            OnUpdating<AutoroutePart>((ctx, part) =>
            {
                if (_updates.ContainsKey(part.Id))
                {
                    _updates[part.Id] = part.DisplayAlias;
                }
                else
                {
                    _updates.Add(part.Id, part.DisplayAlias);
                }
            });

            OnUpdated<AutoroutePart>((ctx, part) =>
            {
                if (!_updates.ContainsKey(part.Id))
                    return;

                var sourceUrl = _updates[part.Id];
                if (string.IsNullOrWhiteSpace(sourceUrl) || string.IsNullOrWhiteSpace(part.DisplayAlias))
                    return;

                if (string.Equals(sourceUrl.TrimStart('/'), part.DisplayAlias.TrimStart('/'), System.StringComparison.OrdinalIgnoreCase))
                    return;

                routingAppService.Add(new RedirectRule
                {
                    SourceUrl = sourceUrl,
                    DestinationUrl = part.DisplayAlias,
                    IsPermanent = true
                });
            });

        }
    }
    
}
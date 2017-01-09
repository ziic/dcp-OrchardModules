using System;
using System.Collections.Generic;
using System.Linq;
using Orchard;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Security.Permissions;

namespace dcp.Utility
{
    public abstract class SecurityShapeTableProviderBase : IShapeTableProvider
    {
        private readonly IOrchardServices _orchardServices;

        protected SecurityShapeTableProviderBase(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        protected abstract IEnumerable<Tuple<string, IEnumerable<string>, IEnumerable<string>, IEnumerable<Permission>>> ShapeDescriptions { get; }

        public void Discover(ShapeTableBuilder builder)
        {
            foreach (var shapeDescription in ShapeDescriptions)
            {
                var contentType = shapeDescription.Item1;
                var notAuthorizedDeniedShapeNames = shapeDescription.Item2;
                var authorizedDeniedShapeNames = shapeDescription.Item3;
                var permissions = shapeDescription.Item4;
                foreach (var deniedShapeName in notAuthorizedDeniedShapeNames)
                {
                    var comps = deniedShapeName.Split('-');
                    var originalShapeName = comps[0];
                    var differentiator = comps.ElementAtOrDefault(1);
                    
                    builder.Describe(originalShapeName).Placement(context =>
                    {
                        if (context.ContentType != contentType)
                            return null;
                        
                        if (context.Differentiator != differentiator)
                            return null;

                        if (permissions.Any(permission => !_orchardServices.Authorizer.Authorize(permission, context.Content)))
                        {
                            return new PlacementInfo
                            {
                                Location = "-",
                                Source = string.Empty
                            };
                        }
                        return null;
                    });
                }

                if (authorizedDeniedShapeNames == null)
                    return;

                foreach (var deniedShapeName in authorizedDeniedShapeNames)
                {
                    var comps = deniedShapeName.Split('-');
                    var originalShapeName = comps[0];
                    var differentiator = comps.ElementAtOrDefault(1);
                    builder.Describe(originalShapeName).Placement(context =>
                    {
                        if (context.ContentType != contentType)
                            return null;

                        if (context.Differentiator != differentiator)
                            return null;

                        if (permissions.Any(permission => _orchardServices.Authorizer.Authorize(permission, context.Content)))
                        {
                            return new PlacementInfo
                            {
                                Location = "-",
                                Source = string.Empty
                            };
                        }
                        return null;
                    });
                }
            }

        }
    }
}
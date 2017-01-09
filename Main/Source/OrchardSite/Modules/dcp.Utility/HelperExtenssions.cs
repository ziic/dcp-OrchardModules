using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Security.Permissions;
using System.Web;

namespace dcp.Utility
{
    public static class ContentItemHelper
    {
        public static T GetField<T>(this ContentItem contentItem, string fieldName) where T : ContentField
        {
            return contentItem.GetField<T>(contentItem.ContentType, fieldName);
        }

        public static T GetField<T>(this ContentItem contentItem, string partName, string fieldName) where T : ContentField
        {
            var part = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == partName);
            if (part == null)
                return null;

            var field = part.Fields.FirstOrDefault(x => x.Name == fieldName);

            return field as T;
        }

        public static T GetField<T>(this ContentPart part, string fieldName) where T : ContentField
        {
            var field = part.Fields.FirstOrDefault(x => x.Name == fieldName);
            return field as T;
        }
    }

    public static class AuthorizerHelper
    {
        public static bool AuthorizeAll(this IAuthorizer authorizer, IContent content, LocalizedString message, params Permission[] permissions)
        {
            return permissions.Select(permission => authorizer.Authorize(permission, content, message)).All(result => result);
        }

        public static bool AuthorizeAny(this IAuthorizer authorizer, IContent content, LocalizedString message, params Permission[] permissions)
        {
            return permissions.Select(permission => authorizer.Authorize(permission, content, message)).Any(result => result);
        }
    }

    public static class RequestHelper
    {
        public static string GetParamValue(this HttpRequestBase request, string paramName)
        {
            return request.Params[paramName] ?? (string)request.RequestContext.RouteData.Values[paramName];
        }
    }
}
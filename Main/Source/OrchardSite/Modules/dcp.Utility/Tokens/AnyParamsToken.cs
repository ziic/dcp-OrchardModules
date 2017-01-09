using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;
using System;

namespace dcp.Utility.Tokens
{
    [OrchardFeature("Tokens", FeatureName = "dcp.Tokens")]
    public class AnyParamTokens : ITokenProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public AnyParamTokens(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        private string _tokenName = "AnyParams";

        public void Describe(DescribeContext context)
        {
            context.For("Request")
                .Token(_tokenName + ":*", T(_tokenName + ":<element>"), T("The any parama from request (route data, cookie, form, querystring)."));
        }

        public void Evaluate(EvaluateContext context)
        {
            if (_workContextAccessor.GetContext().HttpContext == null)
            {
                return;
            }

            context.For("Request", _workContextAccessor.GetContext().HttpContext.Request)
                .Token(
                    token => token.StartsWith(_tokenName + ":", StringComparison.OrdinalIgnoreCase) ? token.Substring((_tokenName + ":").Length) : null,
                    (token, request) =>
                    {
                        var val = request.Params[token] ?? request.RequestContext.RouteData.Values[token].ToString();
                        if (val != null) 
                            return val;

                        var cookie = request.Cookies.Get(token);
                        val = cookie != null ? cookie.Value : null;
                        return val;
                    }
                );
        }
    }
}
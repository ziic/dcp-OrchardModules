using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using dcp.Routing.Models;
using NHibernate;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace dcp.Routing.Services
{
    public interface IRoutingAppService : IDependency
    {
        IEnumerable<RedirectRule> GetRedirects(int startIndex, int pageSize);
        IEnumerable<RedirectRule> GetRedirects(int[] itemIds);
        int GetRedirectsTotalCount();
        RedirectRule GetRedirect(int id);
        RedirectRule GetRedirect(string path);
        RedirectRule Update(RedirectRule redirectRule);
        RedirectRule Add(RedirectRule redirectRule);
        void Delete(int id);
        void Delete(RedirectRule redirectRule);
        bool MoveRedirectRulesToWebConfig(int[] itemIds, string filePath);
    }

    [OrchardFeature("dcp.Routing.Redirects")]
    public class RoutingAppService : IRoutingAppService
    {
        private readonly IRepository<RedirectRule> _repository;
        private readonly IWebConfigService _webConfigService;

        public RoutingAppService(IRepository<RedirectRule> repository, IWebConfigService webConfigService)
        {
            _repository = repository;
            _webConfigService = webConfigService;
        }

        public IEnumerable<RedirectRule> GetRedirects(int startIndex, int pageSize)
        {
            return _repository.Table.Skip(startIndex).Take(pageSize);
        }

        public int GetRedirectsTotalCount()
        {
            return _repository.Table.Count();
        }

        public RedirectRule GetRedirect(int id)
        {
            return _repository.Get(id);
        }

        public RedirectRule Update(RedirectRule redirectRule)
        {
            FixRedirect(redirectRule);
            AssertRule(redirectRule);
            _repository.Update(redirectRule);
            return redirectRule;
        }

        public RedirectRule Add(RedirectRule redirectRule)
        {
            FixRedirect(redirectRule);
            AssertRule(redirectRule);
            _repository.Create(redirectRule);
            return redirectRule;
        }

        public void Delete(int id)
        {
            var redirect = GetRedirect(id);

            _repository.Delete(redirect);
        }

        public void Delete(RedirectRule redirectRule)
        {
            _repository.Delete(redirectRule);
        }

        public bool MoveRedirectRulesToWebConfig(int[] itemIds, string filePath)
        {
            var redirects = GetRedirects(itemIds);

            var redirectsList = redirects as IList<RedirectRule> ?? redirects.ToList();
            var res = _webConfigService.AddRedirectRules(redirectsList, filePath);
            if (res)
                Delete(redirectsList);

            return res;
        }

        private void Delete(IEnumerable<RedirectRule> redirects)
        {
            foreach (var redirect in redirects)
            {
                Delete(redirect);
            }
        }

        public RedirectRule GetRedirect(string path)
        {
            path = HttpUtility.UrlDecode(path.TrimStart('/').ToLower());
            return _repository.Get(x => x.SourceUrl == path);
        }

        private static void FixRedirect(RedirectRule redirectRule)
        {
            redirectRule.SourceUrl = redirectRule.SourceUrl.TrimStart('/');
            redirectRule.DestinationUrl = redirectRule.DestinationUrl.TrimStart('/');
        }

        public IEnumerable<RedirectRule> GetRedirects(int[] itemIds)
        {
            return GetRedirectsPartially(itemIds);
        }

        private IEnumerable<RedirectRule> GetRedirectsPartially(int[] itemIds)
        {
            var i = 0;
            const int pageSize = 1000; //default NHiberant limit paramters
            var count = 0;
            var total = itemIds.Length;
            var res = new List<RedirectRule>();
            while (count < total)
            {
                var currentIds = itemIds.Skip(count).Take(pageSize).ToArray();
                var redirects = _repository.Fetch(x => currentIds.Contains(x.Id));
                res.AddRange(redirects);
                i++;
                count = i * pageSize;
            }
            return res;
        }

        public bool IsValidRule(RedirectRule redirectRule)
        {
            if (!Validator.TryValidateObject(redirectRule, new ValidationContext(redirectRule), new List<ValidationResult>(), true))
                return false;
            
            FixRedirect(redirectRule);
            
            if (string.Equals(redirectRule.SourceUrl, redirectRule.DestinationUrl, StringComparison.OrdinalIgnoreCase))
                return false;

            var rule = _repository.Get(x => x.SourceUrl == redirectRule.DestinationUrl);
            if (rule == null)
                return true;

            return !string.Equals(rule.DestinationUrl, redirectRule.SourceUrl, StringComparison.OrdinalIgnoreCase);
        }

        private void AssertRule(RedirectRule redirectRule)
        {
            if (!IsValidRule(redirectRule))
                throw new ApplicationException("The is circular redirect rule");
        }
    }
}
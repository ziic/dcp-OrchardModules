using Orchard;
using Orchard.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace dcp.Utility.Services.Data
{
    public interface IContentRepositoryBase : IDependency
    {
        IEnumerable<ContentItem> FindBy(params IHqlQueryFilter[] filters);
        IEnumerable<ContentItem> GetAll();
        ContentItem Get(int id);
        IEnumerable<T> FindBy<T>(IEnumerable<IHqlQueryFilter> filters) where T : ContentPart;
        IEnumerable<T> FindBy<T>(params IHqlQueryFilter[] filters) where T : ContentPart;
        void Remove(ContentItem contentItem);
        void Remove<T>(T contentPart) where T : ContentPart;
        ContentItem Create();
    }

    public interface IContentRepositoryBase<out T> : IContentRepositoryBase where T : ContentPart
    {
        new T Create();
        new IEnumerable<T> GetAll();
        new T Get(int id);
        IEnumerable<T> FindBy(IEnumerable<IHqlQueryFilter> filters);
        new IEnumerable<T> FindBy(params IHqlQueryFilter[] filters);
    }

    public abstract class ContentRepositoryBase : IContentRepositoryBase
    {
        protected readonly IContentManager ContentManager;
        protected readonly string ContentType;

        protected ContentRepositoryBase(string contentType, IContentManager contentManager)
        {
            ContentManager = contentManager;
            ContentType = contentType;
        }

        public IEnumerable<ContentItem> GetAll()
        {
            return BuildBaseHqlQuery().List();
        }

        public ContentItem Get(int id)
        {
            return ContentManager.Get(id);
        }

        public IEnumerable<ContentItem> FindBy(IEnumerable<IHqlQueryFilter> filters)
        {
            var query = BuildHqlQueryWithFilters(filters);

            return query.List();
        }

        public IEnumerable<T> FindBy<T>(IEnumerable<IHqlQueryFilter> filters) where T : ContentPart
        {
            return FindBy(filters).AsPart<T>(); 
        }

        public IEnumerable<ContentItem> FindBy(params IHqlQueryFilter[] filters)
        {
            return FindBy(filters.AsEnumerable());
        }

        public IEnumerable<T> FindBy<T>(params IHqlQueryFilter[] filters) where T : ContentPart
        {
            return FindBy(filters).AsPart<T>();
        }

        public void Remove(ContentItem contentItem)
        {
            ContentManager.Remove(contentItem);
        }

        public void Remove<T>(T contentPart) where T : ContentPart
        {
            Remove(contentPart.ContentItem);
        }

        public ContentItem Create()
        {
            return ContentManager.Create(ContentType, VersionOptions.Draft);
        }

        protected IHqlQuery BuildHqlQueryWithFilters(IEnumerable<IHqlQueryFilter> filters)
        {
            var query = BuildBaseHqlQuery();
            query = query.ApplyFilters(filters);
            return query;
        }

        protected IHqlQuery BuildBaseHqlQuery()
        {
            return ContentManager.HqlQuery().ForType(ContentType).ForVersion(VersionOptions.Latest);
        }

        protected IContentQuery<ContentItem> BuildBaseQuery()
        {
            return ContentManager.Query()
                .ForType("ConditerCategory");
        }
    }

    public abstract class DefaultContentRepositoryBase<T> : ContentRepositoryBase, IContentRepositoryBase<T> where T : ContentPart
    {
        protected DefaultContentRepositoryBase(string contentType, IContentManager contentManager) : base(contentType, contentManager)
        {
        }

        public new T Create()
        {
            return base.Create().As<T>();
        }

        public new IEnumerable<T> GetAll()
        {
            return base.GetAll().AsPart<T>();
        }

        public new T Get(int id)
        {
            return base.Get(id).As<T>();
        }

        public new IEnumerable<T> FindBy(IEnumerable<IHqlQueryFilter> filters)
        {
            return FindBy<T>(filters);
        }

        public new IEnumerable<T> FindBy(params IHqlQueryFilter[] filters)
        {
            return FindBy<T>(filters);
        }
    }
}
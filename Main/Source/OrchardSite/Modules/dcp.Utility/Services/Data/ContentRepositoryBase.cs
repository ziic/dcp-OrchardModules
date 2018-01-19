using Orchard;
using Orchard.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.Records;

namespace dcp.Utility.Services.Data
{
    public interface IContentRepositoryBase : IDependency
    {
        IEnumerable<ContentItem> FindBy(params IHqlQueryFilter[] filters);
        IEnumerable<ContentItem> GetAll();
        ContentItem Get(int id);
        IEnumerable<ContentItem> GetMany(int[] ids);
        IEnumerable<T> FindBy<T>(IEnumerable<IHqlQueryFilter> filters) where T : ContentPart;
        IEnumerable<T> FindBy<T>(params IHqlQueryFilter[] filters) where T : ContentPart;
        IEnumerable<ContentItem> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters);
        IEnumerable<ContentItem> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters, IEnumerable<IHqlQueryOrder> orders);
        IEnumerable<ContentItem> FindBy(int skip, int take, params IHqlQueryFilter[] filters);

        void Remove(ContentItem contentItem);
        void Remove<T>(T contentPart) where T : ContentPart;
        

        int Count(IEnumerable<IHqlQueryFilter> filters);
        int Count(params IHqlQueryFilter[] filters);

        ContentItem New();

        /// <summary>
        /// Create new empty content item draft, add to repository/content manager
        /// </summary>
        /// <returns>Content item</returns>
        ContentItem NewAndAdd(VersionOptions opt = null);

        /// <summary>
        /// Added with publish state to repository/content manager
        /// </summary>
        /// <param name="contentItem"></param>
        /// <returns></returns>
        ContentItem Add(ContentItem contentItem);
    }

    public interface IContentRepositoryBase<T> : IContentRepositoryBase where T : ContentPart
    {
        new T New();
        new T NewAndAdd(VersionOptions opt = null);
        T Add(T part);
        new IEnumerable<T> GetAll();
        new T Get(int id);
        IEnumerable<T> GetMany(IEnumerable<int> ids);
        IEnumerable<T> FindBy(IEnumerable<IHqlQueryFilter> filters);
        new IEnumerable<T> FindBy(params IHqlQueryFilter[] filters);
        new IEnumerable<T> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters);
        new IEnumerable<T> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters, IEnumerable<IHqlQueryOrder> orders);
        new IEnumerable<T> FindBy(int skip, int take, params IHqlQueryFilter[] filters);
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
            return FindBy(filters)
                .AsPart<T>()
                .ToList();
        }

        public IEnumerable<ContentItem> FindBy(params IHqlQueryFilter[] filters)
        {
            return FindBy(filters.AsEnumerable());
        }

        public IEnumerable<T> FindBy<T>(params IHqlQueryFilter[] filters) where T : ContentPart
        {
            return FindBy(filters)
                .AsPart<T>()
                .ToList();
        }

        public IEnumerable<ContentItem> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters)
        {
            var query = BuildHqlQueryWithFilters(filters);
            return query.Slice(skip, take)
                .ToList();
        }

        public IEnumerable<ContentItem> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters, IEnumerable<IHqlQueryOrder> orders)
        {
            var query = BuildHqlQueryWithFilters(filters);
            query = BuildHqlQueryWithOrders(query, orders);
            return query.Slice(skip, take)
                .ToList();
        }

        public IEnumerable<ContentItem> FindBy(int skip, int take, params IHqlQueryFilter[] filters)
        {
            return FindBy(skip, take, filters.AsEnumerable());
        }

        public void Remove(ContentItem contentItem)
        {
            ContentManager.Remove(contentItem);
        }

        public void Remove<T>(T contentPart) where T : ContentPart
        {
            Remove(contentPart.ContentItem);
        }

        public ContentItem NewAndAdd(VersionOptions opt = null)
        {
            if (opt == null)
                opt = VersionOptions.Draft;
            return ContentManager.Create(ContentType, opt);
        }

        public ContentItem Add(ContentItem contentItem)
        {
            //add with publish state
            ContentManager.Create(contentItem);
            
            return contentItem;
        }

        public ContentItem New()
        {
            var contentItem = ContentManager.New(ContentType);

            //fix Orchard bug: ContentFieldPicker doesn't save value if create cotnent item with New method
            if (contentItem.VersionRecord == null)
            {
                // produce root record to determine the model id
                contentItem.VersionRecord = new ContentItemVersionRecord
                {
                    ContentItemRecord = new ContentItemRecord(),
                    Number = 1,
                    Latest = true,
                    Published = true
                };
            }

            return contentItem;
        }

        protected IHqlQuery BuildHqlQueryWithFilters(IEnumerable<IHqlQueryFilter> filters)
        {
            var query = BuildBaseHqlQuery();
            query = query.ApplyFilters(filters);
            return query;
        }

        protected IHqlQuery BuildHqlQueryWithOrders(IHqlQuery query, IEnumerable<IHqlQueryOrder> orders)
        {
            query = query.ApplyOrders(orders);
            return query;
        }

        protected IHqlQuery BuildBaseHqlQuery()
        {
            return ContentManager.HqlQuery().ForType(ContentType).ForVersion(VersionOptions.Latest);
        }

        protected IContentQuery<ContentItem> BuildBaseQuery()
        {
            return ContentManager.Query()
                .ForType(ContentType);
        }

        public int Count(IEnumerable<IHqlQueryFilter> filters)
        {
            var query = BuildHqlQueryWithFilters(filters);

            return query.Count();
        }

        public int Count(params IHqlQueryFilter[] filters)
        {
            return Count(filters.AsEnumerable());
        }

        public IEnumerable<ContentItem> GetMany(int[] ids)
        {
            return ContentManager.GetMany<ContentItem>(ids, VersionOptions.Published, QueryHints.Empty);
        }
    }

    public abstract class DefaultContentRepositoryBase<T> : ContentRepositoryBase, IContentRepositoryBase<T> where T : ContentPart
    {
        protected DefaultContentRepositoryBase(string contentType, IContentManager contentManager) : base(contentType, contentManager)
        {
        }

        public new T NewAndAdd(VersionOptions opt = null)
        {
            return base.NewAndAdd(opt).As<T>();
        }

        public T Add(T part)
        {
            return Add(part.ContentItem).As<T>();
        }

        public new T New()
        {
            return base.New().As<T>();
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

        public new IEnumerable<T> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters)
        {
            return base.FindBy(skip, take, filters).AsPart<T>();
        }

        public new IEnumerable<T> FindBy(int skip, int take, IEnumerable<IHqlQueryFilter> filters, IEnumerable<IHqlQueryOrder> orders)
        {
            return base.FindBy(skip, take, filters, orders).AsPart<T>();
        }

        public new IEnumerable<T> FindBy(int skip, int take, params IHqlQueryFilter[] filters)
        {
            return FindBy(skip, take, filters.AsEnumerable());
        }

        public IEnumerable<T> GetMany(IEnumerable<int> ids)
        {
            return ContentManager.GetMany<T>(ids, VersionOptions.Published, QueryHints.Empty);
        }
    }
}
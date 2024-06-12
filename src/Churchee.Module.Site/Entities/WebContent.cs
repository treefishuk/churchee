﻿using System.ComponentModel.DataAnnotations;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class WebContent : AggregateRoot, IHierarchical<WebContent>, ISourced
    {
        protected WebContent()
        {
            Children = new HashSet<WebContent>();
        }


        protected WebContent(Guid applicationTenantId, string source, string title, string url, string description) : base(applicationTenantId)
        {
            RedirectUrls = new HashSet<RedirectUrl>();
            Url = url;
            Title = title;
            Description = description;
            Children = new HashSet<WebContent>();
            SourceName = source;
        }

        protected WebContent(Guid applicationTenantId, string title, string url, string description) : base(applicationTenantId)
        {
            RedirectUrls = new HashSet<RedirectUrl>();
            Url = url;
            Title = title;
            Description = description;
            Children = new HashSet<WebContent>();
            SourceName = "Admin";
        }
        public string Title { get; protected set; }

        public virtual string Description { get; protected set; }

        [Required]
        public string Url { get; set; }

        public ICollection<RedirectUrl> RedirectUrls { get; private set; }

        public void AddRedirectUrl(string url)
        {
            RedirectUrls.Add(new RedirectUrl(url));

        }

        public Guid? PageTypeId { get; set; }

        public virtual PageType PageType { get; set; }


        /// <summary>
        /// For System Pages Such as Blogs, Events or Podcasts
        /// </summary>
        public bool IsSystem { get; set; }

        public Guid? ParentId { get; set; }

        public virtual Page Parent { get; set; }

        public bool HasChildren { get; private set; }

        public ICollection<WebContent> Children { get; set; }

        public string PublishedData { get; protected set; }

        public bool Published { get; protected set; }

        public DateTime? LastPublishedDate { get; protected set; }

        public string SourceName { get; protected set; }

        public string SourceId { get; protected set; }
    }
}

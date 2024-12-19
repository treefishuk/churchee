using System.Xml.Serialization;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnablePodcasts
{
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "rss")]
    public class Rss
    {
        public Rss()
        {
            Channel = new RssChannel();
        }

        [XmlElement("channel")]
        public RssChannel Channel { get; set; }

        [XmlAttribute("version")]
        public decimal Version { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RssChannel
    {
        public RssChannel()
        {
            Items = [];
            ItemsElementName = [];
        }

        [XmlElement("item", typeof(RssChannelItem))]
        public RssChannelItem[] Items { get; set; }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType[] ItemsElementName { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "Category")]
    public class Category
    {
        public Category()
        {
            CategoryCategory = new CategoryCategory();
            Text = string.Empty;
        }

        [XmlAttribute("category")]
        public CategoryCategory CategoryCategory { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    public class CategoryCategory
    {
        public CategoryCategory()
        {
            Text = string.Empty;
        }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "image")]
    public class Image
    {
        public Image()
        {
            Href = string.Empty;
        }

        [XmlAttribute("href")]
        public string Href { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "owner")]
    public class Owner
    {
        public Owner()
        {
            Name = string.Empty;
            Email = string.Empty;
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false, ElementName = "link")]
    public class Link
    {
        public Link()
        {
            Href = string.Empty;
            Rel = string.Empty;
            Type = string.Empty;
        }

        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("rel")]
        public string Rel { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RssChannelImage
    {

        public RssChannelImage()
        {
            Url = string.Empty;
            Title = string.Empty;
            Link = string.Empty;
        }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RssChannelItem
    {
        public RssChannelItem()
        {
            Title = string.Empty;
            Description = string.Empty;
            Link = string.Empty;
            Guid = new RssChannelItemGuid();
            Creator = string.Empty;
            PubDate = string.Empty;
            Enclosure = new RssChannelItemEnclosure();
            Summary = string.Empty;
            Explicit = string.Empty;
            Image = new Image();
            EpisodeType = string.Empty;
        }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("guid")]
        public RssChannelItemGuid Guid { get; set; }

        [XmlElement(Namespace = "http://purl.org/dc/elements/1.1/", ElementName = "creator")]
        public string Creator { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("enclosure")]
        public RssChannelItemEnclosure Enclosure { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "summary")]
        public string Summary { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "@explicit")]
        public string Explicit { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", DataType = "time", ElementName = "duration")]
        public DateTime Duration { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "image")]
        public Image Image { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "season")]
        public ushort Season { get; set; }

        [XmlElement("seasonSpecified")]
        [XmlIgnore]
        public bool SeasonSpecified { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "episode")]
        public byte Episode { get; set; }

        [XmlElement("episodeSpecified")]
        [XmlIgnore]
        public bool EpisodeSpecified { get; set; }

        [XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "episodeType")]
        public string EpisodeType { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RssChannelItemGuid
    {
        public RssChannelItemGuid()
        {
            Value = string.Empty;
        }

        [XmlAttribute("isPermaLink")]
        public bool IsPermaLink { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class RssChannelItemEnclosure
    {
        public RssChannelItemEnclosure()
        {
            Url = string.Empty;
            Type = string.Empty;
        }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("length")]
        public uint Length { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    [Serializable]
    [XmlType(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {
        author,
        copyright,
        description,
        generator,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:author")]
        author1,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:category")]
        category,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:explicit")]
        @explicit,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:image")]
        image,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:owner")]
        owner,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:summary")]
        summary,
        [XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:type")]
        type,
        [XmlEnum("http://www.w3.org/2005/Atom:link")]
        link,
        [XmlEnum("image")]
        image1,
        item,
        language,
        lastBuildDate,
        [XmlEnum("link")]
        link1,
        title,
    }
}

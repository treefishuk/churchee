namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnablePodcasts
{
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false, ElementName = "rss")]
    public partial class Rss
    {
        private RssChannel _channelField;

        private decimal _versionField;

        [System.Xml.Serialization.XmlElement("channel")]
        public RssChannel Channel
        {
            get => _channelField;
            set => _channelField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("version")]
        public decimal Version
        {
            get => _versionField;
            set => _versionField = value;
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RssChannel
    {

        private RssChannelItem[] _itemsField;

        private ItemsChoiceType[] _itemsElementNameField;


        [System.Xml.Serialization.XmlElement("item", typeof(RssChannelItem))]
        public RssChannelItem[] Items
        {
            get => _itemsField;
            set => _itemsField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnore]
        public ItemsChoiceType[] ItemsElementName
        {
            get => _itemsElementNameField;
            set => _itemsElementNameField = value;
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "Category")]
    public partial class Category
    {

        private CategoryCategory _category1Field;

        private string _textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("category")]
        public CategoryCategory Category1
        {
            get => _category1Field;
            set => _category1Field = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("text")]
        public string Text
        {
            get
            {
                return _textField;
            }
            set
            {
                _textField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    public partial class CategoryCategory
    {

        private string textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("text")]
        public string Text
        {
            get
            {
                return textField;
            }
            set
            {
                textField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "image")]
    public partial class Image
    {

        private string hrefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("href")]
        public string Href
        {
            get
            {
                return hrefField;
            }
            set
            {
                hrefField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false, ElementName = "owner")]
    public partial class Owner
    {

        private string nameField;

        private string emailField;

        [System.Xml.Serialization.XmlElement("name")]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("email")]
        public string Email
        {
            get
            {
                return emailField;
            }
            set
            {
                emailField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [System.Xml.Serialization.XmlRoot(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false, ElementName = "link")]
    public partial class Link
    {

        private string hrefField;

        private string relField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("href")]
        public string Href
        {
            get
            {
                return hrefField;
            }
            set
            {
                hrefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("rel")]
        public string Rel
        {
            get
            {
                return relField;
            }
            set
            {
                relField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("type")]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RssChannelImage
    {

        private string urlField;

        private string titleField;

        private string linkField;

        [System.Xml.Serialization.XmlElement("url")]
        public string Url
        {
            get
            {
                return urlField;
            }
            set
            {
                urlField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("title")]
        public string Title
        {
            get
            {
                return titleField;
            }
            set
            {
                titleField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("link")]
        public string Link
        {
            get
            {
                return linkField;
            }
            set
            {
                linkField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RssChannelItem
    {

        private string titleField;

        private string descriptionField;

        private string linkField;

        private RssChannelItemGuid guidField;

        private string creatorField;

        private string pubDateField;

        private RssChannelItemEnclosure enclosureField;

        private string summaryField;

        private string explicitField;

        private DateTime durationField;

        private Image imageField;

        private ushort seasonField;

        private bool seasonFieldSpecified;

        private byte episodeField;

        private bool episodeFieldSpecified;

        private string episodeTypeField;


        [System.Xml.Serialization.XmlElement("title")]
        public string Title
        {
            get
            {
                return titleField;
            }
            set
            {
                titleField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("description")]
        public string Description
        {
            get
            {
                return descriptionField;
            }
            set
            {
                descriptionField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("link")]
        public string Link
        {
            get
            {
                return linkField;
            }
            set
            {
                linkField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("guid")]
        public RssChannelItemGuid Guid
        {
            get
            {
                return guidField;
            }
            set
            {
                guidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://purl.org/dc/elements/1.1/", ElementName = "creator")]
        public string Creator
        {
            get
            {
                return creatorField;
            }
            set
            {
                creatorField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("pubDate")]
        public string PubDate
        {
            get
            {
                return pubDateField;
            }
            set
            {
                pubDateField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("enclosure")]
        public RssChannelItemEnclosure Enclosure
        {
            get
            {
                return enclosureField;
            }
            set
            {
                enclosureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "summary")]
        public string Summary
        {
            get
            {
                return summaryField;
            }
            set
            {
                summaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "@explicit")]
        public string Explicit
        {
            get
            {
                return explicitField;
            }
            set
            {
                explicitField = value;
            }
        }

        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", DataType = "time", ElementName = "duration")]
        public DateTime Duration
        {
            get
            {
                return durationField;
            }
            set
            {
                durationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "image")]
        public Image Image
        {
            get
            {
                return imageField;
            }
            set
            {
                imageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "season")]
        public ushort Season
        {
            get
            {
                return seasonField;
            }
            set
            {
                seasonField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("seasonSpecified")]
        [System.Xml.Serialization.XmlIgnore]
        public bool SeasonSpecified
        {
            get
            {
                return seasonFieldSpecified;
            }
            set
            {
                seasonFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "episode")]
        public byte Episode
        {
            get
            {
                return episodeField;
            }
            set
            {
                episodeField = value;
            }
        }

        [System.Xml.Serialization.XmlElement("episodeSpecified")]
        [System.Xml.Serialization.XmlIgnore]
        public bool EpisodeSpecified
        {
            get
            {
                return episodeFieldSpecified;
            }
            set
            {
                episodeFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", ElementName = "episodeType")]
        public string EpisodeType
        {
            get
            {
                return episodeTypeField;
            }
            set
            {
                episodeTypeField = value;
            }
        }
    }

    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RssChannelItemGuid
    {

        private bool isPermaLinkField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("isPermaLink")]
        public bool IsPermaLink
        {
            get
            {
                return isPermaLinkField;
            }
            set
            {
                isPermaLinkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlText]
        public string Value
        {
            get
            {
                return valueField;
            }
            set
            {
                valueField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class RssChannelItemEnclosure
    {

        private string urlField;

        private uint lengthField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("url")]
        public string Url
        {
            get
            {
                return urlField;
            }
            set
            {
                urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("length")]
        public uint Length
        {
            get
            {
                return lengthField;
            }
            set
            {
                lengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute("type")]
        public string Type
        {
            get
            {
                return typeField;
            }
            set
            {
                typeField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        author,

        /// <remarks/>
        copyright,

        /// <remarks/>
        description,

        /// <remarks/>
        generator,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:author")]
        author1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:category")]
        category,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:explicit")]
        @explicit,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:image")]
        image,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:owner")]
        owner,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:summary")]
        summary,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.itunes.com/dtds/podcast-1.0.dtd:type")]
        type,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("http://www.w3.org/2005/Atom:link")]
        link,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("image")]
        image1,

        /// <remarks/>
        item,

        /// <remarks/>
        language,

        /// <remarks/>
        lastBuildDate,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnum("link")]
        link1,

        /// <remarks/>
        title,
    }


}


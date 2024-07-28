using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnablePodcasts
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class rss
    {

        private rssChannel _channelField;

        private decimal _versionField;

        /// <remarks/>
        public rssChannel channel
        {
            get => _channelField;
            set => _channelField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get => _versionField;
            set => _versionField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rssChannel
    {

        private rssChannelItem[] _itemsField;

        private ItemsChoiceType[] _itemsElementNameField;

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute("author", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("copyright", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("description", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("generator", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("author", typeof(string), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("category", typeof(category), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("explicit", typeof(string), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("image", typeof(image), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("owner", typeof(owner), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("summary", typeof(string), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("type", typeof(string), Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        //[System.Xml.Serialization.XmlElementAttribute("link", typeof(link), Namespace = "http://www.w3.org/2005/Atom")]
        //[System.Xml.Serialization.XmlElementAttribute("image", typeof(rssChannelImage))]
        [System.Xml.Serialization.XmlElementAttribute("item", typeof(rssChannelItem))]
        //[System.Xml.Serialization.XmlElementAttribute("language", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("lastBuildDate", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("link", typeof(string))]
        //[System.Xml.Serialization.XmlElementAttribute("title", typeof(string))]
        //[System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public rssChannelItem[] Items
        {
            get => _itemsField;
            set => _itemsField = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get => _itemsElementNameField;
            set => _itemsElementNameField = value;
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false)]
    public partial class category
    {

        private categoryCategory _category1Field;

        private string _textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("category")]
        public categoryCategory category1
        {
            get => _category1Field;
            set => _category1Field = value;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string text
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    public partial class categoryCategory
    {

        private string textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string text
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false)]
    public partial class image
    {

        private string hrefField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", IsNullable = false)]
    public partial class owner
    {

        private string nameField;

        private string emailField;

        /// <remarks/>
        public string name
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

        /// <remarks/>
        public string email
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2005/Atom")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public partial class link
    {

        private string hrefField;

        private string relField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string href
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rel
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rssChannelImage
    {

        private string urlField;

        private string titleField;

        private string linkField;

        /// <remarks/>
        public string url
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
        public string title
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

        /// <remarks/>
        public string link
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rssChannelItem
    {

        private string titleField;

        private string descriptionField;

        private string linkField;

        private rssChannelItemGuid guidField;

        private string creatorField;

        private string pubDateField;

        private rssChannelItemEnclosure enclosureField;

        private string summaryField;

        private string explicitField;

        private System.DateTime durationField;

        private image imageField;

        private ushort seasonField;

        private bool seasonFieldSpecified;

        private byte episodeField;

        private bool episodeFieldSpecified;

        private string episodeTypeField;

        /// <remarks/>
        public string title
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

        /// <remarks/>
        public string description
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

        /// <remarks/>
        public string link
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

        /// <remarks/>
        public rssChannelItemGuid guid
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/elements/1.1/")]
        public string creator
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

        /// <remarks/>
        public string pubDate
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

        /// <remarks/>
        public rssChannelItemEnclosure enclosure
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public string summary
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public string @explicit
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd", DataType = "time")]
        public System.DateTime duration
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public image image
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public ushort season
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

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool seasonSpecified
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public byte episode
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

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool episodeSpecified
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.itunes.com/dtds/podcast-1.0.dtd")]
        public string episodeType
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rssChannelItemGuid
    {

        private bool isPermaLinkField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isPermaLink
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
        [System.Xml.Serialization.XmlTextAttribute()]
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class rssChannelItemEnclosure
    {

        private string urlField;

        private uint lengthField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string url
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint length
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
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
    [System.SerializableAttribute()]
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
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:author")]
        author1,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:category")]
        category,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:explicit")]
        @explicit,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:image")]
        image,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:owner")]
        owner,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:summary")]
        summary,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.itunes.com/dtds/podcast-1.0.dtd:type")]
        type,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/2005/Atom:link")]
        link,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("image")]
        image1,

        /// <remarks/>
        item,

        /// <remarks/>
        language,

        /// <remarks/>
        lastBuildDate,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("link")]
        link1,

        /// <remarks/>
        title,
    }


}


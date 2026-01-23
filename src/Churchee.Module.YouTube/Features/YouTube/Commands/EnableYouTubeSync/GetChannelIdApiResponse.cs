using System.Text.Json.Serialization;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync
{
    public class Item
    {
        public Item()
        {
            Id = string.Empty;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class GetChannelIdApiResponse
    {
        public GetChannelIdApiResponse()
        {
            Items = [];
        }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        public string ChannelId
        {
            get
            {
                if (Items.Count == 0)
                {
                    return string.Empty;
                }
                return Items[0].Id;
            }
        }

    }


}

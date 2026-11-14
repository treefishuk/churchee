using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.ValueTypes
{
    public class ImageUploadType : Upload
    {
        public ImageUploadType()
        {
            Path = string.Empty;
            ThumbnailUrl = string.Empty;
            SupportedFileTypes = ".jpg,.jpeg,.png,.gif";
        }

        public Guid ApplicationTenantId { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool ShowDescription { get; set; } = true;

        [Required]
        public string Description { get; set; }
    }
}

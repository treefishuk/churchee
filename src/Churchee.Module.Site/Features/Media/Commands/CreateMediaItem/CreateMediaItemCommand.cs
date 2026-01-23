using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaItemCommand : IRequest<CommandResponse>
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string LinkUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string Base64Content { get; set; }

        public Guid? FolderId { get; set; }

        public string CssClass { get; set; }

        public string SupportedFileTypes { get; set; }

        public int Order { get; set; }

        public class Builder
        {
            private readonly CreateMediaItemCommand _command = new();

            public Builder SetSupportedFileTypes(string supportedFileTypes)
            {
                _command.SupportedFileTypes = supportedFileTypes;
                return this;
            }

            public Builder SetName(string name)
            {
                _command.Name = name;
                return this;
            }

            public Builder SetFileName(string fileName)
            {
                string dashedFileName = string.Join("-", fileName.Split(' ', StringSplitOptions.RemoveEmptyEntries));

                _command.FileName = dashedFileName;

                return this;
            }

            public Builder SetExtension(string fileExtension)
            {
                _command.FileExtension = fileExtension;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _command.Description = description;
                return this;
            }

            public Builder SetAdditionalContent(string additionalContent)
            {
                _command.AdditionalContent = additionalContent;
                return this;
            }

            public Builder SetFolderId(Guid? folderId)
            {
                _command.FolderId = folderId;
                return this;
            }

            public Builder SetBase64Content(string base64Image)
            {
                _command.Base64Content = base64Image;
                return this;
            }

            public Builder SetLinkUrl(string linkUrl)
            {
                _command.LinkUrl = linkUrl;
                return this;
            }

            public Builder SetCssClass(string cssClass)
            {
                _command.CssClass = cssClass;
                return this;
            }

            public Builder SetOrder(int order)
            {
                _command.Order = order;
                return this;
            }

            public CreateMediaItemCommand Build()
            {
                return _command;
            }

        }
    }
}

using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string FileName { get; private set; }

        public string FileExtension { get; private set; }

        public string LinkUrl { get; private set; }

        public string Description { get; private set; }

        public string AdditionalContent { get; private set; }

        public string CssClass { get; private set; }

        public string Base64Content { get; private set; }

        public string SupportedFileTypes { get; private set; }

        public int Order { get; private set; }

        public class Builder
        {
            private readonly UpdateMediaItemCommand _command = new();

            public Builder SetSupportedFileTypes(string supportedFileTypes)
            {
                _command.SupportedFileTypes = supportedFileTypes;
                return this;
            }


            public Builder SetId(Guid id)
            {
                _command.Id = id;
                return this;
            }

            public Builder SetName(string name)
            {
                _command.Name = name;
                return this;
            }

            public Builder SetFileName(string fileName)
            {
                _command.FileName = fileName;
                return this;
            }

            public Builder SetExtention(string extention)
            {
                _command.FileExtension = extention;
                return this;
            }

            public Builder SetLinkUrl(string linkUrl)
            {
                _command.LinkUrl = linkUrl;
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

            public Builder SetCssClass(string cssClass)
            {
                _command.CssClass = cssClass;
                return this;
            }

            public Builder SetBase64Image(string base64Image)
            {
                _command.Base64Content = base64Image;
                return this;
            }

            public Builder SetOrder(int order)
            {
                _command.Order = order;
                return this;
            }

            public UpdateMediaItemCommand Build()
            {
                Validate();
                return _command;
            }

            private void Validate()
            {
                if (_command.Id == Guid.Empty)
                {
                    throw new InvalidOperationException("Id must be provided.");
                }

                if (string.IsNullOrWhiteSpace(_command.Name))
                {
                    throw new InvalidOperationException("Name must be provided.");
                }

                if (!string.IsNullOrWhiteSpace(_command.Base64Content) && string.IsNullOrWhiteSpace(_command.FileName))
                {
                    throw new InvalidOperationException("FileName must be provided.");
                }

                if (!string.IsNullOrWhiteSpace(_command.Base64Content) && string.IsNullOrWhiteSpace(_command.FileExtension))
                {
                    throw new InvalidOperationException("Extension must be provided.");
                }

                if (string.IsNullOrWhiteSpace(_command.Description))
                {
                    throw new InvalidOperationException("Description must be provided.");
                }

                if (_command.Order < 0)
                {
                    throw new InvalidOperationException("Order must be a non-negative integer.");
                }
            }
        }
    }
}

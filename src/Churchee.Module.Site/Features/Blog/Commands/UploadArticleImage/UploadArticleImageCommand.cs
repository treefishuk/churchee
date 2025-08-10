using Churchee.Module.Site.Features.Blog.Commands.UploadArticleImage;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UploadArticleImageCommand : IRequest<UploadArticleImageCommandResponse>
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string Description { get; set; }

        public string Base64Content { get; set; }

        public int? Width { get; set; }

        public class Builder
        {
            private readonly UploadArticleImageCommand _command = new();

            public Builder SetFileName(string fileName)
            {
                string dashedFileName = string.Join("-", fileName.Split(' ', StringSplitOptions.RemoveEmptyEntries));

                _command.FileName = dashedFileName;
                _command.Name = fileName.ToSentence();
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

            public Builder SetBase64Content(string base64Image)
            {
                _command.Base64Content = base64Image;
                return this;
            }

            public Builder SetWidth(int? width)
            {
                _command.Width = width;
                return this;
            }

            public UploadArticleImageCommand Build()
            {
                return _command;
            }

        }
    }
}
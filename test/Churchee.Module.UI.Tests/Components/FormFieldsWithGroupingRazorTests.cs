using Bunit;
using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using System.ComponentModel.DataAnnotations;
using FormFields = Churchee.Module.UI.Components.FormFields;
namespace Churchee.Module.UI.Tests.Components
{
    public class FormFieldsWithGroupingRazorTests : BasePageTests
    {

        [Fact]
        public void Renders_Text_Input()
        {
            // Arrange
            var inputModel = new BasicStringTestInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            var child = cut.FindComponent<RadzenTextBox>();

            child.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Text_Input_With_Display_Name()
        {
            // Arrange
            var inputModel = new StringWithNameAttributeTestInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            var formField = cut.FindComponent<RadzenFormField>();
            var input = cut.FindComponent<RadzenTextBox>();

            input.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            formField.Instance.Text.Should().Be("Custom Name");

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_TextWithSlug_When_DataAttribute_Set()
        {
            // Arrange
            var inputModel = new TextWithSlugInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            var input = cut.FindComponent<TextWithSlug>();

            input.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Hidden_When_Set()
        {
            // Arrange
            var inputModel = new HiddenInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.Markup.Contains("<input type=\"hidden\" name=\"MyProperty\" value=\"\" />").Should().BeTrue();

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Upload_When_Set()
        {
            // Arrange
            var inputModel = new UploadInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_ImageUpload_When_Set()
        {
            // Arrange
            var inputModel = new ImageUploadInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_MediaUpload_When_Set()
        {
            // Arrange
            var inputModel = new MediaUploadInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Password_When_Set()
        {
            // Arrange
            var inputModel = new PasswordInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenPassword>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Email_When_Set()
        {
            // Arrange
            var inputModel = new EmailInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_DateTime_When_Set()
        {
            // Arrange
            var inputModel = new DateTimeInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenDatePicker<DateTime>>().Count.Should().Be(1);

            var input = cut.FindComponent<RadzenDatePicker<DateTime>>();

            input.Instance.DateFormat.Should().Be("dd-MM-yyyy HH:mm");

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_Date_When_Set()
        {
            // Arrange
            var inputModel = new DateInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenDatePicker<DateTime>>().Count.Should().Be(1);

            var input = cut.FindComponent<RadzenDatePicker<DateTime>>();

            input.Instance.DateFormat.Should().Be("dd-MM-yyyy");

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_CheckboxList_When_Set()
        {
            // Arrange
            var inputModel = new CheckboxListInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenCheckBoxList<Guid>>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }


        [Fact]
        public void Renders_Url_When_Set()
        {
            // Arrange
            var inputModel = new UrlInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }

        [Fact]
        public void Renders_ReadOnly_When_Set()
        {
            // Arrange
            var inputModel = new ReadOnlyInputModel();

            // Act
            var cut = GenrateClassUnderTest(inputModel);

            // Assert
            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            var input = cut.FindComponent<RadzenTextBox>();

            input.Instance.ReadOnly.Should().BeTrue();

            cut.Markup.Contains("<div class=\"rz-card\"").Should().BeTrue();
        }


        private IRenderedComponent<EditForm> GenrateClassUnderTest(object inputModel)
        {
            var cut = Render<EditForm>(parameters => parameters
                .Add(p => p.Model, inputModel)
                .Add(p => p.ChildContent, childParams =>
                    builder =>
                    {
                        builder.OpenComponent<FormFields>(0);
                        builder.AddAttribute(1, "InputModel", inputModel);
                        builder.AddAttribute(2, "EditContext", new EditContext(inputModel));
                        builder.AddAttribute(3, "Properties", inputModel.GetType().GetProperties());
                        builder.CloseComponent();
                    })
            );

            return cut;
        }





        private class BasicStringTestInputModel
        {
            public BasicStringTestInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            public string MyProperty { get; set; }
        }

        private class StringWithNameAttributeTestInputModel
        {
            public StringWithNameAttributeTestInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(Name = "Custom Name", GroupName = "Group Name")]
            public string MyProperty { get; set; }
        }

        private class TextWithSlugInputModel
        {
            public TextWithSlugInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(Name = "Custom Name", GroupName = "Group Name")]
            [DataType(DataTypes.TextWithSlug)]
            public string MyProperty { get; set; }
        }

        private class HiddenInputModel
        {
            public HiddenInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Hidden)]
            public string MyProperty { get; set; }
        }

        private class PasswordInputModel
        {
            public PasswordInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Password)]
            public string MyProperty { get; set; }
        }

        private class EmailInputModel
        {
            public EmailInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.EmailAddress)]
            public string MyProperty { get; set; }
        }

        private class UploadInputModel
        {
            public UploadInputModel()
            {
                Upload = new Upload();
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Upload)]
            public Upload Upload { get; set; }
        }

        private class ImageUploadInputModel
        {
            public ImageUploadInputModel()
            {
                Upload = new Upload();
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.ImageUpload)]
            public Upload Upload { get; set; }
        }

        private class MediaUploadInputModel
        {
            public MediaUploadInputModel()
            {
                Upload = new Upload();
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.MediaUpload)]
            public Upload Upload { get; set; }
        }

        private class CheckboxListInputModel
        {
            public CheckboxListInputModel()
            {
                MultiSelect = new MultiSelect(new List<MultiSelectItem>());
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.CheckboxList)]
            public MultiSelect MultiSelect { get; set; }
        }


        private class DateTimeInputModel
        {
            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.DateTime)]
            public DateTime Date { get; set; }
        }

        private class DateInputModel
        {
            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Date)]
            public DateTime Date { get; set; }
        }

        private class UrlInputModel
        {
            public UrlInputModel()
            {
                Value = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Url)]
            public string Value { get; set; }
        }

        private class ReadOnlyInputModel
        {
            public ReadOnlyInputModel()
            {
                Value = string.Empty;
            }

            [Display(GroupName = "Group Name")]
            [DataType(DataTypes.Readonly)]
            public string Value { get; set; }
        }
    }
}

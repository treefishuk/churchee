using Bunit;
using Churchee.Common.ValueTypes;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using System.ComponentModel.DataAnnotations;
using FormFields = Churchee.Module.UI.Components.FormFields;
namespace Churchee.Module.UI.Tests.Components
{
    public class FormFieldsRazorTests : BasePageTests
    {

        [Fact]
        public void Renders_Text_Input()
        {
            // Arrange
            var inputModel = new BasicStringTestInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            var child = cut.FindComponent<RadzenTextBox>();

            var input = child.Find("input");

            input.Change("Hello world");

            child.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            Changed.Should().BeTrue();
        }

        [Fact]
        public void Renders_Text_Input_With_Display_Name()
        {
            // Arrange
            var inputModel = new StringWithNameAttributeTestInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            var formField = cut.FindComponent<RadzenFormField>();
            var textBox = cut.FindComponent<RadzenTextBox>();

            var input = textBox.Find("input");

            input.Change("Hello world");

            cut.FindComponents<RadzenFormField>().Count.Should().Be(1);
            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            textBox.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            formField.Instance.Text.Should().Be("Custom Name");

            Changed.Should().BeTrue();

        }

        [Fact]
        public void Renders_TextWithSlug_When_DataAttribute_Set()
        {
            // Arrange
            var inputModel = new TextWithSlugInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            var textBox = cut.FindComponent<TextWithSlug>();

            var input = textBox.Find("input");

            input.Change("Hello world");

            cut.FindComponents<TextWithSlug>().Count.Should().Be(1);

            textBox.Instance.Name.Should().Be(nameof(inputModel.MyProperty));

            Changed.Should().BeTrue();
        }

        [Fact]
        public void Renders_Hidden_When_Set()
        {
            // Arrange
            var inputModel = new HiddenInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.Markup.Contains("<input type=\"hidden\" name=\"MyProperty\" value=\"\" />").Should().BeTrue();
        }

        [Fact]
        public void Renders_Upload_When_Set()
        {
            // Arrange
            var inputModel = new UploadInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);
        }

        [Fact]
        public void Renders_ImageUpload_When_Set()
        {
            // Arrange
            var inputModel = new ImageUploadInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);
        }

        [Fact]
        public void Renders_MediaUpload_When_Set()
        {
            // Arrange
            var inputModel = new MediaUploadInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenFileInput<string>>().Count.Should().Be(1);
        }

        [Fact]
        public void Renders_Password_When_Set()
        {
            // Arrange
            var inputModel = new PasswordInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenPassword>().Count.Should().Be(1);

            var field = cut.FindComponent<RadzenPassword>();

            var input = field.Find("input");

            input.Change("Helloworld22!");

            Changed.Should().BeTrue();

        }

        [Fact]
        public void Renders_Email_When_Set()
        {
            // Arrange
            var inputModel = new EmailInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            var field = cut.FindComponent<RadzenTextBox>();

            var input = field.Find("input");

            input.Change("norepy@example.com");

            Changed.Should().BeTrue();
        }

        [Fact]
        public void Renders_DateTime_When_Set()
        {
            // Arrange
            var inputModel = new DateTimeInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenDatePicker<DateTime>>().Count.Should().Be(1);

            var field = cut.FindComponent<RadzenDatePicker<DateTime>>();

            field.Instance.DateFormat.Should().Be("dd-MM-yyyy HH:mm");



        }

        [Fact]
        public void Renders_Date_When_Set()
        {
            // Arrange
            var inputModel = new DateInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenDatePicker<DateTime>>().Count.Should().Be(1);

            var input = cut.FindComponent<RadzenDatePicker<DateTime>>();

            input.Instance.DateFormat.Should().Be("dd-MM-yyyy");
        }

        [Fact]
        public void Renders_CheckboxList_When_Set()
        {
            // Arrange
            var inputModel = new CheckboxListInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenCheckBoxList<Guid>>().Count.Should().Be(1);
        }


        [Fact]
        public void Renders_Url_When_Set()
        {
            // Arrange
            var inputModel = new UrlInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

        }

        [Fact]
        public void Renders_ReadOnly_When_Set()
        {
            // Arrange
            var inputModel = new ReadOnlyInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenTextBox>().Count.Should().Be(1);

            var input = cut.FindComponent<RadzenTextBox>();

            input.Instance.ReadOnly.Should().BeTrue();

        }

        [Fact]
        public void Renders_Int_When_Set()
        {
            // Arrange
            var inputModel = new IntegerTestInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenNumeric<int>>().Count.Should().Be(1);

        }

        [Fact]
        public void Renders_NullableInt_When_Set()
        {
            // Arrange
            var inputModel = new NullableIntegerTestInputModel();

            var cut = GenrateClassUnderTest(inputModel);

            cut.FindComponents<RadzenNumeric<int?>>().Count.Should().Be(1);

        }

        public bool Changed { get; set; }

        private void InputChanged()
        {
            Changed = true;
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
                        builder.AddAttribute(4, "OnValueChanged", EventCallback.Factory.Create<object>(this, InputChanged));
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

            public string MyProperty { get; set; }
        }

        private class StringWithNameAttributeTestInputModel
        {
            public StringWithNameAttributeTestInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(Name = "Custom Name")]
            public string MyProperty { get; set; }
        }

        private class TextWithSlugInputModel
        {
            public TextWithSlugInputModel()
            {
                MyProperty = string.Empty;
            }

            [Display(Name = "Custom Name")]
            [DataType(DataTypes.TextWithSlug)]
            public string MyProperty { get; set; }
        }

        private class HiddenInputModel
        {
            public HiddenInputModel()
            {
                MyProperty = string.Empty;
            }

            [DataType(DataTypes.Hidden)]
            public string MyProperty { get; set; }
        }

        private class PasswordInputModel
        {
            public PasswordInputModel()
            {
                MyProperty = string.Empty;
            }

            [DataType(DataTypes.Password)]
            public string MyProperty { get; set; }
        }

        private class EmailInputModel
        {
            public EmailInputModel()
            {
                MyProperty = string.Empty;
            }

            [DataType(DataTypes.EmailAddress)]
            public string MyProperty { get; set; }
        }

        private class UploadInputModel
        {
            public UploadInputModel()
            {
                Upload = new Upload();
            }

            [DataType(DataTypes.Upload)]
            public Upload Upload { get; set; }
        }

        private class ImageUploadInputModel
        {
            public ImageUploadInputModel()
            {
                Upload = new Upload();
            }

            [DataType(DataTypes.ImageUpload)]
            public Upload Upload { get; set; }
        }

        private class MediaUploadInputModel
        {
            public MediaUploadInputModel()
            {
                Upload = new Upload();
            }

            [DataType(DataTypes.MediaUpload)]
            public Upload Upload { get; set; }
        }

        private class CheckboxListInputModel
        {
            public CheckboxListInputModel()
            {
                MultiSelect = new MultiSelect(new List<MultiSelectItem>());
            }

            [DataType(DataTypes.CheckboxList)]
            public MultiSelect MultiSelect { get; set; }
        }


        private class DateTimeInputModel
        {
            [DataType(DataTypes.DateTime)]
            public DateTime Date { get; set; }
        }

        private class DateInputModel
        {
            [DataType(DataTypes.Date)]
            public DateTime Date { get; set; }
        }

        private class UrlInputModel
        {
            public UrlInputModel()
            {
                Value = string.Empty;
            }

            [DataType(DataTypes.Url)]
            public string Value { get; set; }
        }

        private class ReadOnlyInputModel
        {
            public ReadOnlyInputModel()
            {
                Value = string.Empty;
            }

            [DataType(DataTypes.Readonly)]
            public string Value { get; set; }
        }

        private class IntegerTestInputModel
        {
            public IntegerTestInputModel()
            {
                Value = 1;
            }

            public int Value { get; set; }
        }

        private class NullableIntegerTestInputModel
        {
            public NullableIntegerTestInputModel()
            {
                Value = 1;
            }

            public int? Value { get; set; }
        }
    }
}

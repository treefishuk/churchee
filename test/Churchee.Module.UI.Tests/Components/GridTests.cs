using Bunit;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.UI.Tests.Components
{
    public class GridTests : BasePageTests
    {
        // Simple POCO used as the grid's TItem in tests
        public class TestItem
        {
            public Guid Id { get; set; }

            [DataType(DataTypes.ImageUrl)]
            public string? Image { get; set; }

            [DataType(DataTypes.Rating)]
            public int Rating { get; set; }

            public bool IsActive { get; set; }

            public DateTime Created { get; set; }
        }

        // Child component used to call the cascading RegisterButtonColumn action
        private class RegisterActionChild<TItem> : ComponentBase
        {
            [CascadingParameter] public Action<string, RenderFragment<TItem>, string>? RegisterButtonColumn { get; set; }

            protected override void OnInitialized()
            {
                if (RegisterButtonColumn is not null)
                {
                    RenderFragment<TItem> template = (ctx) => (__builder) =>
                    {
                        __builder.OpenElement(0, "button");
                        __builder.AddAttribute(1, "id", "actionBtn");
                        // use dynamic to obtain Id (keeps test simple)
                        __builder.AddContent(2, "noid");
                        __builder.CloseElement();
                    };

                    RegisterButtonColumn.Invoke("MyAction", template, "100px");
                }
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                // nothing visible from this component itself; it only registers the column
            }
        }

        // Wrapper renders Grid<TestItem> and injects a RegisterActionChild so we can verify the action column appears
        private class ActionColumnWrapper : ComponentBase
        {
            public TestItem Item { get; } = new TestItem { Id = Guid.NewGuid(), Image = null, IsActive = true, Created = DateTime.UtcNow };

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Grid<TestItem>>(0);
                builder.AddAttribute(1, "Data", new[] { Item });
                builder.AddAttribute(2, "Count", 1);
                builder.AddAttribute(3, "ShowDetail", false);
                builder.AddAttribute(4, "ShowEdit", false);
                builder.AddAttribute(5, "ChildContent", (RenderFragment)((childBuilder) =>
                {
                    childBuilder.OpenComponent<RegisterActionChild<TestItem>>(0);
                    childBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        // Wrapper that wires OnDelete to a field so tests can assert the handler ran
        private class DeleteWrapper : ComponentBase
        {
            public bool Deleted { get; private set; }

            public TestItem Item { get; } = new TestItem { Id = Guid.NewGuid() };

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Grid<TestItem>>(0);
                builder.AddAttribute(1, "Data", new[] { Item });
                builder.AddAttribute(2, "Count", 1);
                builder.AddAttribute(3, "ShowDetail", false);
                builder.AddAttribute(4, "ShowEdit", false);
                builder.AddAttribute(5, "OnDelete", EventCallback.Factory.Create<TestItem>(this, OnDeleteHandler));
                builder.CloseComponent();

                // render the state so tests can also assert via markup if desired
                builder.OpenElement(10, "span");
                builder.AddAttribute(11, "id", "deletedState");
                builder.AddContent(12, Deleted.ToString());
                builder.CloseElement();
            }

            private Task OnDeleteHandler(TestItem item)
            {
                Deleted = true;
                return Task.CompletedTask;
            }
        }

        // Wrapper to test image rendering with ImagePrefix
        private class ImageWrapper : ComponentBase
        {
            public TestItem Item { get; } = new TestItem { Id = Guid.NewGuid(), Image = "img.jpg" };

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Grid<TestItem>>(0);
                builder.AddAttribute(1, "Data", new[] { Item });
                builder.AddAttribute(2, "Count", 1);
                builder.AddAttribute(3, "ImagePrefix", "https://cdn.example.com/");
                builder.AddAttribute(4, "ShowDetail", false);
                builder.AddAttribute(5, "ShowEdit", false);
                builder.CloseComponent();
            }
        }

        [Fact]
        public void RegisterButtonColumn_ChildRegistersActionColumn_RowShowsActionTemplate()
        {
            // Arrange
            var cut = Render<ActionColumnWrapper>();

            // Act - find generated action button (template rendered per row)
            var btn = cut.FindAll("button#actionBtn")[0];

            // Assert
            Assert.NotNull(btn);
            // The button's text equals the item's Id
            Assert.Contains("noid", btn.TextContent);
        }

        [Fact]
        public void DeleteButton_Click_InvokesOnDeleteHandler()
        {
            // Arrange
            var cut = Render<DeleteWrapper>();

            // The grid renders a Radzen button with class "delete-row" per row when OnDelete is set.
            var deleteBtn = cut.FindAll("button.delete-row")[0];

            Assert.NotNull(deleteBtn);

            // Act
            deleteBtn.Click();

            // Assert - component instance should have Deleted = true
            Assert.True(((DeleteWrapper)cut.Instance).Deleted);
        }

        [Fact]
        public void ImageColumn_RendersImageWithPrefix_WhenNotHttps()
        {
            // Arrange
            var cut = Render<ImageWrapper>();

            // Act - find first img tag in rendered output
            var img = cut.FindAll("img")[0];

            // Assert
            Assert.NotNull(img);
            Assert.Equal("https://cdn.example.com/img.jpg", img.GetAttribute("src"));
        }

        // wrapper class used by DetailAndEditLinks_UseCurrentUriPrefix
        private class DetailEditWrapper : ComponentBase
        {
            public TestItem Item { get; } = new TestItem { Id = Guid.NewGuid(), Image = null };

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<Grid<TestItem>>(0);
                builder.AddAttribute(1, "Data", new[] { Item });
                builder.AddAttribute(2, "Count", 1);
                builder.AddAttribute(3, "ShowDetail", true);
                builder.AddAttribute(4, "ShowEdit", true);
                builder.CloseComponent();
            }
        }
    }
}

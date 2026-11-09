using System.Reflection;
using AngleSharp.Html.Parser;
using Bunit;
using Churchee.Module.UI.Components;
using Churchee.Test.Helpers.Blazor;
using Microsoft.AspNetCore.Components;

namespace Churchee.Module.UI.Tests.Components
{
    public class HtmlEditorTests : BasePageTests
    {
        [Fact]
        public void CleanClipboardHtml_RemovesUnsafeAttributes_AndUnwrapsInlineContainers()
        {
            var cut = Render<HtmlEditor>();
            var method = typeof(HtmlEditor).GetMethod("CleanClipboardHtml", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);

            var input = "<div style=\"color:red\" class=\"foo\" align=\"center\"><span id=\"a\" style=\"font-weight:bold\">Hello</span> <font face=\"Arial\">world</font><div>!</div></div>";
            var result = (string)method.Invoke(cut.Instance, new object[] { input })!;

            var document = new HtmlParser().ParseDocument(result);
            var text = document.Body?.TextContent.Replace("\r", " ").Replace("\n", " ").Replace("  ", " ").Trim();

            Assert.NotNull(text);
            Assert.Contains("Hello", text);
            Assert.Contains("world", text);
            Assert.Contains("!", text);
            Assert.DoesNotContain("style", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("class", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("align", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("face", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("size", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("id", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task OnValueChanged_UpdatesValue_AndInvokesCallback()
        {
            string? callbackValue = null;

            var cut = Render<HtmlEditor>(parameters => parameters
                .Add(p => p.Value, "<p>initial</p>")
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<string?>(this, value => callbackValue = value)));

            var method = typeof(HtmlEditor).GetMethod("OnValueChanged", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.NotNull(method);

            var task = (Task)method.Invoke(cut.Instance, new object[] { "<p>updated</p>" })!;
            await task;

            Assert.Equal("<p>updated</p>", cut.Instance.Value);
            Assert.Equal("<p>updated</p>", callbackValue);
        }

        [Fact]
        public void Render_UsesConfiguredHeight_AndChildContent()
        {
            var cut = Render<HtmlEditor>(parameters => parameters
                .Add(p => p.Height, 325)
                .AddChildContent("<button class=\"custom-tool\">Custom</button>"));

            Assert.Equal(325, cut.Instance.Height);
            Assert.NotNull(cut.Instance.ChildContent);
        }
    }
}

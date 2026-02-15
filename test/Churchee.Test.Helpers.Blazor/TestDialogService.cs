using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Churchee.Test.Helpers.Blazor
{
    public class TestDialogService : DialogService
    {
        public TestDialogService(NavigationManager uriHelper, IJSRuntime jsRuntime) : base(uriHelper, jsRuntime)
        {
            OnClose += MarkAsClosed;
        }

        public string LastTitle { get; private set; } = string.Empty;

        public int OpenCalls { get; private set; }

        public bool Closed { get; private set; } = true;

        private void MarkAsClosed(dynamic dynamic)
        {
            Closed = true;
        }

        // Overload that accepts a component type and parameters
        public override Task<dynamic> OpenAsync<TComponent>(string title, Dictionary<string, object?>? parameters = null, DialogOptions? options = null)
        {
            LastTitle = title;
            OpenCalls++;
            Closed = false;
            return base.OpenAsync<TComponent>(title, parameters, options);
        }

        public List<object> Dialogs => dialogs;

        public override Task<dynamic> OpenAsync(string title, Type componentType, Dictionary<string, object?>? parameters = null, DialogOptions? options = null)
        {
            LastTitle = title;
            OpenCalls++;
            Closed = false;
            return base.OpenAsync(title, componentType, parameters, options);
        }


        public override Task<dynamic> OpenAsync(string title, RenderFragment<DialogService> childContent, DialogOptions? options = null, CancellationToken? cancellationToken = null)
        {
            LastTitle = title;
            OpenCalls++;
            Closed = false;
            return base.OpenAsync(title, childContent, options, cancellationToken);
        }

        public override Task<dynamic> OpenAsync(RenderFragment<DialogService> titleContent, RenderFragment<DialogService> childContent, DialogOptions? options = null, CancellationToken? cancellationToken = null)
        {
            OpenCalls++;
            Closed = false;
            return base.OpenAsync(titleContent, childContent, options, cancellationToken);
        }
    }
}

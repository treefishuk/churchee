namespace Churchee.Module.Hangfire.Tests.Services
{
    public static class DummyJobHelper
    {
        public static Task DummyTaskMethod()
        {
            return Task.CompletedTask;
        }
    }
}

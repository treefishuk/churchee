using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class DateTimeValidationTester : ValidationTester<DateTime>
    {
        private readonly DateTime _instance;

        public DateTimeValidationTester(DateTime instance) : base(instance)
        {
            _instance = instance;
        }

        public void BeCloseTo(DateTime dateTime, TimeSpan timeSpan)
        {
            var min = dateTime - timeSpan;
            var max = dateTime + timeSpan;

            Assert.InRange(_instance, min, max);
        }
    }
}

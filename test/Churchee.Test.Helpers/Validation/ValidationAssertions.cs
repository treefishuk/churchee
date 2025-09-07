namespace Churchee.Test.Helpers.Validation
{
    public static class ValidationAssertions
    {
        public static ValidationTester<T> Should<T>(this T instance)
        {
            return new ValidationTester<T>(instance);
        }

        public static BoolValidationTester Should(this bool instance)
        {
            return new BoolValidationTester(instance);
        }

        public static EnumerableValidationTester<T> Should<T>(this IEnumerable<T> instance)
        {
            return new EnumerableValidationTester<T>(instance);
        }

        public static StringValidationTester Should(this string instance)
        {
            return new StringValidationTester(instance);
        }

        public static IntegerValidationTester Should(this int instance)
        {
            return new IntegerValidationTester(instance);
        }

        public static DateTimeValidationTester Should(this DateTime instance)
        {
            return new DateTimeValidationTester(instance);
        }

        public static ListValidationTester<T> Should<T>(this List<T> instance)
        {
            return new ListValidationTester<T>(instance);
        }

        public static AsyncMethodValidationTester Should(this Func<Task> instance)
        {
            return new AsyncMethodValidationTester(instance);
        }
    }
}

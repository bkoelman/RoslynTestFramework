using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace RoslynTestFramework
{
    /// <summary>
    /// Member precondition checks.
    /// </summary>
    internal static class FrameworkGuard
    {
        [AssertionMethod]
        [ContractAnnotation("value: null => halt")]
        public static void NotNull<T>([CanBeNull] [NoEnumeration] T value, [NotNull] [InvokerParameterName] string name)
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(name);
            }
        }

        [AssertionMethod]
        [ContractAnnotation("value: null => halt")]
        public static void NotNullNorWhiteSpace([CanBeNull] string value, [NotNull] [InvokerParameterName] string name)
        {
            NotNull(value, name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"'{name}' cannot be empty or contain only whitespace.", name);
            }
        }
    }
}

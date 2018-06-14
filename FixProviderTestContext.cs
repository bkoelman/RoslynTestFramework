using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace RoslynTestFramework
{
    public sealed class FixProviderTestContext
    {
        [NotNull]
        public AnalyzerTestContext AnalyzerTestContext { get; }

        [NotNull]
        [ItemNotNull]
        public ImmutableList<string> ExpectedCode { get; }

        public TextComparisonMode CodeComparisonMode { get; }

        public FixProviderTestContext([NotNull] AnalyzerTestContext analyzerTestContext,
            [NotNull] [ItemNotNull] IEnumerable<string> expectedCode, TextComparisonMode codeComparisonMode)
        {
            FrameworkGuard.NotNull(analyzerTestContext, nameof(analyzerTestContext));
            FrameworkGuard.NotNull(expectedCode, nameof(expectedCode));

            AnalyzerTestContext = analyzerTestContext;
            ExpectedCode = ImmutableList.CreateRange(expectedCode);
            CodeComparisonMode = codeComparisonMode;
        }

        [NotNull]
        public FixProviderTestContext WithExpectedCode([NotNull] [ItemNotNull] IEnumerable<string> expectedCode)
        {
            FrameworkGuard.NotNull(expectedCode, nameof(expectedCode));

            return new FixProviderTestContext(AnalyzerTestContext, expectedCode, CodeComparisonMode);
        }
    }
}

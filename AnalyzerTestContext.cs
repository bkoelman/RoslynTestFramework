using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

// ReSharper disable once CheckNamespace
namespace RoslynTestFramework
{
    public sealed class AnalyzerTestContext
    {
        private const string DefaultFileName = "TestDocument";
        private const string DefaultAssemblyName = "TestProject";
        private const DocumentationMode DefaultDocumentationMode = DocumentationMode.None;
        private const OutputKind DefaultOutputKind = OutputKind.DynamicallyLinkedLibrary;
        private const TestValidationMode DefaultTestValidationMode = TestValidationMode.AllowCompileWarnings;

        [NotNull]
        [ItemNotNull]
        private static readonly Lazy<ImmutableHashSet<MetadataReference>> DefaultReferencesLazy =
            new Lazy<ImmutableHashSet<MetadataReference>>(ResolveDefaultReferences, LazyThreadSafetyMode.PublicationOnly);

        [NotNull]
        [ItemNotNull]
        private static ImmutableHashSet<MetadataReference> ResolveDefaultReferences()
        {
            string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            if (assemblyPath == null)
            {
                throw new InvalidOperationException("Failed to locate assembly for System.Object.");
            }

            return ImmutableHashSet.Create(new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
            });
        }

        [NotNull]
        public string SourceCode { get; }

        [NotNull]
        public IList<TextSpan> SourceSpans { get; }

        [NotNull]
        public string LanguageName { get; }

        [NotNull]
        public string FileName { get; }

        [NotNull]
        public string AssemblyName { get; }

        [NotNull]
        [ItemNotNull]
        public ImmutableHashSet<MetadataReference> References { get; }

        public DocumentationMode DocumentationMode { get; }

        public OutputKind OutputKind { get; }

        [CanBeNull]
        public int? CompilerWarningLevel { get; }

        public TreatWarningsAsErrors WarningsAsErrors { get; }

        public TestValidationMode ValidationMode { get; }

        public DiagnosticsCaptureMode DiagnosticsCaptureMode { get; }

        [NotNull]
        public AnalyzerOptions Options { get; }

#pragma warning disable AV1561 // Signature contains more than 3 parameters
#pragma warning disable AV1500 // Member or local function contains more than 7 statements
        private AnalyzerTestContext([NotNull] string sourceCode, [NotNull] IList<TextSpan> sourceSpans,
            [NotNull] string languageName, [NotNull] string fileName, [NotNull] string assemblyName,
            [NotNull] [ItemNotNull] ImmutableHashSet<MetadataReference> references, DocumentationMode documentationMode,
            OutputKind outputKind, [CanBeNull] int? compilerWarningLevel, TreatWarningsAsErrors warningsAsErrors,
            TestValidationMode validationMode,
            DiagnosticsCaptureMode diagnosticsCaptureMode, [NotNull] AnalyzerOptions options)
        {
            SourceCode = sourceCode;
            SourceSpans = sourceSpans;
            LanguageName = languageName;
            FileName = fileName;
            AssemblyName = assemblyName;
            References = references;
            DocumentationMode = documentationMode;
            OutputKind = outputKind;
            CompilerWarningLevel = compilerWarningLevel;
            WarningsAsErrors = warningsAsErrors;
            ValidationMode = validationMode;
            DiagnosticsCaptureMode = diagnosticsCaptureMode;
            Options = options;
        }
#pragma warning restore AV1500 // Member or local function contains more than 7 statements
#pragma warning restore AV1561 // Signature contains more than 3 parameters

#pragma warning disable AV1561 // Signature contains more than 3 parameters
        public AnalyzerTestContext([NotNull] string sourceCode, [NotNull] IList<TextSpan> sourceSpans,
            [NotNull] string languageName, [NotNull] AnalyzerOptions options)
            : this(sourceCode, sourceSpans, languageName, DefaultFileName, DefaultAssemblyName, DefaultReferencesLazy.Value,
                DefaultDocumentationMode, DefaultOutputKind, null, TreatWarningsAsErrors.None, DefaultTestValidationMode,
                DiagnosticsCaptureMode.RequireInSourceTree, options)
        {
            FrameworkGuard.NotNull(sourceCode, nameof(sourceCode));
            FrameworkGuard.NotNull(sourceSpans, nameof(sourceSpans));
            FrameworkGuard.NotNullNorWhiteSpace(languageName, nameof(languageName));
            FrameworkGuard.NotNull(options, nameof(options));
        }
#pragma warning restore AV1561 // Signature contains more than 3 parameters

        [NotNull]
        public AnalyzerTestContext WithCode([NotNull] string sourceCode, [NotNull] IList<TextSpan> sourceSpans)
        {
            FrameworkGuard.NotNull(sourceCode, nameof(sourceCode));
            FrameworkGuard.NotNull(sourceSpans, nameof(sourceSpans));

            return new AnalyzerTestContext(sourceCode, sourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext InFileNamed([NotNull] string fileName)
        {
            FrameworkGuard.NotNullNorWhiteSpace(fileName, nameof(fileName));

            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, fileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext InAssemblyNamed([NotNull] string assemblyName)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, assemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext WithReferences([NotNull] [ItemNotNull] IEnumerable<MetadataReference> references)
        {
            FrameworkGuard.NotNull(references, nameof(references));

            ImmutableList<MetadataReference> referenceList = ImmutableList.CreateRange(references);

            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName,
                referenceList.ToImmutableHashSet(), DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors,
                ValidationMode, DiagnosticsCaptureMode, Options);
        }

        [NotNull]
        public AnalyzerTestContext WithDocumentationMode(DocumentationMode mode)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References, mode,
                OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode, Options);
        }

        [NotNull]
        public AnalyzerTestContext WithOutputKind(OutputKind outputKind)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, outputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext CompileAtWarningLevel(int warningLevel)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, warningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode, Options);
        }

        [NotNull]
        public AnalyzerTestContext CompileWithWarningsAsErrors(TreatWarningsAsErrors warningsAsErrors)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, warningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext InValidationMode(TestValidationMode validationMode)
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, validationMode, DiagnosticsCaptureMode,
                Options);
        }

        [NotNull]
        public AnalyzerTestContext AllowingDiagnosticsOutsideSourceTree()
        {
            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode,
                DiagnosticsCaptureMode.AllowOutsideSourceTree, Options);
        }

        [NotNull]
        public AnalyzerTestContext WithOptions([NotNull] AnalyzerOptions options)
        {
            FrameworkGuard.NotNull(options, nameof(options));

            return new AnalyzerTestContext(SourceCode, SourceSpans, LanguageName, FileName, AssemblyName, References,
                DocumentationMode, OutputKind, CompilerWarningLevel, WarningsAsErrors, ValidationMode, DiagnosticsCaptureMode,
                options);
        }
    }
}

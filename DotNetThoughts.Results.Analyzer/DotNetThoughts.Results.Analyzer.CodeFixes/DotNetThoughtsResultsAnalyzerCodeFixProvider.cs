using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;

namespace DotNetThoughts.Results.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DotNetThoughtsResultsAnalyzerCodeFixProvider)), Shared]
    public class DotNetThoughtsResultsAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DotNetThoughtsResultsAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            return Task.CompletedTask;
        }
    }
}

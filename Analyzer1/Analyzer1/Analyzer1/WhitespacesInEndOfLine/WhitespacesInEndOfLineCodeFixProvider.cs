using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;

namespace PartA
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhitespacesInEndOfLineCodeFixProvider)), Shared]
    public class WhitespacesInEndOfLineCodeFixProvider: CodeFixProvider
    {
        private const string title = "Remove whitespaces in the end of line";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(WhitespacesInEndOfLineAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)/*.ConfigureAwait(false)*/;
            var diagnosticSpan = context.Diagnostics.First().Location.SourceSpan;

            var whitespacesInEndOfLine = root.DescendantTrivia(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(title, c => RemoveWhitespaces(context, whitespacesInEndOfLine)),
                context.Diagnostics);
        }

        private async Task<Document> RemoveWhitespaces(CodeFixContext context, IEnumerable<SyntaxTrivia> whitespaces)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            SyntaxNode newRoot = root.ReplaceTrivia(whitespaces, (x, y) => default(SyntaxTrivia));
            return context.Document.WithSyntaxRoot(newRoot);
        }
    }
}

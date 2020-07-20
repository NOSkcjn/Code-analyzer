using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace PartA
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LineConsistsInWhitespacesCodeFixProvider)), Shared]
    public class LineConsistsInWhitespacesCodeFixProvider : CodeFixProvider
    {
        private const string title = "Remove whitespaces";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LineConsistsInWhitespacesAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnosticSpan = context.Diagnostics.First().Location.SourceSpan;

            var emptyLineTrivia = root.DescendantTrivia(diagnosticSpan);

            context.RegisterCodeFix(
                CodeAction.Create(title, c => RemoveWhiteLine(context, emptyLineTrivia)),
                context.Diagnostics);
        }

        private async Task<Document> RemoveWhiteLine(CodeFixContext context, IEnumerable<SyntaxTrivia> whitespaces)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            SyntaxNode newRoot = root.ReplaceTrivia(whitespaces, (x, y) => default(SyntaxTrivia));

            return context.Document.WithSyntaxRoot(newRoot);
        }
    }
}

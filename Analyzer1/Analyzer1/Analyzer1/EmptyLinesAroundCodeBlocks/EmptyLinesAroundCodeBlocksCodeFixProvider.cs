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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyLinesAroundCodeBlocksCodeFixProvider)), Shared]
    public class EmptyLinesAroundCodeBlocksCodeFixProvider : CodeFixProvider
    {
        private const string title = "Remove empty lines around block code";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(EmptyLinesAroundCodeBlocksAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            context.RegisterCodeFix(
                CodeAction.Create(title, c => RemoveWhiteLine(context)),
                context.Diagnostics);
        }

        private async Task<Document> RemoveWhiteLine(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            List<SyntaxTrivia> triviasForRemove = new List<SyntaxTrivia>();
            foreach (var node in root.DescendantNodesAndSelf())
            {
                var tokens = node.DescendantTokens();
                SyntaxToken prevToken = default(SyntaxToken);
                foreach (var token in tokens)
                {
                    if (prevToken.Equals(default(SyntaxToken)))
                        prevToken = token;

                    var trivias = token.LeadingTrivia;

                    foreach (var trivia in trivias)
                    {
                        if (token.IsKind(SyntaxKind.CloseBraceToken) ||
                            (prevToken.IsKind(SyntaxKind.OpenBraceToken) && !token.IsKind(SyntaxKind.OpenBraceToken)
                            && !token.IsKind(SyntaxKind.CloseBraceToken)))
                        {
                            if (trivia.Kind().Equals(SyntaxKind.EndOfLineTrivia))
                            {
                                triviasForRemove.Add(trivia);
                            }
                        }
                    }
                    prevToken = token;
                }
            }

            SyntaxNode newRoot = root.ReplaceTrivia(triviasForRemove, (x, y) => default(SyntaxTrivia));
            return context.Document.WithSyntaxRoot(newRoot);
        }
    }
}

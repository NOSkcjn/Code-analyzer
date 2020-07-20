using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartA
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyLinesCodeFixProvider)), Shared]
    public class EmptyLinesCodeFixProvider : CodeFixProvider
    {
        private const string title = "Remove empty lines";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(EmptyLinesAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            context.RegisterCodeFix(
                CodeAction.Create(title, c => RemoveWhiteLine(context), title),
                context.Diagnostics);
        }

        private async Task<Document> RemoveWhiteLine(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync();
            List<SyntaxTrivia> triviasForRemove = new List<SyntaxTrivia>();
            List<Location> locationsWas = new List<Location>();
            foreach (var node in root.DescendantNodesAndSelf())
            {
                var tokens = node.DescendantTokens().ToList();
                for (int i = 0; i < tokens.Count(); i++)
                {
                    int current = 0;
                    var trivias = tokens[i].LeadingTrivia;

                    foreach (var trivia in trivias)
                    {
                        if (trivia.Kind().Equals(SyntaxKind.EndOfLineTrivia))
                        {
                            current++;
                            if (current > 1)
                            {
                                if ((current < trivias.Count()) ||
                                    (tokens[i].Kind().Equals(SyntaxKind.CloseBraceToken) &&
                                    i < tokens.Count() - 1 && tokens[i + 1].Kind().Equals(SyntaxKind.EndOfFileToken)))
                                {
                                    var location = trivia.GetLocation();
                                    if (!locationsWas.Contains(location))
                                    {
                                        triviasForRemove.Add(trivia);
                                    }
                                }
                            }
                        }
                        else if (!trivia.Kind().Equals(SyntaxKind.WhitespaceTrivia))
                            break;
                    }
                }
            }

            SyntaxNode newRoot = root.ReplaceTrivia(triviasForRemove, (x, y) => default(SyntaxTrivia));
            return context.Document.WithSyntaxRoot(newRoot);
        }
    }
}

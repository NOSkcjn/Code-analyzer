using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PartA
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyLinesAroundCodeBlocksAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = Constants.FourthTaskID;

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EmptyLinesAroundCodeBlocksAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EmptyLinesAroundCodeBlocksAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EmptyLinesAroundCodeBlocksAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = Constants.CategoryLinesName;

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeText);
        }

        private static void AnalyzeText(SyntaxTreeAnalysisContext context)
        {
            var tree = context.Tree;
            var root = tree.GetRoot();
            List<int> linesWas = new List<int>();
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
                                var location = trivia.GetLocation();
                                var line = location.GetLineSpan().StartLinePosition.Line + 1;
                                if (!linesWas.Contains(line))
                                {
                                    linesWas.Add(line);
                                    var diagnostic = Diagnostic.Create(Rule, location);
                                    context.ReportDiagnostic(diagnostic);
                                }
                            }
                        }
                    }

                    prevToken = token;
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PartA
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhitespacesInEndOfLineAnalyzer: DiagnosticAnalyzer
    {
        public const string DiagnosticId = Constants.ThirdTaskID;
        
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.WhitespacesInEndOfLineTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.WhitespacesInEndOfLineMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.WhitespacesInEndOfLinesDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = Constants.CategorySpacesName;

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
                foreach (var token in tokens)
                {
                    var trivias = token.TrailingTrivia;
                    int current = 1;
                    
                    foreach (var trivia in trivias)
                    {
                        if (trivia.Kind().Equals(SyntaxKind.WhitespaceTrivia))
                        {
                            if (current < trivias.Count())
                            {
                                var location = trivia.GetLocation();
                                var line = location.GetLineSpan().StartLinePosition.Line + 1;
                                if (!linesWas.Contains(line))
                                {
                                    linesWas.Add(line);
                                    var diagnostic = Diagnostic.Create(Rule, trivia.GetLocation());
                                    context.ReportDiagnostic(diagnostic);
                                }
                            }
                        }

                        current++;
                    }
                }
            }
        }
    }
}

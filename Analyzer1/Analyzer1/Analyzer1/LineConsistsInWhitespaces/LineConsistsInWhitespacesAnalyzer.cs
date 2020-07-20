using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PartA
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LineConsistsInWhitespacesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = Constants.FirsTaskID;
        
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.LineConsistsInWhitespacesTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.LineConsistsInWhitespacesMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.LineConsistsInWhitespacesDescription), Resources.ResourceManager, typeof(Resources));
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
                    var trivias = token.LeadingTrivia;
                    for (int i = 0; i < trivias.Count; i++)
                    {
                        if (trivias[i].Kind().Equals(SyntaxKind.WhitespaceTrivia))
                        {
                            const int inc = 1;
                            if (i + inc < trivias.Count && trivias[i + inc].Kind().Equals(SyntaxKind.EndOfLineTrivia))
                            {
                                var location = trivias[i].GetLocation();
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
                }
            }
        }
    }
}

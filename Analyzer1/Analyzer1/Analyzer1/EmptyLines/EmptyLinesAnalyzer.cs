using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PartA
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyLinesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = Constants.SecondTaskID;

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.EmptyLinesAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.EmptyLinesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.EmptyLinesAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
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
                var tokens = node.DescendantTokens().ToList();
                for (int i = 0; i < tokens.Count(); i++)
                {
                    var trivias = tokens[i].LeadingTrivia;
                    int current = 0;

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
                                    var line = location.GetLineSpan().StartLinePosition.Line + 1;
                                    if (!linesWas.Contains(line))
                                    {
                                        var diagnostic = Diagnostic.Create(Rule, location);
                                        context.ReportDiagnostic(diagnostic);
                                        linesWas.Add(line);
                                    }
                                }
                            }
                        }
                        else if (!trivia.Kind().Equals(SyntaxKind.WhitespaceTrivia))
                            break;
                    }
                }
                break;
            }
        }
    }
}

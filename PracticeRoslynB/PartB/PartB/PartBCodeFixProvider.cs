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

namespace PartB
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PartBCodeFixProvider)), Shared]
    public class PartBCodeFixProvider : CodeFixProvider
    {
        private const string title = "Incapsulate";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(PartBAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var fieldDeclaration = root.FindNode(diagnosticSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>();
            var classDeclaration = root.FindNode(diagnosticSpan).FirstAncestorOrSelf<ClassDeclarationSyntax>();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title, c => IncapsulateAsync(context.Document, classDeclaration, fieldDeclaration, c), title),
                diagnostic);
        }

        private async Task<Document> IncapsulateAsync(Document document, ClassDeclarationSyntax classDeclaration,
            FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var accessModifier = fieldDeclaration.Modifiers.FirstOrDefault(m =>
                m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.InternalKeyword) || m.IsKind(SyntaxKind.ProtectedKeyword));
            var staticModifier = fieldDeclaration.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.StaticKeyword));
            var readonlyModifier = fieldDeclaration.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.ReadOnlyKeyword));

            var privateModifier = SyntaxFactory.Token(SyntaxKind.PrivateKeyword);
            var newModifiers = fieldDeclaration.Modifiers.Replace(accessModifier, privateModifier);
            var fieldIdentifierText = fieldDeclaration.Declaration.Variables.FirstOrDefault().Identifier.Text;
            string propertyIdentifierText = fieldIdentifierText;
            //string newFieldIdentifierText = fieldIdentifierText;
            const char underscore = '_';

            int inc = 0;
            if (fieldIdentifierText[0] == underscore)
                inc++;
            propertyIdentifierText = String.Format("{0}{1}", fieldIdentifierText.ToUpper()[0 + inc], fieldIdentifierText.Substring(1 + inc));
            //newFieldIdentifierText = String.Format("{0}{1}", fieldIdentifierText.ToLower()[0 + inc], fieldIdentifierText.Substring(1 + inc));
            //var tempFieldIdentifierText = String.Format("{0}{1}", underscore, newFieldIdentifierText);
            //var expIdentifierToReturn = SyntaxFactory.ParseExpression(newFieldIdentifierText);

            //var newFieldDeclaration = Utils.FieldFromText(fieldDeclaration.Declaration.Type, newFieldIdentifierText).WithModifiers(newModifiers);
            //var tempFieldDeclaration = Utils.FieldFromText(fieldDeclaration.Declaration.Type, tempFieldIdentifierText).WithModifiers(newModifiers);

            //const string valueKeyword = "value";
            //var expEqual = SyntaxFactory.ParseExpression(String.Format("{0} = {1}", newFieldIdentifierText, valueKeyword));
            var property =
                SyntaxFactory.PropertyDeclaration(fieldDeclaration.Declaration.Type, SyntaxFactory.Identifier(propertyIdentifierText))
                .AddModifiers(accessModifier)
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration/*,
                        SyntaxFactory.Block(new List<StatementSyntax>() { SyntaxFactory.ReturnStatement(expIdentifierToReturn) })*/).
                        WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            if (!staticModifier.Equals(default(SyntaxToken)))
                property = property.AddModifiers(staticModifier);
            if (readonlyModifier.Equals(default(SyntaxToken)))
                property = property.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration/*,
                        SyntaxFactory.Block(new List<StatementSyntax>() { SyntaxFactory.ExpressionStatement(expEqual) })*/).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            
            ClassDeclarationSyntax newClassDeclaration = classDeclaration.ReplaceNode(fieldDeclaration, new List<SyntaxNode>() { /*tempFieldDeclaration,*/ property });
            List<SyntaxToken> tokensToReplace = new List<SyntaxToken>();
            foreach (var token in newClassDeclaration.DescendantTokens())
            {
                if (token.ValueText.Equals(fieldIdentifierText))
                    tokensToReplace.Add(token);
            }
            newClassDeclaration = newClassDeclaration.ReplaceTokens(tokensToReplace, (x, y) => SyntaxFactory.Identifier(propertyIdentifierText));
            //var new2ClassDeclaration = newClassDeclaration.ReplaceNode(tempFieldDeclaration, new List<SyntaxNode>() { newFieldDeclaration });

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace PartB
{
    internal static class Utils
    {
        //internal static FieldDeclarationSyntax FieldFromText(TypeSyntax fieldType, string line)
        //{
        //    var varDeclarator = SyntaxFactory.VariableDeclarator(line);
        //    var varDeclaration = SyntaxFactory.VariableDeclaration(fieldType).AddVariables(varDeclarator);
        //    return SyntaxFactory.FieldDeclaration(varDeclaration);
        //}

        //internal static ISymbol FindSymbol(SyntaxNode root, SemanticModel model, string identifier)
        //{
        //    foreach (var node in root.DescendantNodesAndSelf())
        //    {
        //        if (node.IsKind(SyntaxKind.FieldDeclaration))
        //        {
        //            var s = model.GetDeclaredSymbol(node);
        //            if (s != null && s.Name.Equals(identifier))
        //                return s;
        //        }
        //    }
        //    return null;
        //}
    }
}

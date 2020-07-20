using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using PartA;

namespace PartA.Test
{
    [TestClass]
    public class WhitespacesInEndOfLineTests : BasePartATests
    {
        [TestInitialize]
        public void TestInit()
        {
            DiagnosticID = WhitespacesInEndOfLineAnalyzer.DiagnosticId;
            Message = "The line contains whitespaces in the end";
            Severity = DiagnosticSeverity.Warning;
        }

        [TestMethod]
        public void TestWhitespacesAfterClassDeclarationOpenBrace()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";

            #region expected
            var expected = defaultExpected;
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 10) };
            #endregion

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestWhitespacesAfterClassDeclarationCloseBrace()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
        }       
    }";

            #region expected
            var expected = defaultExpected;
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 10) };
            #endregion

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestWhitespacesAfterFieldDeclaration()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            private int abc = 333;   
        }
    }";

            #region expected
            var expected = defaultExpected;
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 35) };
            #endregion

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            private int abc = 333;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WhitespacesInEndOfLineCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new WhitespacesInEndOfLineAnalyzer();
        }
    }
}

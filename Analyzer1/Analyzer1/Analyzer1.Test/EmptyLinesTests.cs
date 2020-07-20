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
    public class EmptyLinesTests : BasePartATests
    {
        [TestInitialize]
        public void TestInit()
        {
            DiagnosticID = EmptyLinesAnalyzer.DiagnosticId;
            Message = "The text contains 2 or more empty lines";
            Severity = DiagnosticSeverity.Warning;
        }

        [TestMethod]
        public void TestEmptyCodeString()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestEmptyLinesBeforeClassName()
        {
            var test = @"
    namespace ConsoleApplication1
    {



        class TypeName
        {
        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2);

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
        public void TestEmptyLinesAfterClassDeclarationOpenBrace()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {



        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {

        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new EmptyLinesCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EmptyLinesAnalyzer();
        }
    }
}

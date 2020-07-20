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
    public class LineConsistsInWhitespacesTests : BasePartATests
    {
        [TestInitialize]
        public void TestInit()
        {
            DiagnosticID = LineConsistsInWhitespacesAnalyzer.DiagnosticId;
            Message = "Empty line contains whitespaces";
            Severity = DiagnosticSeverity.Warning;
        }

        [TestMethod]
        public void TestWhitespacesBeforeClassName()
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
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 1) };
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
        public void TestWhitespacesInMethod()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void Method()
            {
     
            }
        }
    }";

            #region expected
            var expected = defaultExpected;
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 1) };

            #endregion

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public void Method()
            {

            }
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
            private int a = 2;
   
            public void Method()
            {
            }
        }
    }";

            #region expected
            var expected = defaultExpected;
            expected.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 1) };

            #endregion

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            private int a = 2;

            public void Method()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new LineConsistsInWhitespacesCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new LineConsistsInWhitespacesAnalyzer();
        }
    }
}

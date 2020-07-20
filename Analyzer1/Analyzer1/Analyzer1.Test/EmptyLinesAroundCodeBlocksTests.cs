using System;
using System.Collections.Generic;
using System.Text;
using TestHelper;
using PartA;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PartA.Test
{
    [TestClass]
    public class EmptyLinesAroundCodeBlocksTests : CodeFixVerifier
    {
        private const string DiagnosticID = EmptyLinesAroundCodeBlocksAnalyzer.DiagnosticId;
        private const string Message = "Code block contains empty line(s) in start or in end";
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        private DiagnosticResult defaultExpected
        {
            get
            {
                return new DiagnosticResult
                {
                    Id = DiagnosticID,
                    Message = Message,
                    Severity = Severity
                };
            }
        }

        [TestMethod]
        public void TestEmptyLinesBeforeBlockInMethod()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {   
            int a = 2;

            public void Method() 
            {



                a += 3;
            }
        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 1) };

            var expected3 = defaultExpected;
            expected3.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2, expected3);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {   
            int a = 2;

            public void Method() 
            {
                a += 3;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEmptyLinesAfterBlockInMethod()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {   
            int a = 2;

            public void Method() 
            {
                a += 3;


            }
        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {   
            int a = 2;

            public void Method() 
            {
                a += 3;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEmptyLinesBeforeAndAfterBlockInMethod()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            int a = 2;

            public void Method() 
            {


                a += 3;
                a -= 4;

            }
        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 11, 1) };

            var expected3 = defaultExpected;
            expected3.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2, expected3);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            int a = 2;

            public void Method() 
            {
                a += 3;
                a -= 4;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEmptyLinesBeforeBlockInClass()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {


            int a = 2;

            public void Method() 
            {
                a += 3;
            }
        }
    }";

            #region expected
            var expected1 = defaultExpected;
            expected1.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) };

            var expected2 = defaultExpected;
            expected2.Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 1) };
            #endregion

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            int a = 2;

            public void Method() 
            {
                a += 3;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new EmptyLinesAroundCodeBlocksCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EmptyLinesAroundCodeBlocksAnalyzer();
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using PartB;

namespace PartB.Test
{
    [TestClass]
    public class SemanticTreeTests : CodeFixVerifier
    {
        [TestMethod]
        public void TestEmpty()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestEncapsulatePublicIntField()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int b;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "b"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 24)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int B { get; set; }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEncapsulatePublicIntFieldWithUnderscope()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int _b;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "_b"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 24)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int B { get; set; }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEncapsulatePublicIntFieldWithReferences()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int b;

            public void Inc()
            {
                b++;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "b"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 24)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public int B { get; set; }

        public void Inc()
            {
            B++;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEncapsulateInternalStringField()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal string line;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "line"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 29)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal string Line { get; set; }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEncapsulatePublicReadonlyCharField()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public readonly char symbol;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "symbol"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 34)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public char Symbol { get; }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void TestEncapsulatePublicStaticDoubleField()
        {
            var test = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public static double Pi;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "PartB",
                Message = String.Format("The field '{0}' is not private", "Pi"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 34)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    namespace ConsoleApplication1
    {
        class TypeName
        {
            public static double Pi { get; set; }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new PartBCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PartBAnalyzer();
        }
    }
}

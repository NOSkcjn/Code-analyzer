using Microsoft.CodeAnalysis;
using TestHelper;

namespace PartA.Test
{
    public class BasePartATests: CodeFixVerifier
    {
        protected string DiagnosticID;
        protected string Message;
        protected DiagnosticSeverity Severity;

        protected DiagnosticResult defaultExpected
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
    }
}

using System;
using System.Linq;

namespace EFSpecs
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }

        public override string StackTrace
        {
            get
            {
                string Namespace = this.GetType().Namespace;
                var stacktracestring = SplitTheStackTraceByEachNewLine()
                    .Where(s => !s.TrimStart(' ').StartsWith("at " + Namespace))
                    .ToArray();
                return JoinArrayWithNewLineCharacters(stacktracestring);
            }
        }

        private string[] SplitTheStackTraceByEachNewLine()
        {
            return base.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string JoinArrayWithNewLineCharacters(string[] stacktracestring)
        {
            return string.Join(Environment.NewLine, stacktracestring);
        }
    }
}
using System;
using System.ComponentModel.Composition;
using DemoApp.Contracts;

namespace DemoApp.Implementations
{
    [Export(typeof(IOutput))]
    public class QuietConsoleOutput : IOutput
    {
        public void Write(string s)
        {
            Console.WriteLine(s.ToLowerInvariant());
        }
    }
}
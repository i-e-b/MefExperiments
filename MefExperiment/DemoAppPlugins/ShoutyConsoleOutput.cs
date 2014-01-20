using System;
using System.ComponentModel.Composition;
using DemoApp.Contracts;

namespace DemoAppPlugins
{
    [Export(typeof(IOutput))]
    public class ShoutyConsoleOutput : IOutput
    {
        public void Write(string s)
        {
            Console.WriteLine(s.ToUpperInvariant());
        }
    }
}
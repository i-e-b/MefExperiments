using System;
using System.ComponentModel.Composition;
using System.Linq;
using DemoApp.Contracts;

namespace DemoAppPlugins
{
    [Export(typeof(IOutput))]
    public class MashConsoleOutput : IOutput
    {
        public void Write(string s)
        {
            Console.WriteLine(string.Join("*", s.ToList()));
        }
    }
}
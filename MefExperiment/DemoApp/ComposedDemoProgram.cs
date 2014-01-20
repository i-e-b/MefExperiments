using System.Collections.Generic;
using System.ComponentModel.Composition;
using DemoApp.Contracts;
using DemoApp.Roots;

namespace DemoApp
{
    /// <summary>
    /// poco container for the program's agregate roots (entry points to behaviours)
    /// </summary>
    public class ComposedDemoProgram
    {
        [Import]
        public DemoInstance Instance { get; set; } // this demands an IDemoDataSource
        
        [ImportMany(typeof(IOutput))]
        public IEnumerable<IOutput> Outputs { get; set; }
    }
}
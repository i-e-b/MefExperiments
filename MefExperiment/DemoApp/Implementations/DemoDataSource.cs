using System.Collections.Generic;
using System.ComponentModel.Composition;
using DemoApp.Contracts;

namespace DemoApp.Implementations
{
    [Export(typeof(IDemoDataSource))]
    public class DemoDataSource : IDemoDataSource
    {
        public IEnumerable<string> GetData(int id)
        {
            return new[] { "One", "Two", "Three" };
        }
    }
}
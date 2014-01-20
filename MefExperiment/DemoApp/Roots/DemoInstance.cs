using System.Collections.Generic;
using System.ComponentModel.Composition;
using DemoApp.Contracts;

namespace DemoApp.Roots
{
    /// <summary>
    /// This is a self-exporting class. It allows you to define a root class that MEF will fully
    /// auto-populate. Note that it is decorated with an ExportAttribute with its own type,
    /// and that it has a constructor with the ImportingConstructorAttribute. Both of these are required.
    /// </summary>
    [Export(typeof(DemoInstance))]
    public class DemoInstance
    {
        public readonly IDemoDataSource DataSource;
        public readonly IEnumerable<IOutput> Outputs;

        [ImportingConstructor]
        public DemoInstance(
            IDemoDataSource dataSource,                                 // single dependencies don't need any decoration
            [ImportMany(typeof(IOutput))]IEnumerable<IOutput> outputs   // group dependencies must be marked as ImportManyAttribute
            )
        {
            DataSource = dataSource;
            Outputs = outputs;
        }

        public void GoDoStuff()
        {
            foreach (var output in Outputs)
            {
                output.Write(string.Join(", ", DataSource.GetData(1)));
            }
        }
    }
}
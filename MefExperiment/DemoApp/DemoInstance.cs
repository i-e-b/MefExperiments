using System;
using System.ComponentModel.Composition;
using DemoApp.Contracts;

namespace DemoApp
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

        [ImportingConstructor]
        public DemoInstance(IDemoDataSource dataSource)
        {
            DataSource = dataSource;
        }

        public void GoDoStuff()
        {
            Console.WriteLine(string.Join(", ", DataSource.GetData(1)));
        }
    }
}
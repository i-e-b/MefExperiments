using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using DemoApp.Contracts;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // MEF Bootstrap (MEF comes from System.ComponentModel.Composition in the GAC)
            using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
            {
                var container = new CompositionContainer(catalog);

                var parts = new PartsContainer();
                container.SatisfyImportsOnce(parts);

                var instance = new DemoInstance(parts);

                instance.GoDoStuff();
            }

            Console.WriteLine("Done. Press [enter]");
            Console.ReadKey();
        }
    }
}

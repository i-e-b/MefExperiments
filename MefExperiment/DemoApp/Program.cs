using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace DemoApp
{
    public class Program
    {
        /// <summary>
        /// MEF Bootstrap (MEF comes from System.ComponentModel.Composition in the GAC).
        /// <para>This will return a class containing all the application's aggregate roots.</para>
        /// </summary>
        public static ComposedDemoProgram Configure()
        {
            using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
            {
                var container = new CompositionContainer(catalog);

                var composedProgram = new ComposedDemoProgram();
                container.SatisfyImportsOnce(composedProgram);

                return composedProgram;
            }
        }

        /// <summary>
        /// poco container for the program's agregate roots (entry points to behaviours)
        /// </summary>
        public class ComposedDemoProgram
        {
            [Import]
            public DemoInstance Instance { get; set; } // this demands an IDemoDataSource
        }

        static void Main()
        {
            Configure().Instance.GoDoStuff();

            Console.WriteLine("Done. Press [enter]");
            Console.ReadKey();
        }
    }
}

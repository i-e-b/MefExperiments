using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace DemoApp
{
    public class Program
    {
        /// <summary>
        /// MEF Bootstrap (MEF comes from System.ComponentModel.Composition in the GAC).
        /// <para>This will return a class containing all the application's aggregate roots.</para>
        /// </summary>
        public static ComposedDemoProgram Configure(params string[] pluginDirectories)
        {
            var catalogues = 
                pluginDirectories.Select<string, ComposablePartCatalog>(d=>new DirectoryCatalog(d)).
                Concat(new []{new AssemblyCatalog(Assembly.GetExecutingAssembly())}).ToList();

            var catalog = new AggregateCatalog(catalogues);
            try
            {
                var container = new CompositionContainer(catalog);

                var composedProgram = new ComposedDemoProgram();
                container.SatisfyImportsOnce(composedProgram);

                return composedProgram;
            }
            finally
            {
                catalog.Dispose();
                foreach (var cat in catalogues)
                {
                    cat.Dispose();
                }
            }
        }


        static void Main()
        {
            Configure("./Plugins").Instance.GoDoStuff();

            Console.WriteLine("Done. Press [enter]");
            Console.ReadKey();
        }
    }
}

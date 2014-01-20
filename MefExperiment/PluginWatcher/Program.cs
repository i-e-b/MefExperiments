using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PluginWatcher
{
    public class Program
    {
        private static CompositionContainer _container;
        private static readonly object _lock = new object();
        private static DirectoryCatalog _pluginCatalog;
        private static AssemblyCatalog _localCatalog;
        private static bool _reload;


        static void Main()
        {
            Console.WriteLine("As plugins are added and removed, you should see output below. Press [enter] to exit");

            _pluginCatalog = new DirectoryCatalog("./Plugins");
            _localCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(_localCatalog);
            catalog.Catalogs.Add(_pluginCatalog);

            _container = new CompositionContainer(catalog, false);
            _container.ExportsChanged += ExportsChanged;

            _reload = true;

            try
            {
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        lock (_lock)
                        {
                            Thread.Sleep(200);
                            try
                            {
                                _pluginCatalog.Refresh();
                            }
                            catch (ChangeRejectedException rejex)
                            {
                                Console.WriteLine("Could not update plugins: " + rejex.Message);
                            }
                        }

                        lock (_lock)
                        {
                            if (_reload)
                            {
                                _reload = false;
                                ListKnownPlugins();
                            }
                        }
                    }
                }
                while (Console.ReadKey().Key != ConsoleKey.Enter);

            }
            finally
            {
                _pluginCatalog.Dispose();
                _localCatalog.Dispose();
                catalog.Dispose();
                _container.Dispose();
            }
        }

        static void ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            lock (_lock)
            {
                if (e.AddedExports.Any() || e.RemovedExports.Any())
                    _reload = true;
            }
        }

        private static void ListKnownPlugins()
        {
            var loaded = new PluginContainer();
            lock (_lock)
            {
                _container.ComposeParts(loaded);
            }
            Console.WriteLine("\r\nPlugins reloaded:");
            Console.WriteLine(string.Join(", ", loaded.Plugins.Select(p => p.Name())));
        }
    }
}

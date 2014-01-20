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
        private static readonly object CompositionLock = new object();
        private static bool _reload;
        private static PluginContainer _loadedPlugins;


        static void Main()
        {
            Console.WriteLine("As plugins are added and removed, you should see output below. Press [enter] to exit");

            var pluginCatalog = new DirectoryCatalog("./Plugins");
            var localCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(localCatalog);
            catalog.Catalogs.Add(pluginCatalog);

            _container = new CompositionContainer(catalog, false);
            _container.ExportsChanged += ExportsChanged;

            _reload = true;

            try
            {
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        lock (CompositionLock)
                        {
                            Thread.Sleep(200);
                            try
                            {
                                pluginCatalog.Refresh();
                            }
                            catch (ChangeRejectedException rejex)
                            {
                                Console.WriteLine("Could not update plugins: " + rejex.Message);
                            }
                        }

                        lock (CompositionLock)
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
                pluginCatalog.Dispose();
                localCatalog.Dispose();
                catalog.Dispose();
                _container.Dispose();
            }
        }

        static void ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            lock (CompositionLock)
            {
                if (e.AddedExports.Any() || e.RemovedExports.Any()) _reload = true;
            }
        }

        private static void ListKnownPlugins()
        {
            _loadedPlugins = new PluginContainer();
            lock (CompositionLock)
            {
                _container.ComposeParts(_loadedPlugins);
            }
            Console.WriteLine("\r\nPlugins reloaded:");
            Console.WriteLine(string.Join(", ", _loadedPlugins.Plugins.Select(p => p.Name())));
        }
    }
}

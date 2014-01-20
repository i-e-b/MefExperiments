using System;
using System.Linq;
using System.Threading;

namespace PluginWatcher
{
    public class Program
    {
        static void Main()
        {
            Console.WriteLine("As plugins are added and removed, you should see output below. Press [enter] to exit");

            var watcher = new PluginWatcher("./Plugins");
            watcher.PluginsChanged += watcher_PluginsChanged;


            Console.WriteLine("\r\nPlugins loaded:");
            Console.WriteLine(string.Join(", ", watcher.CurrentlyAvailable.Select(p => p.Name())));

            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(200);
                }
            }
            while (Console.ReadKey().Key != ConsoleKey.Enter);
        }

        static void watcher_PluginsChanged(object sender, PluginsChangedEventArgs e)
        {
            Console.WriteLine("\r\nPlugins reloaded:");
            Console.WriteLine(string.Join(", ", e.AvailablePlugins.Select(p => p.Name())));
        }
    }
}

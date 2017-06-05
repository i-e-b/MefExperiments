namespace HotSwapDemo.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using HotSwap.Contracts;
    using PluginWatcher;

    public class Program
    {
        private static readonly object VersionLock = new object();
        static ISimpleTask current;

        static void Main()
        {
            Console.WriteLine("Copy new versions into the Plugins folder, and the output below should change. Press [enter] to exit");

            IPluginWatcher<ISimpleTask> watcher = new PluginWatcher<ISimpleTask>("./Plugins");
            watcher.PluginsChanged += watcher_PluginsChanged;

            lock (VersionLock)
            {
                current = Latest(watcher.CurrentlyAvailable);

                Console.WriteLine("\r\nInitial startup\r\nLatest version:");
                Console.WriteLine(current.Version());

                // Here we set some state that should be maintained between versions
                current.SetGreets("Hello");
            }

            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(200);
                    lock (VersionLock)
                    {
                        Console.SetCursorPosition(0, 8);
                        Console.WriteLine(current.Greeting("world"));
                    }
                }
            }
            while (Console.ReadKey().Key != ConsoleKey.Enter);
        }

        /// <summary>
        /// Do a version upgrade!
        /// </summary>
        static void Upgrade(IHotSwap old, ISimpleTask latest)
        {
            lock (VersionLock)
            {
                try
                {
                    latest.SetState(old.GetState());
                    current = latest;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can't upgrade: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Set of plugins has changed
        /// </summary>
        static void watcher_PluginsChanged(object sender, PluginsChangedEventArgs<ISimpleTask> e)
        {
            var latest = Latest(e.AvailablePlugins);
            lock (VersionLock)
            {
                if (latest.Version() > current.Version()) Upgrade(current, latest);
                
                Console.SetCursorPosition(0, 3);
                Console.WriteLine("Latest version:");
                Console.WriteLine(current.Version());
            }
        }

        /// <summary>
        /// Get highest version available
        /// </summary>
        static ISimpleTask Latest(IEnumerable<ISimpleTask> currentlyAvailable)
        {
            var list = currentlyAvailable.ToList();
            if (!list.Any()) throw new Exception("No tasks available");
            return list.OrderBy(task => task.Version()).Last();
        }

    }
}

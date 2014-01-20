using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using PluginWatcher.Contracts;

namespace PluginWatcher
{
    /// <summary>
    /// Watches for a fixed kind of plugin
    /// </summary>
    /// <remarks>
    /// The mix of attributes, exceptions, thread safety problems and odd workflow makes this 
    /// behaviour very tricky to implement, and full of gotchas.
    /// Every weird implementation detail below is the result of trial-and-error.
    /// Change it at your peril.
    /// <para>How it works:</para>
    /// A file system watcher waits for *.dll changes in the target directory. On each change, the DirectoryCatalog will be refreshed.
    /// That triggers `ExportsChanged`, which may flag `_reload`. If reload is set to true, the update thread will recompose the list of plugins.
    /// </remarks>
    public class PluginWatcher : IDisposable
    {
        [ImportMany(typeof(IWatchablePlugin), AllowRecomposition = true)]
        protected IEnumerable<IWatchablePlugin> Plugins { get; set; }


        private static readonly object CompositionLock = new object();
        private volatile bool _reload;
        private volatile bool _running;

        private readonly string _pluginDirectory;
        private FileSystemWatcher _fsw;
        private Thread _updateThread;
        private DirectoryCatalog _pluginCatalog;

        public event EventHandler<PluginsChangedEventArgs> PluginsChanged;

        protected virtual void OnPluginsChanged()
        {
            var handler = PluginsChanged;
            if (handler != null) handler(this, new PluginsChangedEventArgs { AvailablePlugins = CurrentlyAvailable });
        }

        public PluginWatcher(string pluginDirectory)
        {
            _pluginDirectory = pluginDirectory;
            if (!Directory.Exists(_pluginDirectory)) throw new Exception("Can't watch \"" + _pluginDirectory + "\", might not exist or not enough permissions");

            CurrentlyAvailable = new IWatchablePlugin[0];
            _fsw = new FileSystemWatcher(_pluginDirectory, "*.dll");
            SetupFileWatcher();

            _reload = true;
            _running = true;

            _updateThread = new Thread(UpdateThreadLoop) { IsBackground = true };
            _updateThread.Start();

            while (_reload) { Thread.Sleep(100); }
        }

        private void UpdateThreadLoop()
        {
            _pluginCatalog = new DirectoryCatalog("./Plugins");
            try
            {
                using (var localCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
                using (var catalog = new AggregateCatalog())
                {
                    catalog.Catalogs.Add(localCatalog);
                    catalog.Catalogs.Add(_pluginCatalog);

                    using (var container = new CompositionContainer(catalog, false))
                    {
                        container.ExportsChanged += ExportsChanged;

                        while (_running)
                        {
                            lock (CompositionLock)
                            {
                                if (_reload)
                                {
                                    container.ComposeParts(this);
                                    CurrentlyAvailable = (Plugins ?? new IWatchablePlugin[0]).ToArray();

                                    _reload = false;
                                    OnPluginsChanged();
                                }
                            }
                            while (_running && !_reload) Thread.Sleep(1000);
                        }
                    }
                }
            }
            finally
            {
                lock (CompositionLock)
                {
                    var cat = Interlocked.Exchange(ref _pluginCatalog, null);
                    if (cat != null) cat.Dispose();
                }
            }
        }

        private void SetupFileWatcher()
        {
            _fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName |
                                NotifyFilters.LastAccess | NotifyFilters.LastWrite    | NotifyFilters.Size     | NotifyFilters.Security;

            _fsw.Changed += FileAddedOrRemoved;
            _fsw.Created += FileAddedOrRemoved;
            _fsw.Deleted += FileAddedOrRemoved;
            _fsw.Renamed += FileRenamed;

            _fsw.EnableRaisingEvents = true;
        }


        private void ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            lock (CompositionLock)
            {
                // if anything changed, trigger a rebuild (can't do it on the event thread)
                if (e.AddedExports.Any() || e.RemovedExports.Any()) _reload = true;
            }
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            RefreshPlugins();
        }

        void FileAddedOrRemoved(object sender, FileSystemEventArgs e)
        {
            RefreshPlugins();
        }

        private void RefreshPlugins()
        {
            try
            {
                var cat = _pluginCatalog;
                if (cat == null) { return; }
                lock (CompositionLock)
                {
                    cat.Refresh();
                }
            }
            catch (ChangeRejectedException rejex)
            {
                Console.WriteLine("Could not update plugins: " + rejex.Message);
            }
        }

        public IEnumerable<IWatchablePlugin> CurrentlyAvailable { get; protected set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            _running = false;
            var fsw = Interlocked.Exchange(ref _fsw, null);
            if (fsw != null) fsw.Dispose();

            var thread = Interlocked.Exchange(ref _updateThread, null);
            if (thread != null)
            {
                thread.Join(2000);
            }
        }
    }

    public class PluginsChangedEventArgs: EventArgs
    {
        public IEnumerable<IWatchablePlugin> AvailablePlugins { get; set; }
    }
}
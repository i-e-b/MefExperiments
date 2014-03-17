namespace PluginWatcher
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Watch for changes to a plugin directory for a specific MEF Import type.
    /// <para>Keeps a list of last seen exports and exposes a change event</para>
    /// </summary>
    /// <typeparam name="T">Plugin type. Plugins should contain classes implementing this type and decorated with [Export(typeof(...))]</typeparam>
    public class PluginWatcher<T> : IPluginWatcher<T>
    {
        private readonly object _compositionLock = new object();
        private AggregateCatalog _catalog;

        private CompositionContainer _container;
        private FileSystemWatcher _fsw;
        private AssemblyCatalog _localCatalog;
        private DirectoryCatalog _pluginCatalog;

        public PluginWatcher(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory)) throw new Exception("Can't watch \"" + pluginDirectory + "\", might not exist or not enough permissions");

            CurrentlyAvailable = new T[0];
            _fsw = new FileSystemWatcher(pluginDirectory, "*.dll");
            SetupFileWatcher();

            try
            {
                _pluginCatalog = new DirectoryCatalog(pluginDirectory);
                _localCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
                _catalog = new AggregateCatalog();
                _catalog.Catalogs.Add(_localCatalog);
                _catalog.Catalogs.Add(_pluginCatalog);
                _container = new CompositionContainer(_catalog, false);
                _container.ExportsChanged += ExportsChanged;
            }
            catch
            {
                Dispose(true);
                throw;
            }

            ReadLoadedPlugins();
        }

        ~PluginWatcher()
        {
            Dispose(true);
        }

        public event EventHandler<PluginsChangedEventArgs<T>> PluginsChanged;
        public IEnumerable<T> CurrentlyAvailable { get; protected set; }

        protected virtual void OnPluginsChanged()
        {
            var handler = PluginsChanged;
            if (handler != null) handler(this, new PluginsChangedEventArgs<T> { AvailablePlugins = CurrentlyAvailable });
        }

        private void SetupFileWatcher()
        {
            _fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName |
                                NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security;

            _fsw.Changed += FileAddedOrRemoved;
            _fsw.Created += FileAddedOrRemoved;
            _fsw.Deleted += FileAddedOrRemoved;
            _fsw.Renamed += FileRenamed;

            _fsw.EnableRaisingEvents = true;
        }

        private void ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            lock (_compositionLock)
            {
                if (e.AddedExports.Any() || e.RemovedExports.Any()) ReadLoadedPlugins();
            }
        }

        private void ReadLoadedPlugins()
        {
            CurrentlyAvailable = _container.GetExports<T>().Select(y => y.Value).ToArray();
            OnPluginsChanged();
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
                lock (_compositionLock)
                {
                    cat.Refresh();
                }
            }
            catch (ChangeRejectedException rejex)
            {
                Console.WriteLine("Could not update plugins: " + rejex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (!disposing) return;

            var fsw = Interlocked.Exchange(ref _fsw, null);
            if (fsw != null) fsw.Dispose();

            var plg = Interlocked.Exchange(ref _pluginCatalog, null);
            if (plg != null) plg.Dispose();

            var con = Interlocked.Exchange(ref _container, null);
            if (con != null) con.Dispose();

            var loc = Interlocked.Exchange(ref _localCatalog, null);
            if (loc != null) loc.Dispose();

            var cat = Interlocked.Exchange(ref _catalog, null);
            if (cat != null) cat.Dispose();
        }
    }

}
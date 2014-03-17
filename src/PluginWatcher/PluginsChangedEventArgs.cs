namespace PluginWatcher
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Event arguments relating to a change in available MEF Export types.
    /// </summary>
    public class PluginsChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Last known Exports matching type <typeparamref name="T"/>.
        /// </summary>
        public IEnumerable<T> AvailablePlugins { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.Composition;
using PluginWatcher.Contracts;

namespace PluginWatcher
{
    public class PluginContainer
    {
        [ImportMany(typeof(IWatchablePlugin), AllowRecomposition = true)]
        public IEnumerable<IWatchablePlugin> Plugins { get; set; }
    }
}
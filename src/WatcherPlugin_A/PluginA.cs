using System.ComponentModel.Composition;
using PluginWatcher.Contracts;

namespace WatcherPlugin_A
{
    [Export(typeof(IWatchablePlugin))]
    public class PluginA : IWatchablePlugin
    {
        public string Name()
        {
            return "I am plugin 'A'";
        }
    }
}

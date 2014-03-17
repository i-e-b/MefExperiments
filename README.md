MEF Experiments
===============

A few quick demo projects showing Dependency injection and run-time dynamic plugins using MEF.

* Plugin watcher
  
  A utility class to watch folders for changing plugins of a specific type

* Simple plugin and IoC demo
  
  Shows basic use of contract interfaces, plugin implementations, unit testing and
  a simple composed application to bring it all together.

  * DemoApp
  * DemoApp.Contracts
  * DemoApp.Unit.Tests
  * DemoAppPlugins

* Live plugin updates
  
  Using the PluginWatcher to add and remove implementations at run time

  * PluginWatcher.App
  * PluginWatcher.Contracts
  * WatcherPlugin_A

* Hot code-swap
  
  Showing a simple way to update an implementation at run time, using the PluginWatcher

  * HotSwap.Contracts -- basic upgrade contract and a sample task for the demo
  * HotSwap.Greeting.V1 -- first version, configured at startup
  * HotSwap.Greeting.V2 -- second version. Drop it's `.dll` file in the running
HotSwapDemo.App's bin/Plugins folder to see an upgrade happen
  * HotSwapDemo.App -- an application that does a simple hot code swap 
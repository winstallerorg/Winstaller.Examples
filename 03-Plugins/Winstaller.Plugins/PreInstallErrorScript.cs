using System;
using Winstaller.Interfaces;

namespace Winstaller.Plugins
{
    /// <summary>
    /// Plugin pre-installation script throwing an exception for testing purposes.
    /// </summary>
    [PluginName("DemoPreInstallErrorScript")]
    public class PreInstallErrorScript : IScript
    {
        public void Execute(InstallerContext installerContext)
        {
            var logger = installerContext.GetService<ILogger>();
            logger?.Error("DemoPreInstallErrorScript: Intentional pre-installation error script execution.");
            throw new InvalidOperationException("DemoPreInstallErrorScript: Test exception thrown before installation.");
        }
    }
}

using System;
using Winstaller.Sdk;

namespace Winstaller.Plugins
{
    /// <summary>
    /// Plugin post-installation script throwing an exception for testing purposes.
    /// </summary>
    [PluginName("DemoPostInstallErrorScript")]
    public class PostInstallErrorScript : IScript
    {
        public void Execute(InstallerContext installerContext)
        {
            var logger = installerContext.GetService<ILogger>();
            logger?.Error("DemoPostInstallErrorScript: Intentional post-installation error script execution.");
            throw new InvalidOperationException("DemoPostInstallErrorScript: Test exception thrown after installation.");
        }
    }
}


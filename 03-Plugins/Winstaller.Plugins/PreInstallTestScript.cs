using Winstaller.Sdk;

namespace Winstaller.Plugins
{
    [PluginName("DemoPreInstallScript")]
    public class PreInstallTestScript : IScript
    {
        public void Execute(InstallerContext installerContext)
        {
            var logger = installerContext.GetService<ILogger>();
            logger?.Info("DemoPreInstallScript: Pre-installation script executed successfully.");
        }
    }
}


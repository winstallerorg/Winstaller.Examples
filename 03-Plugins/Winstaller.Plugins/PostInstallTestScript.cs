using Winstaller.Interfaces;

namespace Winstaller.Plugins
{
    [PluginName("DemoPostInstallScript")]
    public class PostInstallTestScript : IScript
    {
        public void Execute(InstallerContext installerContext)
        {
            var logger = installerContext.GetService<ILogger>();
            logger?.Info("DemoPostInstallScript: Post-installation script executed successfully.");
        }
    }
}

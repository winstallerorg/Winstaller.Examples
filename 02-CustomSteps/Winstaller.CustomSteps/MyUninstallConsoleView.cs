using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyUninstallConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyUninstall uninstall)
            {
                if (!uninstall.Context.IsSilent)
                {
                    console.WriteLine("[CUSTOM] Uninstalling application...");
                }

                return Task.Run(async () =>
                {
                    try
                    {
                        await uninstall.ExecuteUninstallAsync(uninstall.Context.Progress);
                        console.WriteSuccess("[CUSTOM] Uninstallation finished.");
                        uninstall.RaiseNext();
                    }
                    catch (Exception ex)
                    {
                        console.WriteError($"[CUSTOM] Uninstallation failed: {ex.Message}");
                        uninstall.RaiseCanceled();
                    }
                });
            }
            return Task.FromResult(0);
        }
    }
}

using System;
using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyInstallConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyInstall install)
            {
                if (!install.Context.IsSilent)
                {
                    console.WriteLine("[CUSTOM] Installing components and copying files...");
                }

                return Task.Run(async () =>
                {
                    try
                    {
                        await install.ExecuteInstallAsync(install.Context.Progress);
                        console.WriteSuccess("[CUSTOM] File installation completed.");
                        install.RaiseNext();
                    }
                    catch (Exception ex)
                    {
                        console.WriteError($"[CUSTOM] Installation error: {ex.Message}");
                        install.RaiseCanceled();
                    }
                });
            }
            return Task.FromResult(0);
        }
    }
}

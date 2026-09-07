using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyInstallTypeConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyInstallType installType)
            {
                if (!installType.Context.IsSilent)
                {
                    console.WriteLine($"[CUSTOM] Installation path: {installType.Context.InstallationPath}");
                    console.WriteLine();
                }
                installType.RaiseNext();
            }
            return Task.FromResult(0);
        }
    }
}

using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyWelcomeConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyWelcome welcome)
            {
                if (!welcome.Context.IsSilent)
                {
                    console.WriteLine("======================================================================");
                    console.WriteSuccess($"  [CUSTOM] Welcome to {welcome.Context.AppName} Setup");
                    if (!string.IsNullOrEmpty(welcome.Context.AppVersion))
                    {
                        console.WriteLine($"  Version: {welcome.Context.AppVersion}");
                    }
                    console.WriteLine("======================================================================");
                    console.WriteLine();
                }
                welcome.RaiseNext();
            }
            return Task.FromResult(0);
        }
    }
}

using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MySuccessConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MySuccess success)
            {
                if (!success.Context.IsSilent)
                {
                    console.WriteLine();
                    console.WriteLine("======================================================================");
                    console.WriteSuccess($"  [CUSTOM] {success.Context.AppName} has been successfully installed!");
                    console.WriteLine("======================================================================");
                    console.WriteLine();
                }
                success.RaiseNext();
            }
            return Task.FromResult(0);
        }
    }
}

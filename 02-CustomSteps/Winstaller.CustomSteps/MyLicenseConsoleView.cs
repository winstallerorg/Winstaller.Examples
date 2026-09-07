using System.Threading.Tasks;
using Winstaller.Sdk;

namespace Winstaller.CustomSteps
{
    public class MyLicenseConsoleView : IConsoleStepView
    {
        public Task ExecuteAsync(IStep step, IConsoleContext console)
        {
            if (step is MyLicense license)
            {
                if (license.Context.IsSilent)
                {
                    license.RaiseNext();
                    return Task.FromResult(0);
                }

                console.WriteLine("--- [CUSTOM] End User License Agreement ---");
                var text = license.GetLicenseText();
                if (!string.IsNullOrEmpty(text))
                {
                    console.WriteLine(text.Length > 200 ? text.Substring(0, 200) + "..." : text);
                }
                console.WriteLine();

                bool accepted = console.PromptYesNo("Do you accept the custom license terms?", defaultYes: true);
                if (accepted)
                {
                    license.RaiseNext();
                }
                else
                {
                    console.WriteError("License was not accepted. Aborting setup.");
                    license.RaiseCanceled();
                }
            }
            return Task.FromResult(0);
        }
    }
}

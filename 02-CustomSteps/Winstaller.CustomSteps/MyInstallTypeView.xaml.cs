using Winstaller.Sdk;
using Winstaller.Sdk.Services;
using System;
using System.Windows;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MyInstallTypeView.xaml
    /// </summary>
    public partial class MyInstallTypeView : StepViewBase
    {
        private readonly MyInstallType _step;

        public MyInstallTypeView(MyInstallType step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MyInstallTypeView_Loaded;

            LocalRadioButton.Checked += (s, e) => UpdatePathPreview();
            SystemRadioButton.Checked += (s, e) => UpdatePathPreview();
        }

        private void MyInstallTypeView_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Installation Scope";
            Header = "Installation Scope";

            IsBackVisible = true;
            IsBackEnabled = true;
            IsNextVisible = true;
            IsNextEnabled = true;
            IsCancelVisible = true;
            IsCancelEnabled = true;
            IsFinishVisible = false;

            var pathService = _step.Context.GetService<IPathService>();
            bool initiallyLocal = pathService != null && pathService.IsUserDirectory(_step.Context.InstallationPath);
            if (initiallyLocal)
            {
                LocalRadioButton.IsChecked = true;
            }
            else
            {
                SystemRadioButton.IsChecked = true;
            }

            UpdatePathPreview();
        }

        private void UpdatePathPreview()
        {
            try
            {
                var pathResolver = _step.Context.GetService<IPathVariableResolver>();
                if (pathResolver == null)
                {
                    PathPreviewTextBlock.Text = _step.Context.InstallationPath;
                    return;
                }

                var nameNormalizer = _step.Context.GetService<INameNormalizer>();
                string appNameSafe = nameNormalizer != null ? nameNormalizer.Normalize(_step.Context.AppName) : _step.Context.AppName;
                pathResolver.SetVariable("appname", appNameSafe);

                string baseDir = LocalRadioButton.IsChecked == true ? "%localappdata%" : "%programfiles%";
                string subDir = _step.Context.Data.TryGetValue("Config.Installation.Dir", out var dirVal) && !string.IsNullOrEmpty(dirVal)
                    ? dirVal.ToString()
                    : "%appname%";

                string rawInstallPath = System.IO.Path.Combine(baseDir, subDir);
                string resolvedInstallPath = pathResolver.Resolve(rawInstallPath);

                PathPreviewTextBlock.Text = resolvedInstallPath;
            }
            catch (Exception ex)
            {
                PathPreviewTextBlock.Text = "Error: " + ex.Message;
            }
        }

        public override void OnNext()
        {
            _step.Context.InstallationPath = PathPreviewTextBlock.Text;
            _step.Context.Data["Installation.Type"] = LocalRadioButton.IsChecked == true ? "local" : "system";
            _step.RaiseNext();
        }

        public override void OnBack()
        {
            _step.RaiseBack();
        }

        public override void OnCancel()
        {
            _step.RaiseCanceled();
        }
    }
}


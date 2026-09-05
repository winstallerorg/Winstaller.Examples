using Winstaller.Sdk;
using System;
using System.Windows;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MyConfirmUninstallView.xaml
    /// </summary>
    public partial class MyConfirmUninstallView : StepViewBase
    {
        private readonly MyConfirmUninstall _step;

        public MyConfirmUninstallView(MyConfirmUninstall step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MyConfirmUninstallView_Loaded;
        }

        private void MyConfirmUninstallView_Loaded(object sender, RoutedEventArgs e)
        {
            var localization = _step.Context.GetService<ILocalizationService>();
            
            Title = localization != null 
                ? localization.GetString("ConfirmStep_Title", _step.Context.AppName) 
                : $"Uninstall {_step.Context.AppName}";
                
            Header = Title;
            HeaderTextBlock.Text = Header;

            if (_step.Message != null)
            {
                MessageTextBlock.Text = _step.Message;
            }
            else
            {
                MessageTextBlock.Text = localization != null
                    ? localization.GetString("ConfirmStep_MessageDefault", _step.Context.AppName)
                    : $"Are you sure you want to completely remove {_step.Context.AppName} and all of its components?";
            }

            IsBackVisible = false;
            IsBackEnabled = false;
            IsNextVisible = true;
            IsNextEnabled = true;
            IsCancelVisible = true;
            IsCancelEnabled = true;
            IsFinishVisible = false;
        }

        public override void OnNext()
        {
            _step.RaiseNext();
        }

        public override void OnCancel()
        {
            _step.RaiseCanceled();
        }
    }
}


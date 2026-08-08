using Winstaller.Interfaces;
using System;
using System.Windows;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MyWelcomeView.xaml
    /// </summary>
    public partial class MyWelcomeView : StepViewBase
    {
        private readonly MyWelcome _step;

        public MyWelcomeView(MyWelcome step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MyWelcomeView_Loaded;
        }

        private void MyWelcomeView_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Welcome";
            Header = "Welcome";

            IsBackVisible = false;
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

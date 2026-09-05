using Winstaller.Sdk;
using System;
using System.Windows;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MySuccessView.xaml
    /// </summary>
    public partial class MySuccessView : StepViewBase
    {
        private readonly MySuccess _step;

        public MySuccessView(MySuccess step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MySuccessView_Loaded;
        }

        private void MySuccessView_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Finished";
            Header = "Finished";

            IsBackVisible = false;
            IsNextVisible = false;
            IsCancelVisible = false;
            IsFinishVisible = true;
            IsFinishEnabled = true;
        }

        public override void OnFinish()
        {
            _step.RaiseNext();
        }
    }
}


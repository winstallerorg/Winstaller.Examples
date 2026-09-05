using Winstaller.Sdk;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Winstaller.CustomSteps
{
    /// <summary>
    /// Interaction logic for MyLicenseView.xaml
    /// </summary>
    public partial class MyLicenseView : StepViewBase
    {
        private readonly MyLicense _step;

        public MyLicenseView(MyLicense step)
        {
            _step = step;
            Step = step;
            InitializeComponent();
            Loaded += MyLicenseView_Loaded;
        }

        private void MyLicenseView_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "License Agreement";
            Header = "License Agreement";

            IsBackVisible = true;
            IsBackEnabled = true;
            IsNextVisible = true;
            IsNextEnabled = false; // Enabled only when CheckBox is checked
            IsCancelVisible = true;
            IsCancelEnabled = true;
            IsFinishVisible = false;

            LoadLicenseContent();
        }

        private void LoadLicenseContent()
        {
            try
            {
                if (_step.IsRtfFormat())
                {
                    using (var stream = _step.GetLicenseStream())
                    {
                        if (stream != null)
                        {
                            TextRange range = new TextRange(LicenseRichTextBox.Document.ContentStart, LicenseRichTextBox.Document.ContentEnd);
                            range.Load(stream, DataFormats.Rtf);
                            LicenseRichTextBox.Visibility = Visibility.Visible;
                            LicenseTextBox.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            ShowFallbackText();
                        }
                    }
                }
                else
                {
                    string text = _step.GetLicenseText();
                    if (!string.IsNullOrEmpty(text))
                    {
                        LicenseTextBox.Text = text;
                        LicenseTextBox.Visibility = Visibility.Visible;
                        LicenseRichTextBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ShowFallbackText();
                    }
                }
            }
            catch (Exception ex)
            {
                LicenseTextBox.Text = "Failed to load license content: " + ex.Message;
                LicenseTextBox.Visibility = Visibility.Visible;
                LicenseRichTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowFallbackText()
        {
            LicenseTextBox.Text = "Please read and accept the license agreement to continue.";
            LicenseTextBox.Visibility = Visibility.Visible;
            LicenseRichTextBox.Visibility = Visibility.Collapsed;
        }

        private void AcceptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsNextEnabled = true;
        }

        private void AcceptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsNextEnabled = false;
        }

        public override void OnNext()
        {
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


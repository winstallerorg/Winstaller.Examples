using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Winstaller.Interfaces;

namespace Winstaller.CustomSteps;

public partial class TopBannerControl : UserControl
{
    public TopBannerControl()
    {
        InitializeComponent();
        Loaded += TopBannerControl_Loaded;
    }

    private void TopBannerControl_Loaded(object sender, RoutedEventArgs e)
    {
        TryLoadResourceBanner();
    }

    public void TryLoadResourceBanner()
    {
        var parentStepView = FindParentStepView(this);
        if (parentStepView?.Step?.Context != null)
        {
            LoadBannerFromContext(parentStepView.Step.Context);
        }
    }

    public void LoadBannerFromContext(InstallerContext context)
    {
        try
        {
            var resourceProvider = context.GetService<IResourceProvider>();
            if (resourceProvider != null && resourceProvider.HasResource("banner"))
            {
                byte[] bytes = resourceProvider.GetResourceBytes("banner");
                if (bytes != null && bytes.Length > 0)
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();

                        BannerImage.Source = bitmap;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load graphic resource banner: {ex.Message}");
        }
    }

    private static StepViewBase? FindParentStepView(DependencyObject element)
    {
        while (element != null)
        {
            if (element is StepViewBase stepView)
            {
                return stepView;
            }
            element = System.Windows.Media.VisualTreeHelper.GetParent(element);
        }
        return null;
    }
}

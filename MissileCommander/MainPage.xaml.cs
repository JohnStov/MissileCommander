using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MissileCommander
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture;
        private readonly DisplayRequest displayRequest = new DisplayRequest();

        public MainPage()
        {
            this.InitializeComponent();

            Application.Current.Suspending += Application_Suspending;
            Application.Current.Resuming += Application_Resuming;
        }

        private async void Application_Resuming(object sender, object e)
        {
            await ConfigureCameraAsync();
        }

        private async void Application_Suspending(object sender, SuspendingEventArgs e)
        {
            await CleanupCameraAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ConfigureCameraAsync();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupCameraAsync();
        }

        private async Task ConfigureCameraAsync()
        {
            if (mediaCapture == null)
            {
                var camera = await FindCameraAsync();
                if (camera != null)
                {
                    mediaCapture = new MediaCapture();
                    await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = camera.Id
                    });
                }

                await StartPreviewAsync();
            }
        }

        private async Task StartPreviewAsync()
        {
            displayRequest.RequestActive();
            PreviewControl.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
        }

        private async Task<DeviceInformation> FindCameraAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return devices.FirstOrDefault(x => x.Name == "Microsoft® LifeCam HD-3000");
        }

        private Task CleanupCameraAsync()
        {
            if (mediaCapture != null)
            {
                mediaCapture.Dispose();
                mediaCapture = null;
            }

            return Task.CompletedTask;
        }
    }
}

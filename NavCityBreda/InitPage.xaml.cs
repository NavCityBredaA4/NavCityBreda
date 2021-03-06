﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NavCityBreda
{
    public sealed partial class InitPage : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;

        public InitPage(SplashScreen splashscreen)
        {
            this.InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += ExtendedSplash_OnResize;

            splash = splashscreen;

            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, Object>(DismissedEventHandler);

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        void PositionImage()
        {
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y - 21);
            extendedSplashImage.Height = splashImageRect.Height / scaleFactor;
            extendedSplashImage.Width = splashImageRect.Width / scaleFactor;
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;
            AwaitInitialize();
        }

        private async void AwaitInitialize()
        {
            while (App.RouteManager.Status == Model.RouteManager.RouteStatus.LOADING)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(150));
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    splashProgressText.Text = App.RouteManager.LoadingElement;
                });
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                // Navigate to mainpage
                App.rootFrame = rootFrame;
                rootFrame.Navigate(typeof(MainPage));
            });

        }
    }
}

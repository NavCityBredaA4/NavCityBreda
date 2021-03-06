﻿using NavCityBreda.Model;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace NavCityBreda
{
    sealed partial class App : Application
    {

        public static Frame rootFrame;


        // =======================
        //      SINGLETONS
        // =======================
        private static GeoTracker geo = new GeoTracker();

        public static GeoTracker Geo
        {
            get
            {
                return geo;
            }
        }

        private static RouteManager rm = new RouteManager();

        public static RouteManager RouteManager
        {
            get
            {
                return rm;
            }
        }

        private static CompassTracker cm = new CompassTracker();

        public static CompassTracker CompassTracker
        {
            get
            {
                return cm;
            }
        }

        public static CoreDispatcher Dispatcher
        {
            get
            {
                return Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            }
        }


        // =========================
        // STATIC HELPER FUNCTIONS
        // =========================

        public static Size ScreenSize
        {
            get
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                Size size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
                return size;
            }
        }

        public static MainPage MainPage
        {
            get
            {
                Frame f = Window.Current.Content as Frame;
                MainPage mp = f.Content as MainPage;
                return mp;
            }
        }



        // ===============================
        // NORMAL STUFF
        // ===============================


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            ApplicationLanguages.PrimaryLanguageOverride = "en";

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    InitPage extendedSplash = new InitPage(e.SplashScreen);
                    rootFrame.Content = extendedSplash;
                    Window.Current.Content = rootFrame;
                }
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

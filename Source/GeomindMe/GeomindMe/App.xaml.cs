﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

namespace GeomindMe
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Use property to check if application is in Trial mode
        /// </summary>
        public static bool IsTrial
        {
            get;
            // setting the IsTrial property from outside is not allowed
            private set;
        }

        private void DetermineIsTrail()
        {
#if TRIAL
    // return true if debugging with trial enabled (DebugTrial configuration is active)
    IsTrial = true;
#else
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();
            IsTrial = license.IsTrial();
#endif
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = false;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            DetermineIsTrail();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            DetermineIsTrail();

            if (e.IsApplicationInstancePreserved)
            {
                //app is returning from dormant state
                return;
            }


            ViewModels.ViewModelLocator locator = Resources["Locator"] as ViewModels.ViewModelLocator;
            if (locator == null)
            {
                throw new NullReferenceException("locator is null!");
            }

            locator.LoadDataFromApplicationState();

            //Load currentUri state
            LoadCurrentUriFromApplicationState();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            ViewModels.ViewModelLocator locator = Resources["Locator"] as ViewModels.ViewModelLocator;
            if (locator == null)
            {
                throw new NullReferenceException("locator is null!");
            }

            locator.SaveDataToApplicationState();

            //Save currentUri state
            SaveCurrentUriToApplicationState();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            var webException = e.ExceptionObject as WebException;
            if (webException != null)
            {
                MessageBox.Show("No internet connection!");
                e.Handled = true;
            }

            if ((!e.Handled) && (e.ExceptionObject is Exception))
            {
                MessageBox.Show("Unexpected error!");
                e.Handled = true;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Loaded += new RoutedEventHandler(RootFrame_Loaded);
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        private string _lastUri = string.Empty;
        void RootFrame_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastUri))
            {
                RootFrame.Navigate(new Uri(_lastUri, UriKind.RelativeOrAbsolute));
            }
        }

        private static readonly string CurrentUriKey = "CurrentUri";
        public void SaveCurrentUriToApplicationState()
        {
            string currentUri = RootFrame.CurrentSource.OriginalString;
            PhoneApplicationService.Current.State[CurrentUriKey] = currentUri;

        }

        public void LoadCurrentUriFromApplicationState()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey(CurrentUriKey))
            {
                return;
            }

            string currentUri = PhoneApplicationService.Current.State[CurrentUriKey] as string;
            if (string.IsNullOrEmpty(currentUri))
            {
                return;
            }

            _lastUri = currentUri;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
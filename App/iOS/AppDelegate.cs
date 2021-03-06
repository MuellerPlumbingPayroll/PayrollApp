﻿using System;
using Foundation;
using Timecard.Services;
using UIKit;

namespace Timecard.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            FirebaseManager.Configure();

            App.Initialize();

            #if ENABLE_TEST_CLOUD
                Xamarin.Calabash.Start();
            #endif

            GoogleUserInfo userInfo = GoogleUserInfo.ReadFromDevice().Result;

            if (userInfo != null)
            {
                var mainStoryboard = UIStoryboard.FromName("Main", null);
                var tabBarController = mainStoryboard.InstantiateViewController("tabViewController") as TabBarController;

                var navigationController = tabBarController.ViewControllers[0] as UINavigationController;
                var homeViewController = navigationController.ViewControllers[0] as HomeViewController;

                homeViewController.ViewModel = new ViewModels.HomeViewModel(userInfo.GivenName);

                // Set the tab bar controller as root
                Window.RootViewController = tabBarController;
            }

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
            var uri_netfx = new Uri(url.AbsoluteString);

            AuthViewController.Auth?.OnPageLoading(uri_netfx);

            return true;
        }
    }
}

﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Data.Entity;
using ToastmasterTools.UWP.Views;
using ToastmasterTools.Core.Features.Feedback;
using ToastmasterTools.Core.Features.Storage;
using ToastmasterTools.Core.ViewModels;

namespace ToastmasterTools.UWP
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Bootstrapper
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Cache
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-BackButton
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplashScreen
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    /// https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-NavigationService

    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
            CacheMaxDuration = TimeSpan.FromDays(2);
            ShowShellBackButton = true;
            SplashFactory = e => new Splash(e);
            try
            {
                using (var db = new ToastmasterContext())
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception exception)
            {
            }
        }

        // runs even if restored from state
        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var keys = PageKeys<Pages>();
            keys.Clear();
            keys.Add(Pages.Home, typeof(HomeView));
            keys.Add(Pages.Settings, typeof(SettingsView));
            keys.Add(Pages.Timer, typeof(TimerView));
            keys.Add(Pages.Login, typeof(LoginView));
            keys.Add(Pages.AhCounter, typeof(AHCounterView));
            keys.Add(Pages.Grammarian, typeof(GrammarView));
            return Task.CompletedTask;
        }

        public override async Task OnSuspendingAsync(object s, SuspendingEventArgs e, bool prelaunchActivated)
        {
            await NavigationService.SaveNavigationAsync();
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var feedbackCollector = SimpleIoc.Default.GetInstance<IFeedbackCollector>();
            await feedbackCollector.CheckForFeedback();
            NavigationService.Navigate(Pages.Login);
        }
    }
}
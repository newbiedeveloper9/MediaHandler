using fbchat_sharp.API;
using MediaHandler.Extensions;
using MediaHandler.Interfaces;
using MediaHandler.Services;
using MediaHandler.ViewModels;

namespace MediaHandler {
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;

    public class AppBootstrapper : BootstrapperBase {
        SimpleContainer container;

        public AppBootstrapper() {
            Initialize();
        }

        protected override void Configure() {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            container.Singleton<FbClient, FbClient>();
            container.Singleton<IFbService, FbService>();
            container.PerRequest<IFbThreadService, FbThreadService>();

            container.PerRequest<IShell, ShellViewModel>();
            container.PerRequest<IThread, ThreadViewModel>();
        }

        protected override object GetInstance(Type service, string key) {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) {
            FixWpfHeader.UnsetRestrictedHeaders();
            DisplayRootViewFor<IShell>();
        }
    }
}
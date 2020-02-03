using System.Windows.Input;
using MediaHandler.Extensions;
using MediaHandler.Input;
using MediaHandler.Interfaces;
using MediaHandler.Services;
using MediaHandler.ViewModels;
using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace MediaHandler {
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
            container.Singleton<IFbLocalProfile, FbLocalProfile>();

            container.PerRequest<IShell, ShellViewModel>();
            container.PerRequest<IThread, ThreadViewModel>();

            KeysAndGestures();
        }

        private void KeysAndGestures()
        {
            var defaultCreateTrigger = Parser.CreateTrigger;

            //Create key gestures like ctrl+k
            Parser.CreateTrigger = (target, triggerText) =>
            {
                if (triggerText == null)
                {
                    return defaultCreateTrigger(target, null);
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                switch (splits[0])
                {
                    case "Key":
                        var key = (Key)Enum.Parse(typeof(Key), splits[1], true);
                        return new KeyTrigger { Key = key };

                    case "Gesture":
                        var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(splits[1]);
                        return new KeyTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };
                }

                return defaultCreateTrigger(target, triggerText);
            };
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
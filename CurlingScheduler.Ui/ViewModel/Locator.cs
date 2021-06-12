using CommonServiceLocator;
using CurlingScheduler.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.ServiceLocation;

namespace CurlingScheduler.Ui.ViewModel
{
    public class Locator
    {
        public Locator()
        {
            var container = new UnityContainer();

            container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<ScheduleCreator>(new ContainerControlledLifetimeManager());

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}

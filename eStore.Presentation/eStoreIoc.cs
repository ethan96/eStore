using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace eStore.Presentation
{
    public class eStoreIoc
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() => {
            var _container = new UnityContainer();
            RegisterTypes(_container);
            return _container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        private static void RegisterTypes(UnityContainer _container)
        {
            _container.LoadConfiguration("defaultContainer");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return GetConfiguredContainer().Resolve<T>();
        }
    }
}

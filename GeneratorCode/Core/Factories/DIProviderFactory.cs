using System;
using System.Collections.Generic;
using System.Linq;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.DependencyInjection;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// Factory لموفري Dependency Injection
    /// </summary>
    public class DIProviderFactory : IDIProviderFactory
    {
        private readonly Dictionary<DIContainerType, Func<IDependencyInjectionProvider>> _providers;
        
        public DIProviderFactory()
        {
            _providers = new Dictionary<DIContainerType, Func<IDependencyInjectionProvider>>();
            RegisterDefaultProviders();
        }
        
        private void RegisterDefaultProviders()
        {
            _providers[DIContainerType.MicrosoftDI] = () => new MicrosoftDIProvider();
            _providers[DIContainerType.Autofac] = () => new AutofacProvider();
            // يمكن إضافة موفري DI أخرى هنا
        }
        
        public IDependencyInjectionProvider CreateProvider(DIContainerType containerType)
        {
            if (_providers.TryGetValue(containerType, out var factory))
            {
                return factory();
            }
            
            return null;
        }
        
        public List<IDependencyInjectionProvider> GetSupportedProviders()
        {
            return _providers.Values.Select(factory => factory()).ToList();
        }
        
        public List<DIContainerType> GetSupportedContainerTypes()
        {
            return _providers.Keys.ToList();
        }
        
        public bool IsContainerTypeSupported(DIContainerType containerType)
        {
            return _providers.ContainsKey(containerType);
        }
        
        public void RegisterProvider(IDependencyInjectionProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
                
            _providers[provider.ContainerType] = () => provider;
        }
    }
} 
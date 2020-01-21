using Aiursoft.Scanner.Interfaces;
using Aiursoft.Scanner.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Aiursoft.Scanner
{
    public static class Extends
    {
        private static void AddScanned(
            Type service,
            Type condition,
            Action<Type, Type> abstractImplementation,
            Action<Type> realisticImplementation,
            params Type[] abstracts)
        {
            if (!service.GetInterfaces().Contains(condition))
            {
                return;
            }
            foreach (var inputInterface in abstracts.Where(t => t.IsInterface))
            {
                if (service.GetInterfaces().Any(t => t == inputInterface))
                {
                    abstractImplementation(inputInterface, service);
                    Console.WriteLine($"Service: {service.Name} - was successfully registered as a {inputInterface.Name} service.");
                }
            }
            foreach (var inputAbstractClass in abstracts.Where(t => t.IsAbstract))
            {
                if (service.IsSubclassOf(inputAbstractClass))
                {
                    abstractImplementation(inputAbstractClass, service);
                }
            }
            realisticImplementation(service);
            Console.WriteLine($"Service: {service.Name} - was successfully registered as a service.");
        }

        public static IServiceCollection AddScannedDependencies(this IServiceCollection services, params Type[] abstracts)
        {
            var executingTypes = new ClassScanner().AllAccessiableClass(false, false);
            foreach (var item in executingTypes)
            {
                AddScanned(item, typeof(ISingletonDependency), (@abstract, implement) => services.AddSingleton(@abstract, implement), (implement) => services.AddSingleton(implement), abstracts);
                AddScanned(item, typeof(IScopedDependency), (@abstract, implement) => services.AddScoped(@abstract, implement), (implement) => services.AddScoped(implement), abstracts);
                AddScanned(item, typeof(ITransientDependency), (@abstract, implement) => services.AddTransient(@abstract, implement), (implement) => services.AddTransient(implement), abstracts);
            }
            return services;
        }
    }
}

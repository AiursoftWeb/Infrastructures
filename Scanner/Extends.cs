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
                AddScanned(item, typeof(ISingletonDependency), (i, e) => services.AddSingleton(i, e), (i) => services.AddSingleton(i), abstracts);
                AddScanned(item, typeof(IScopedDependency), (i, e) => services.AddScoped(i, e), (i) => services.AddScoped(i), abstracts);
                AddScanned(item, typeof(ITransientDependency), (i, e) => services.AddTransient(i, e), (i) => services.AddTransient(i), abstracts);
            }
            return services;
        }
    }
}

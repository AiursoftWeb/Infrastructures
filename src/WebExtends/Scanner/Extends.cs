using Aiursoft.Scanner.Interfaces;
using Aiursoft.Scanner.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aiursoft.Scanner
{
    public static class Extends
    {
        private static void AddScanned(
            Type service,
            Type condition,
            Action<Type, Type> abstractImplementation,
            Action<Type> realisticImplementation,
            IServiceCollection services,
            params Type[] abstracts)
        {
            if (!service.GetInterfaces().Contains(condition))
            {
                return;
            }
            foreach (var inputInterface in abstracts.Where(t => t.IsInterface))
            {
                if (service.GetInterfaces().All(t => t != inputInterface)) continue;
                if (services.Any(t => t.ServiceType == service && t.ImplementationType == inputInterface)) continue;
                abstractImplementation(inputInterface, service);
                Console.WriteLine($"Service:\t{service.Name}\t\t{service.Assembly.FullName?.Split(',')[0]}\t\tsuccess as\t{inputInterface.Name}");
            }
            foreach (var inputAbstractClass in abstracts.Where(t => t.IsAbstract))
            {
                if (!service.IsSubclassOf(inputAbstractClass)) continue;
                if (services.Any(t => t.ServiceType == service && t.ImplementationType == inputAbstractClass))
                    continue;
                abstractImplementation(inputAbstractClass, service);
                Console.WriteLine($"Service:\t{service.Name}\t\t{service.Assembly.FullName?.Split(',')[0]}\t\tsuccess as\t{inputAbstractClass.Name}");
            }

            if (services.Any(t => t.ServiceType == service && t.ImplementationType == service)) return;
            realisticImplementation(service);
            Console.WriteLine($"Service:\t{service.Name}\t\t{service.Assembly.FullName?.Split(',')[0]}\t\tsuccess");
        }

        private static void Register(List<Type> types, IServiceCollection services, params Type[] abstracts)
        {
            foreach (var item in types)
            {
                AddScanned(item, typeof(ISingletonDependency), (i, e) => services.AddSingleton(i, e), (i) => services.AddSingleton(i), services, abstracts);
                AddScanned(item, typeof(IScopedDependency), (i, e) => services.AddScoped(i, e), (i) => services.AddScoped(i), services, abstracts);
                AddScanned(item, typeof(ITransientDependency), (i, e) => services.AddTransient(i, e), (i) => services.AddTransient(i), services, abstracts);
            }
        }

        /// <summary>
        /// Scan all dependencies from the highest level. Very useful when you are building a project.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="abstracts"></param>
        /// <returns></returns>
        public static IServiceCollection AddScannedDependencies(this IServiceCollection services, params Type[] abstracts)
        {
            var executingTypes = new ClassScanner().AllAccessibleClass(false, false);
            Register(executingTypes, services, abstracts);
            return services;
        }

        /// <summary>
        /// Scan all class from the calling assembly. Very useful when you are building a library.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="abstracts"></param>
        /// <returns></returns>
        public static IServiceCollection AddLibraryDependencies(this IServiceCollection services, params Type[] abstracts)
        {
            var calling = Assembly.GetCallingAssembly();
            var executingTypes = new ClassScanner().AllClassUnder(calling, false, false);
            Register(executingTypes, services, abstracts);
            return services;
        }

        public static IServiceCollection AddAssemblyDependencies(this IServiceCollection services, Assembly entry, params Type[] abstracts)
        {
            var executingTypes = new ClassScanner().AllClassUnder(entry, false, false);
            Register(executingTypes, services, abstracts);
            return services;
        }
    }
}
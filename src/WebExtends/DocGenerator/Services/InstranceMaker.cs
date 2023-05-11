using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aiursoft.DocGenerator.Attributes;
using Newtonsoft.Json;

namespace Aiursoft.DocGenerator.Services;

public static class InstanceMaker
{
    private static IList GetArrayWithInstanceInherts(Type itemType)
    {
        var listType = typeof(List<>);
        var constructedListType = listType.MakeGenericType(itemType);
        var instance = (IList)Activator.CreateInstance(constructedListType);
        if (instance == null)
        {
            return new List<object>();
        }

        if (!itemType.IsAbstract)
        {
            instance.Add(Make(itemType));
        }

        foreach (var item in Assembly.GetEntryAssembly()?.GetTypes().Where(t => !t.IsAbstract)
                     .Where(t => t.IsSubclassOf(itemType)) ?? new List<Type>())
        {
            instance.Add(Make(item));
        }

        return instance;
    }

    private static object GenerateWithConstructor(Type type)
    {
        // Has default constructor.
        if (type.GetConstructors().Length == 1 &&
            !type.GetConstructors()[0].GetParameters().Any() &&
            !type.IsAbstract)
        {
            return Assembly.GetAssembly(type)?.CreateInstance(type.FullName ?? string.Empty);
        }

        if (type.GetConstructors().Any(t => t.IsPublic) && !type.IsAbstract)
        {
            // Has a constructor, and constructor has some arguments.
            var constructor = type.GetConstructors()[0];
            var args = constructor.GetParameters();
            var parameters = new object[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var requirement = args[i].ParameterType;
                parameters[i] = Make(requirement);
            }

            return Assembly.GetAssembly(type)?.CreateInstance(type.FullName ?? string.Empty, true, BindingFlags.Default,
                null, parameters, null, null);
        }

        if (type.IsAbstract)
        {
            return null;
        }

        if (!type.GetConstructors().Any(t => t.IsPublic))
        {
            return null;
        }

        return Assembly.GetAssembly(type)?.CreateInstance(type.FullName ?? string.Empty);
    }

    public static object Make(this Type type)
    {
        if (type == typeof(string))
        {
            return "an example string.";
        }

        if (type == typeof(int) || type == typeof(int?))
        {
            return 0;
        }

        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            return DateTime.UtcNow;
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return Guid.NewGuid();
        }

        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
        {
            return DateTimeOffset.UtcNow;
        }

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
        {
            return TimeSpan.FromMinutes(37);
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            return true;
        }

        // List
        if (type.IsGenericType && type.GetGenericTypeDefinition().GetInterfaces()
                .Any(t => t.IsAssignableFrom(typeof(IEnumerable))))
        {
            var itemType = type.GetGenericArguments()[0];
            return GetArrayWithInstanceInherts(itemType);
        }
        // Array

        if (type.GetInterface(typeof(IEnumerable<>).FullName ?? string.Empty) != null)
        {
            var itemType = type.GetElementType();
            var list = GetArrayWithInstanceInherts(itemType);
            var array = Array.CreateInstance(itemType ?? typeof(string[]), list.Count);
            list.CopyTo(array, 0);
            return array;
        }

        var instance = GenerateWithConstructor(type);
        if (instance == null)
        {
            return null;
        }

        foreach (var property in instance.GetType().GetProperties())
        {
            if (property.CustomAttributes.Any(t => t.AttributeType == typeof(JsonIgnoreAttribute)) &&
                property.SetMethod != null)
            {
                property.SetValue(instance, null);
            }
            else if (property.CustomAttributes.Any(t => t.AttributeType == typeof(InstanceMakerIgnore)) &&
                     property.SetMethod != null)
            {
                property.SetValue(instance, null);
            }
            else if (property.SetMethod != null && property.SetMethod != null)
            {
                property.SetValue(instance, Make(property.PropertyType));
            }
        }

        return instance;
    }
}
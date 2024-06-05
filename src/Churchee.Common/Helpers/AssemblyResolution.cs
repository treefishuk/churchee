using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Churchee.Common.Abstractions.Extensibility;
using Microsoft.Extensions.DependencyModel;

public static class AssemblyResolution
{
    public static IEnumerable<Assembly> GetAssemblies()
    {
        return DependencyContext.Default.RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
            .Where(w => w.FullName.StartsWith("Churchee"))
            .Select(Assembly.Load)
            .Distinct()
            .ToList();
    }

    public static Assembly[] GetModuleAssemblies()
    {
        var returnAssemblies = GetAssemblies()
            .Where(w => w.FullName.StartsWith("Churchee") && w.GetTypes().Any(a => a.GetInterfaces().Contains(typeof(IModule))));

        return returnAssemblies.ToArray();
    }



}

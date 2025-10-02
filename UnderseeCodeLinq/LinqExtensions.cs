using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace UnderseeCodeLinq;

public static class LinqExtensions {
    public static List<Type> GetAllTypesFromDll(this System.Reflection.Assembly assembly)
    {
        return assembly.GetForwardedTypes().Concat(assembly.GetExportedTypes()).ToList();
    }

    public static IEnumerable<Type> GetAllTypesThatHaveAttribute(this IEnumerable<Type> allTypes, Type targetAttribute)
    {
        var allTypesThatHaveAttribute =
            allTypes.Where(t => t.GetCustomAttributes().Any(a => a.TypeId == targetAttribute));
        return allTypesThatHaveAttribute;
    }

    public static IEnumerable<Type> GetAllTypesThatHaveAttribute(this IEnumerable<Type> allTypes, string attributeName)
    {
        var targetAttribute = allTypes.FirstOrDefault(t => t.Name == attributeName);
        return GetAllTypesThatHaveAttribute(allTypes, targetAttribute);
    }

    public static IEnumerable<MethodInfo> GetAllMethodsThatHaveAttribute(this IEnumerable<Type> allTypes, Type targetAttribute)
    {
        return GetAllMethodsThatHaveAttribute(allTypes.SelectMany(t => t.GetMethods()), targetAttribute);
    }

    public static IEnumerable<MethodInfo> GetAllMethodsThatHaveAttribute(this IEnumerable<MethodInfo> allMethods, Type targetAttribute)
    {
        var methodsWithTheAttribute = allMethods.Where(m => m.CustomAttributes.Any(a => a.AttributeType == targetAttribute));
        return methodsWithTheAttribute;
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnderseeCodeLinq;

namespace UnderseeCodeLinq;

public class Program {
    public static async Task Main(string[] args)
    {
        DependencyInjectionGrouping grouping = new();
        var csv = grouping.CreateParameterTypeUsageMatrix(Assembly.GetExecutingAssembly(), "UnderseeCodeLinq.Example");
        await File.WriteAllTextAsync("results.csv", csv.ToString());
    }

    public static async Task<IEnumerable<Type>> FindNamespacesForTypeName(IEnumerable<Assembly> searchSpace, string typeName, bool exactMatch, string? namespacePrefix = null) {
        var allTypes = searchSpace.SelectMany(a => a.GetAllTypesFromDll());
        IEnumerable<Type> typesWithNameSearch = exactMatch ? 
            allTypes.Where(t => t.Name == typeName) : // case sensitive on purpose 
            allTypes.Where(t => t.Name.Contains(typeName, StringComparison.InvariantCultureIgnoreCase));

        if (!string.IsNullOrWhiteSpace(namespacePrefix)) {
            typesWithNameSearch = typesWithNameSearch.Where(t => t.Namespace.StartsWith(namespacePrefix));
        }

        return typesWithNameSearch;
    }

    public static async Task GetTypeByAttributeTest() {
        Type targetType = typeof(TypeInfo);
        var assembly = Assembly.GetAssembly(targetType);
        var allTypes = assembly.GetAllTypesFromDll();

        var typeWithAssemblyTitle = allTypes.GetAllTypesThatHaveAttribute("AssemblyTitleAttribute");
        foreach (var type in typeWithAssemblyTitle)
        {
            Console.WriteLine(type.FullName);
        }
    }
}

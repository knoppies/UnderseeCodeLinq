using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace UnderseeCodeLinq;

public class DependencyInjectionGrouping {
    public StringBuilder CreateParameterTypeUsageMatrix(Assembly targetAssembly, string namespaceSearch) {
        var allTargetTypes = targetAssembly.GetAllTypesFromDll().Where(t => t.Namespace.StartsWith(namespaceSearch));
        int nextCol = 0;
        ConcurrentDictionary<string, int> columnIndex = new();
        StringBuilder csv = new();
        foreach (var type in allTargetTypes) {
            int constructorNumber = 0;
            foreach (var constructorInfo in type.GetConstructors()) {
                string constructorName = type.Name + "-" + constructorNumber;
                ConcurrentDictionary<int, int> thisLineParameterIndex = GetParameterIndexLocations(constructorInfo, columnIndex, ref nextCol);
                csv.Append(constructorName).Append(",");
                if (thisLineParameterIndex.Any()) {
                    PrintParametersToCsv(thisLineParameterIndex, csv);
                }
                csv.AppendLine();
            }
        }
        AddColumnNamesToCsv(columnIndex, csv);

        return csv;
    }

    public void GetAllConstructorInfo(Assembly targetAssembly, string namespaceSearch) {
        var allTargetTypes = targetAssembly.GetAllTypesFromDll().Where(t => t.Namespace.StartsWith(namespaceSearch));
        var extracted = allTargetTypes.SelectMany(t => t.GetConstructors().Select(c => new ConstructionParameters() {
            ConstructedType = t,
            Constructor = c,
            Parameters = c.GetParameters().Select(p => p.ParameterType).OrderBy(p => p.Name).ToList(),
        }));
    }

    private static void AddColumnNamesToCsv(ConcurrentDictionary<string, int> columnIndex, StringBuilder csv) {
        StringBuilder columnNames = new();
        columnNames.Append("Type Name");
        int lastIndex = -1;
        foreach (var colName in columnIndex.OrderBy(ci => ci.Value)) {
            if (lastIndex + 1 != colName.Value) {
                throw new Exception("Column index is not sequential");
            }
            lastIndex = colName.Value;
            columnNames.Append(colName.Key).Append(",");
        }
        csv.Insert(0, columnNames);
    }

    private static void PrintParametersToCsv(ConcurrentDictionary<int, int> thisLineParameterIndex, StringBuilder csv) {
        int paramCount = thisLineParameterIndex.Keys.Max();
        for (int i = 0; i < paramCount + 1; i++) {
            csv.Append(thisLineParameterIndex.TryGetValue(i, out int value) ? value : string.Empty).Append(",");
        }
    }

    private static ConcurrentDictionary<int, int> GetParameterIndexLocations(ConstructorInfo constructorInfo,
        ConcurrentDictionary<string, int> columnIndex, ref int nextCol) {
        int localNextCol = nextCol;
        ConcurrentDictionary<int, int> thisLineParameterIndex = new();
        foreach (var arg in constructorInfo.GetParameters()) {
            //TODO: need to deal with generic types
            int index = columnIndex.GetOrAdd(arg.ParameterType.FullName, (_) => localNextCol++);
            thisLineParameterIndex.AddOrUpdate(index, 1, (key, oldValue) => oldValue + 1);
        }
        nextCol = localNextCol;
        return thisLineParameterIndex;
    }

    ConcurrentDictionary<string, List<ConstructionParameters>> parameters = new ConcurrentDictionary<string, List<ConstructionParameters>>();
}

public class ConstructionParameters {
    public Type ConstructedType { get; set; }
    public ConstructorInfo Constructor { get; set; }
    public List<Type> Parameters { get; set; }
}
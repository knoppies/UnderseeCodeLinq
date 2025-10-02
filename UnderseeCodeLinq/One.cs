namespace UnderseeCodeLinq.Example.Numbers;

///<summary>These are just examples of objects with multiple constructors</summary>
public class One {
    public One() { }
    public One(string name) { }
    public One(DependencyInjectionGrouping grouping) { }
    public One(string name, DependencyInjectionGrouping grouping) { }
    public One(DependencyInjectionGrouping grouping, string message) { }
}

public class Two {
    public Two(One one) { }
    public Two(One one, string message) { }
    public Two(One one, string message, int number) { }
    public Two(One one, DependencyInjectionGrouping grouping, string message) { }
}
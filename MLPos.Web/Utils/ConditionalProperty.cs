namespace MLPos.Web.Utils;

public static class ConditionalProperty
{
    public static IDictionary<string, object> ConditionalProp(this object obj, bool condition, string name, object value)
    {
        var items= !condition ? new RouteValueDictionary(obj) : new RouteValueDictionary(obj) {{name, value}};
        return UnderlineToDashInDictionaryKeys(items);
    }
    
    private static IDictionary<string, object> UnderlineToDashInDictionaryKeys(IDictionary<string,object> items)
    {
        var newItems = new RouteValueDictionary();
        foreach (var item in items)
        {
            newItems.Add(item.Key.Replace("_", "-"), item.Value);
        }
        return (IDictionary<string, object>)newItems;
    }
}
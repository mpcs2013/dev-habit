using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace DevHabit.Api.Services;

public sealed class DataShapingService
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesCache = new();

    public ExpandoObject ShapeData<T>(T entity, string? fields)
    {
        HashSet<string> fieldsSet = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        // Get or add the property infos for type T from the cache - or retrieve them if already cached
        // This improves performance by avoiding repeated reflection calls
        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            );

        if (fieldsSet.Any())
        {
            propertyInfos = propertyInfos
            .Where(pi => fieldsSet.Contains(pi.Name))
            .ToArray();
        }

        IDictionary<string, object?> shapedObject = new ExpandoObject();

        // apply data shaping
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            shapedObject[propertyInfo.Name] = propertyInfo.GetValue(entity);
        }

        return (ExpandoObject)shapedObject;
    }

    public List<ExpandoObject> ShapeCollectionData<T>(IEnumerable<T> entities, string? fields)
    {
        HashSet<string> fieldsSet = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        // Get or add the property infos for type T from the cache - or retrieve them if already cached
        // This improves performance by avoiding repeated reflection calls
        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T), 
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            );

        if(fieldsSet.Any())
        { 
            propertyInfos = propertyInfos
            .Where(pi => fieldsSet.Contains(pi.Name))
            .ToArray();
        }

        List<ExpandoObject> shapedObjects = [];
        foreach (T entity in entities)
        {
            IDictionary<string, object?> shapedObject = new ExpandoObject();

            // apply data shaping
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                shapedObject[propertyInfo.Name] = propertyInfo.GetValue(entity);
            }

            shapedObjects.Add((ExpandoObject)shapedObject);
        }

        return shapedObjects;
    }

    public bool ValidateFields<T>(string? fields)
    {
        if(string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        HashSet<string> fieldsSet = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        // Get or add the property infos for type T from the cache - or retrieve them if already cached
        // This improves performance by avoiding repeated reflection calls
        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            );

        return fieldsSet.All(field => propertyInfos.Any(pi => pi.Name.Equals(field, StringComparison.OrdinalIgnoreCase)));
    }
}

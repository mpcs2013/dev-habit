using System.Linq.Dynamic.Core;

namespace DevHabit.Api.Services.Sorting;

internal static class QueryableExtensions
{ 
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] sortMappings,
        string defaultOrderBy = "Id")
    {
        if(string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        string[] sortFields = sort.Split(',')
            .Select(s => s.Trim())
            .Where (s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var orderByParts = new List<string>();
        foreach (string field in sortFields)
        {
            (string sortfield, bool isDescending) = ParseSortField(field);

            SortMapping mapping = sortMappings.First(m => m.SortField.Equals(sortfield, StringComparison.OrdinalIgnoreCase));

            string direction = (isDescending, mapping.Reverse) switch
            { 
                (false, false) => "ASC",
                (true, false) => "DESC",
                (false, true) => "DESC",
                (true, true) => "ASC"
            };

            orderByParts.Add($"{mapping.PropertyName} {direction}");
        }

        string orderBy = string.Join(",", orderByParts);

        return query.OrderBy(orderBy);
    }

    private static (string SortField, bool IsDescending) ParseSortField(string field)
    {
        string[] parts = field.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string sortField = parts[0];
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return (sortField, isDescending);
    }
}

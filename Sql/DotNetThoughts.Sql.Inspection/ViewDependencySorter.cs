
namespace DotNetThoughts.Sql.Inspection;

internal class ViewDependencySorter : IComparer<Schema.ViewInfo>
{
    List<int> _ordered = new List<int>();
    public ViewDependencySorter(IEnumerable<Schema.DependencyInfo> view_viewDependencies)
    {
        Dictionary<int, HashSet<int>> deps = [];
        foreach (var dependency in view_viewDependencies)
        {
            if (!deps.ContainsKey(dependency.referencing_id))
            {
                deps.Add(dependency.referencing_id, [dependency.referenced_id]);
            }
            else
            {
                deps[dependency.referencing_id].Add(dependency.referenced_id);
            }
            if (!deps.ContainsKey(dependency.referenced_id))
            {
                deps.Add(dependency.referenced_id, []);
            }
        }
        while (deps.Count > 0)
        {
            var hasNoDeps = deps.First(x => x.Value.Count == 0);
            _ordered.Add(hasNoDeps.Key);
            deps.Remove(hasNoDeps.Key);
            foreach (var dependency in deps)
            {
                dependency.Value.Remove(hasNoDeps.Key);
            }
        }
    }

    public int Compare(Schema.ViewInfo? x, Schema.ViewInfo? y)
    {
        if (x == null || y == null)
        {
            throw new Exception("Cant order nulls.");
        }
        var indexOfX = _ordered.IndexOf(x.object_id);
        if (indexOfX < 0) return 1;
        var indexOfY = _ordered.IndexOf(y.object_id);
        if (indexOfY < 0) return -1;
        return indexOfX > indexOfY ? 1 :-1;

    }
}
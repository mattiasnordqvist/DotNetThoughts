
namespace DotNetThoughts.Sql.Inspection;

internal class ViewDependencySorter : IComparer<Schema.ViewInfo>
{
    private IEnumerable<Schema.DependencyInfo> _view_viewDependencies;

    public ViewDependencySorter(IEnumerable<Schema.DependencyInfo> view_viewDependencies)
    {
        _view_viewDependencies = view_viewDependencies;
    }

    public int Compare(Schema.ViewInfo? x, Schema.ViewInfo? y)
    {
        if (x == null || y == null)
        {
            throw new Exception("Cant order nulls.");
        }
        if (x.object_id == y.object_id)
        {
            // x and y are the same
            return 0;
        }
        if (_view_viewDependencies.Any(d => d.referencing_id == x.object_id && d.referenced_id == y.object_id))
        {
            // x references y
            return 1;
        }
        if (_view_viewDependencies.Any(d => d.referencing_id == y.object_id && d.referenced_id == x.object_id))
        {
            // x is referenced by y
            return -1;
        }
        if ((!_view_viewDependencies.Any(d => d.referencing_id == x.object_id || d.referenced_id == x.object_id)) && (!_view_viewDependencies.Any(d => d.referencing_id == y.object_id || d.referenced_id == y.object_id)))
        {
            // x is not referenced or referencing on anyone
            return 0;
        }
        if (!_view_viewDependencies.Any(d => d.referencing_id == x.object_id || d.referenced_id == x.object_id))
        {
            // x is not referenced or referencing on anyone
            return -1;
        }
        if (!_view_viewDependencies.Any(d => d.referencing_id == y.object_id || d.referenced_id == y.object_id))
        {
            // y is not referenced or referencing on anyone
            return 1;
        }
        return 0;
    }
}
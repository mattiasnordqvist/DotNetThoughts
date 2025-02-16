namespace DotNetThoughts.Sql.Migrations.Tests;
public partial class Tests
{
    public class VersionInfo
    {
        public required long Version { get; set; }
        public required string Name { get; set; }
        public required bool IsSnapshot { get; set; }
    }
}

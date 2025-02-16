using System.Data.Common;

namespace DotNetThoughts.Sql.Migrations.Tests;
public partial class Tests
{
    public record FakeMigration(long Version, string Name, bool IsSnapshot) : IMigration
    {
        public Action<DbConnection, DbTransaction, int> ExecuteImpl { get; set; } = (c, t, commandTimeout) => { };

        public virtual void Execute(DbConnection c, DbTransaction t, int commandTimeout)
        {
            ExecuteImpl(c, t, commandTimeout);
        }
    }
}

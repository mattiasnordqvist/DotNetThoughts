using Xunit;

namespace DotNetThoughts.Sql.Utilities.Tests;

public class ConnectionStringUtilsTests
{
    [Theory]
    [InlineData("Data Source=PINKGOLD\\PINKGOLD16; Integrated Security=SSPI; Initial Catalog=master; TrustServerCertificate=True;MultipleActiveResultSets=False;")]
    [InlineData("TrustServerCertificate=True;Data Source=PINKGOLD\\PINKGOLD16;Integrated Security=SSPI;Initial Catalog=master;MultipleActiveResultSets=False;")]
    public void GetInitialCatalog_HappyCases(string connectionString)
    {
        var initialCatalog = ConnectionStringUtils.GetInitialCatalog(connectionString);

        Assert.Equal("master", initialCatalog);
    }

    [Fact]
    public async Task ReplaceInitialCatalog_HappyCases()
    {
        var connectionString = "Data Source=PINKGOLD\\PINKGOLD16; Integrated Security=SSPI; Initial Catalog=master; TrustServerCertificate=True;MultipleActiveResultSets=False;";
        var replaced = ConnectionStringUtils.ReplaceInitialCatalog(connectionString, "newCatalog");
        await Verify(new
        {
            ConnectionString = connectionString,
            AfterInitialCatalogReplaced = replaced
        });
    }
}

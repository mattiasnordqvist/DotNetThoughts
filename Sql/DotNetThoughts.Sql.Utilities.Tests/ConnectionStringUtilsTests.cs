namespace DotNetThoughts.Sql.Utilities.Tests;

public class ConnectionStringUtilsTests
{
    [Test]
    [Arguments("Data Source=PINKGOLD\\PINKGOLD16; Integrated Security=SSPI; Initial Catalog=master; TrustServerCertificate=True;MultipleActiveResultSets=False;")]
    [Arguments("TrustServerCertificate=True;Data Source=PINKGOLD\\PINKGOLD16;Integrated Security=SSPI;Initial Catalog=master;MultipleActiveResultSets=False;")]
    public async Task GetInitialCatalog_HappyCases(string connectionString)
    {
        var initialCatalog = ConnectionStringUtils.GetInitialCatalog(connectionString);
        await Assert.That(initialCatalog).IsEqualTo("master");
    }

    [Test]
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

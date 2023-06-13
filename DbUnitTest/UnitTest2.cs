namespace DbUnitTest;

using Dapper;

public class UnitTest2
{
    [Fact]
    public async Task Test2()
    {
        var container = await Helper.CreateNewDbContainer();
        var conn = Helper.GetContainerSqlConnection(container);
        await Helper.InsertRandomProducts(conn, 20);
        var products = conn.Query<Product>("SELECT * FROM Product");
        Assert.Equal(20, products.Count());
    }
}
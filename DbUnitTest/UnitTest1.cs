namespace DbUnitTest;

using Dapper;

public class UnitTest1
{    
    [Fact]
    public async Task Test1()
    {
        var container = await Helper.CreateNewDbContainer();
        var conn = Helper.GetContainerSqlConnection(container);
        var products = conn.Query<Product>("SELECT * FROM Product");
        Assert.Equal(0, products.Count());
        await Helper.InsertRandomProducts(conn, 10);
        products = conn.Query<Product>("SELECT * FROM Product");
        Assert.Equal(10, products.Count());        
    }    
}
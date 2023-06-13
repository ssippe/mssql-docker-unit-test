namespace DbUnitTest;

using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Testcontainers.MsSql;

public class Helper
{
    public static async Task<IContainer> CreateNewDbContainer()
    {
        var container = new ContainerBuilder()
          .WithImage("ssippe/mssql-docker-unit-test:latest")
          .WithPortBinding(1433, true)
          .WithEnvironment(new Dictionary<string, string>
              {
                   { "ACCEPT_EULA", "Y" },
                   { "SA_PASSWORD", "123456a@" },
              })
          .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlBuilder.MsSqlPort))
          .Build();

        // Start the container.
        await container.StartAsync()
          .ConfigureAwait(false);
        Debug.WriteLine("Created container " + container.Name);
        return container;
    }
    public static SqlConnection GetContainerSqlConnection(IContainer container) 
    {

        var port = container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);
        var connectionString = GetConnectionString(container.Hostname, port.ToString(), "testdb", "sa", "123456a@");

        var conn = new SqlConnection(connectionString);
        //conn.Open();
        return conn;
    }

    public static async Task InsertRandomProducts(SqlConnection conn, int count)
    {
        var rand = new Random();
        for (int i = 0; i < count; i++)
        {
            await conn.ExecuteAsync("INSERT INTO Product(ProductName) VALUES(@ProductName)", new { ProductName = i.ToString() + " " + rand.Next(1000) });
        }
    }

    static string GetConnectionString(string hostname, string port, string database, string username, string password)
    {
        var properties = new Dictionary<string, string>
        {
            { "Server", hostname + "," + port },
            { "Database", database },
            { "User Id", username },
            { "Password", password },
            { "TrustServerCertificate", bool.TrueString }
        };
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}
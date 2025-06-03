using MotoStore.Api.Models;
using MySql.Data.MySqlClient;
using System.Data;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();

        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT Id, Name, Price, Category FROM Products";

        using MySqlCommand command = new(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price"),
                Category = reader.GetString("Category")
            });
        }

        return products;
    }

    public Product? GetProductById(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT Id, Name, Price, Category FROM Products WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Product
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Price = reader.GetDecimal("Price"),
                Category = reader.GetString("Category")
            };
        }

        return null; // если не найден
    }

    public void AddProduct(Product product)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "INSERT INTO Products (Name, Price, Category) VALUES (@Name, @Price, @Category)";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Category", product.Category);

        command.ExecuteNonQuery();
    }

    public bool UpdateProduct(Product product)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = @"UPDATE Products 
                     SET Name = @Name, Price = @Price, Category = @Category 
                     WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@Id", product.Id);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public bool DeleteProduct(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "DELETE FROM Products WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }
}
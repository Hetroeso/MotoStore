using MotoStore.Api.Models;
using MySql.Data.MySqlClient;

public class OrderItemRepository
{
    private readonly string _connectionString;

    public OrderItemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<OrderItem> GetItemsByOrderId(int orderId)
    {
        var items = new List<OrderItem>();

        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@OrderId", orderId);

        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(new OrderItem
            {
                Id = reader.GetInt32("Id"),
                OrderId = reader.GetInt32("OrderId"),
                ProductId = reader.GetInt32("ProductId"),
                Quantity = reader.GetInt32("Quantity"),
                UnitPrice = reader.GetDecimal("UnitPrice")
            });
        }

        return items;
    }

    public void AddOrderItem(OrderItem item)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                         VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@OrderId", item.OrderId);
        command.Parameters.AddWithValue("@ProductId", item.ProductId);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);

        command.ExecuteNonQuery();
    }

    public bool DeleteItem(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "DELETE FROM OrderItems WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }
}

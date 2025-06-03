using MotoStore.Api.Models;
using MySql.Data.MySqlClient;

public class OrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public List<Order> GetAllOrders()
    {
        var orders = new List<Order>();

        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT * FROM Orders";

        using MySqlCommand command = new(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32("Id"),
                CustomerName = reader.GetString("CustomerName"),
                OrderDate = reader.GetDateTime("OrderDate"),
                TotalAmount = reader.GetDecimal("TotalAmount")
            });
        }

        return orders;
    }

    public Order? GetOrderById(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT * FROM Orders WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using MySqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Order
            {
                Id = reader.GetInt32("Id"),
                CustomerName = reader.GetString("CustomerName"),
                OrderDate = reader.GetDateTime("OrderDate"),
                TotalAmount = reader.GetDecimal("TotalAmount")
            };
        }

        return null;
    }

    public void AddOrder(Order order)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = @"INSERT INTO Orders (CustomerName, OrderDate, TotalAmount) 
                         VALUES (@CustomerName, @OrderDate, @TotalAmount)";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

        command.ExecuteNonQuery();
    }

    public bool DeleteOrder(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "DELETE FROM Orders WHERE Id = @Id";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }

    public void AddOrderWithItems(Order order)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // Вставка заказа
            string insertOrder = @"INSERT INTO Orders (CustomerName, OrderDate, TotalAmount)
                               VALUES (@CustomerName, @OrderDate, @TotalAmount);
                               SELECT LAST_INSERT_ID();";

            using MySqlCommand orderCommand = new(insertOrder, connection, transaction);
            orderCommand.Parameters.AddWithValue("@CustomerName", order.CustomerName);
            orderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            orderCommand.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

            var orderId = Convert.ToInt32(orderCommand.ExecuteScalar());
            order.Id = orderId;

            // Вставка позиций заказа
            string insertItem = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                              VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

            foreach (var item in order.Items)
            {
                using MySqlCommand itemCommand = new(insertItem, connection, transaction);
                itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                itemCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public Order? GetOrderWithItemsById(int id)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string orderQuery = "SELECT * FROM Orders WHERE Id = @Id";
        using MySqlCommand orderCmd = new(orderQuery, connection);
        orderCmd.Parameters.AddWithValue("@Id", id);

        using MySqlDataReader reader = orderCmd.ExecuteReader();
        if (!reader.Read())
            return null;

        var order = new Order
        {
            Id = reader.GetInt32("Id"),
            CustomerName = reader.GetString("CustomerName"),
            OrderDate = reader.GetDateTime("OrderDate"),
            TotalAmount = reader.GetDecimal("TotalAmount"),
            Items = new List<OrderItem>()
        };

        reader.Close();

        // Получаем позиции заказа
        string itemsQuery = "SELECT * FROM OrderItems WHERE OrderId = @OrderId";
        using MySqlCommand itemsCmd = new(itemsQuery, connection);
        itemsCmd.Parameters.AddWithValue("@OrderId", id);

        using MySqlDataReader itemsReader = itemsCmd.ExecuteReader();
        while (itemsReader.Read())
        {
            order.Items.Add(new OrderItem
            {
                Id = itemsReader.GetInt32("Id"),
                OrderId = itemsReader.GetInt32("OrderId"),
                ProductId = itemsReader.GetInt32("ProductId"),
                Quantity = itemsReader.GetInt32("Quantity"),
                UnitPrice = itemsReader.GetDecimal("UnitPrice")
            });
        }

        return order;
    }
}
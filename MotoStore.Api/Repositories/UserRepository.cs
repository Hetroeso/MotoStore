using MotoStore.Api.Models;
using MySql.Data.MySqlClient;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public void AddUser(User user)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = @"INSERT INTO Users (Username, PasswordHash, Role)
                         VALUES (@Username, @PasswordHash, @Role)";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@Role", user.Role);

        command.ExecuteNonQuery();
    }

    public User? GetUserByUsername(string username)
    {
        using MySqlConnection connection = new(_connectionString);
        connection.Open();

        string query = "SELECT * FROM Users WHERE Username = @Username";

        using MySqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@Username", username);

        using MySqlDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new User
        {
            Id = reader.GetInt32("Id"),
            Username = reader.GetString("Username"),
            PasswordHash = reader.GetString("PasswordHash"),
            Role = reader.GetString("Role")
        };
    }
}


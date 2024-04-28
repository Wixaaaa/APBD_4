using APBD_Zadanie_6.Models;
using System.Data.SqlClient;

namespace APBD_Zadanie_6.Services
{
    public class WarehouseService(IConfiguration configuration) : IWarehouseService
    {
        public async Task<int> AddProduct(ProductWarehouse productWarehouse)
        {
            var connectionString = configuration.GetConnectionString("Database");
            await using var connection = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand();

            cmd.Connection = connection;

            await connection.OpenAsync();
            cmd.CommandText = "SELECT TOP 1 [Order].IdOrder FROM [Order] " +
                              "LEFT JOIN Product_Warehouse ON [Order].IdOrder = Product_Warehouse.IdOrder " +
                              "WHERE [Order].IdProduct = @IdProduct " +
                              "AND [Order].Amount = @Amount " +
                              "AND Product_Warehouse.IdProductWarehouse IS NULL " +
                              "AND [Order].CreatedAt < @CreatedAt";

            cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
            cmd.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
            cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

            var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("No order found");

            await reader.ReadAsync();
            var idOrder = (int)reader["IdOrder"];
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);

            reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("No product found");

            await reader.ReadAsync();
            var price = (decimal)reader["Price"];
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT IdWarehouse FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            cmd.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);

            reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("No warehouse found");

            await reader.CloseAsync();

            cmd.Parameters.Clear();

            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            cmd.Transaction = transaction;

            try
            {
                cmd.CommandText = "UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
                cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);
                cmd.Parameters.AddWithValue("@IdOrder", idOrder);

                var rowsUpdated = await cmd.ExecuteNonQueryAsync();

                if (rowsUpdated < 1) throw new Exception("Error while updating order");

                cmd.Parameters.Clear();

                cmd.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                                  $"VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*{price}, @CreatedAt)";
                cmd.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
                cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
                cmd.Parameters.AddWithValue("@IdOrder", idOrder);
                cmd.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
                cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

                var rowsInserted = await cmd.ExecuteNonQueryAsync();

                if (rowsInserted < 1) throw new Exception("Error while adding product to warehouse");

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while adding product to warehouse", e);
            }

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

            reader = await cmd.ExecuteReaderAsync();

            await reader.ReadAsync();

            var idProductWarehouse = (int)reader["IdProductWarehouse"];

            await reader.CloseAsync();
            await connection.CloseAsync();

            return idProductWarehouse;
        }
    }
}

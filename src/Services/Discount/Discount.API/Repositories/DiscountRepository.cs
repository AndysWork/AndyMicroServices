using Discount.API.Entities;
using Npgsql;
using Dapper;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _connection;
        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon where ProductName = @ProductName",
                new { ProductName = productName });

            if (coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Description = "No Discount", Amount = 0 };
            }
            else
            {
                return coupon;
            }
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var affected = await _connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new { coupon.ProductName, coupon.Description, coupon.Amount });
            return affected > 0;

        }
        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var updated = await _connection.ExecuteAsync("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount where ID = @Id)",
                new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.ID });
            return updated > 0;
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            var affected = await _connection.ExecuteAsync("DELETE FROM Coupon where ProductName = @ProductName",
                new { ProductName = productName });
            return affected > 0;
        }


    }
}

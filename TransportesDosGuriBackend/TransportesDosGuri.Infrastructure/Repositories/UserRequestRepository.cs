using Dapper;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Infrastructure.Data;

namespace TransportesDosGuri.Infrastructure.Repositories
{
    public class UserRequestRepository : IUserRequestRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRequestRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(UserRequest userRequest)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                INSERT INTO UserRequest
                (
                    Price,
                    DueDate,
                    ApplicationUserId,
                    AsaasSubscriptionId
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Price,
                    @DueDate,
                    @ApplicationUserId,
                    @AsaasSubscriptionId
                );
                """;

            userRequest.Id = await connection.ExecuteScalarAsync<long>(sql, new
            {
                userRequest.Price,
                DueDate = userRequest.DueDate.ToDateTime(TimeOnly.MinValue),
                userRequest.ApplicationUserId,
                userRequest.AsaasSubscriptionId
            });
        }

        public async Task DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                DELETE FROM UserRequest
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<UserRequest>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Price,
                    DueDate,
                    ApplicationUserId,
                    AsaasSubscriptionId
                FROM UserRequest
                ORDER BY Id DESC;
                """;

            return await connection.QueryAsync<UserRequest>(sql);
        }

        public async Task<UserRequest?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Price,
                    DueDate,
                    ApplicationUserId,
                    AsaasSubscriptionId
                FROM UserRequest
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<UserRequest>(sql, new { Id = id });
        }

        public async Task UpdateAsync(UserRequest userRequest)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                UPDATE UserRequest
                SET
                    Price = @Price,
                    DueDate = @DueDate,
                    ApplicationUserId = @ApplicationUserId,
                    AsaasSubscriptionId = @AsaasSubscriptionId
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, userRequest);
        }
    }
}

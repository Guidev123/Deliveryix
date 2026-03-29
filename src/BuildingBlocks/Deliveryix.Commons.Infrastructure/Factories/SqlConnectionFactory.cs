using Microsoft.Data.SqlClient;

namespace Deliveryix.Commons.Infrastructure.Factories
{
    public sealed class SqlConnectionFactory(string connectionString)
    {
        public SqlConnection Create() => new(connectionString);
    }
}
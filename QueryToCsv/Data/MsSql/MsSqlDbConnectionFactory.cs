using System.Data;
using System.Data.SqlClient;

namespace QueryToCsv.Data.MsSql
{
   public class MsSqlDbConnectionFactory : IDbConnectionFactory
   {
      readonly string connectionString;

      public MsSqlDbConnectionFactory(string connectionString)
      {
         this.connectionString = connectionString;
      }
      public IDbConnection CreateConnection()
      {
         return new SqlConnection(connectionString);
      }
   }
}
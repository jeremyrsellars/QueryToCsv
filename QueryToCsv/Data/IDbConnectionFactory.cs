using System.Data;

namespace QueryToCsv.Data
{
   public interface IDbConnectionFactory
   {
      IDbConnection CreateConnection();
   }
}
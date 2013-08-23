using System;
using System.Data;

namespace QueryToCsv.Data
{
   public static class DataHelper
   {
      public static T OpenExecuteClose<T>(this IDbConnectionFactory factory, Func<IDbConnection, IDbCommand> commandCreator, Func<IDbCommand, T> func)
      {
         return factory.ExecuteWithConnection(OpenExecuteClose(commandCreator, func));
      }

      public static T ExecuteWithConnection<T>(this IDbConnectionFactory factory, Func<IDbConnection, T> func)
      {
         using (var connection = factory.CreateConnection())
            return func(connection);
      }

      public static Func<IDbConnection, T> OpenExecuteClose<T>(Func<IDbConnection, IDbCommand> commandCreator, Func<IDbCommand, T> func)
      {
         return
            con => OpenExecuteClose(con, commandCreator, func);
      }

      public static T OpenExecuteClose<T>(this IDbConnection con, Func<IDbConnection, IDbCommand> commandCreator, Func<IDbCommand, T> func)
      {
         using (var command = commandCreator(con))
         {
            command.Connection = con;
            con.Open();
            var value = func(command);
            con.Close();
            return value;
         }
      }
   }
}
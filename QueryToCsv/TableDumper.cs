using System;
using System.Data;
using System.IO;
using QueryToCsv.Data;
using QueryToCsv.Data.MsSql;

namespace QueryToCsv
{
   class TableDumper
   {
      public class Configuration
      {
         public string ConnectionString = "Integrated Security=SSPI; Initial Catalog=NigiriArchive";
         public string QueryText = "Message";
         public string OutputFile;
      }

      public readonly Configuration Config;
      readonly MsSqlDbConnectionFactory db;

      public TableDumper(Configuration config = null)
      {
         Config = config ?? new Configuration();
         db = new MsSqlDbConnectionFactory(config.ConnectionString);
      }

      public void Run()
      {
         var activity = db.OpenExecuteClose(CreateCommand, WriteResults);
         Console.Error.WriteLine("Processed {0} rows.", activity.DataRowCount);
      }

      IDbCommand CreateCommand(IDbConnection connection)
      {
         var cmd = connection.CreateCommand();
         cmd.CommandText = Config.QueryText;
         //cmd.CommandType = CommandType.TableDirect;
         return cmd;
      }

      DataReaderToCsvActivity WriteResults(IDbCommand cmd)
      {
         using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
         using (var writer = GetOutputStreamWriter())
            return DataReaderToCsvActivity.WriteDataTo(rdr, writer);
      }

      TextWriter GetOutputStreamWriter()
      {
         if (string.IsNullOrWhiteSpace(Config.OutputFile))
            return Console.Out;
         return new StreamWriter(Config.OutputFile);
      }
   }
}
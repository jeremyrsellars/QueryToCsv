﻿using System;
using System.IO;
using QueryToCsv.Data.MsSql;

namespace QueryToCsv
{
   using Configuration = ArchiveMessageDumper.Configuration;

   class ArchiveContentDumper
   {
      public readonly Configuration Config;
      readonly MsSqlDbConnectionFactory db;

      public ArchiveContentDumper(Configuration config = null)
      {
         Config = config ?? new Configuration();
         db = new MsSqlDbConnectionFactory(config.ConnectionString);
      }

      public void Run()
      {
         var activity =
            MessageStreamToCsvActivity.WriteDataTo(
               new DbMessageStream(db),
               GetOutputStreamWriter());
         Console.Error.WriteLine("Messages dumped: " + activity.DataRowCount);
      }

      TextWriter GetOutputStreamWriter()
      {
         if (string.IsNullOrWhiteSpace(Config.OutputFile))
            return Console.Out;
         return new StreamWriter(Config.OutputFile);
      }
   }
}
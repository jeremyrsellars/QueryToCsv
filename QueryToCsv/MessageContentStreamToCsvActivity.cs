using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Softek.Nigiri;
using Softek.Nigiri.Data;

namespace QueryToCsv
{
   class MessageContentStreamToCsvActivity
   {
      readonly IMessageStream stream;
      readonly CsvWriter csv;

      public int DataRowCount { get; private set; }

      public static MessageStreamToCsvActivity WriteDataTo(IMessageStream reader, TextWriter writer)
      {
         var p = new MessageStreamToCsvActivity(reader, writer);
         p.WriteHeader();
         p.WriteData();
         writer.Flush();
         return p;
      }

      public MessageContentStreamToCsvActivity(IMessageStream stream, TextWriter writer)
      {
         this.stream = stream;
         var config = new CsvConfiguration { Encoding = Encoding.UTF8, HasHeaderRecord = true, };
         csv = new CsvWriter(writer, config);
      }

      public void WriteHeader()
      {
         csv.WriteHeader(typeof(Message));
      }

      public void WriteData()
      {
         while (stream.Read())
            WritePage();
      }

      void WritePage()
      {
         foreach (var row in stream.GetMessages())
         {
            DataRowCount++;
            csv.WriteRecord(row.Content);
         }
      }
   }
}
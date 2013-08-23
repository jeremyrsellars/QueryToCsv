using System.Data;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace QueryToCsv
{
   class DataReaderToCsvActivity
   {
      readonly IDataReader reader;
      readonly CsvWriter csv;

      public int DataRowCount { get; private set; }

      public static DataReaderToCsvActivity WriteDataTo(IDataReader reader, TextWriter writer)
      {
         var p = new DataReaderToCsvActivity(reader, writer);
         p.WriteHeader();
         p.WriteData();
         writer.Flush();
         return p;
      }

      public DataReaderToCsvActivity(IDataReader reader, TextWriter writer)
      {
         this.reader = reader;
         var config = new CsvConfiguration{Encoding = Encoding.UTF8, HasHeaderRecord=true,};
         csv = new CsvWriter(writer, config);
      }

      public void WriteHeader()
      {
         for (int i = 0; i < reader.FieldCount; i++)
            csv.WriteField(reader.GetName(i));
         csv.NextRecord();
      }

      public void WriteData()
      {
         while(reader.Read())
            WriteRow();
      }

      void WriteRow()
      {
         for(int i = 0; i < reader.FieldCount; i++)
            WriteField(i);
         WriteEndRow();
      }

      void WriteField(int i)
      {
         csv.WriteField(reader.GetValue(i));
      }

      void WriteEndRow()
      {
         csv.NextRecord();
         DataRowCount++;
      }

   }
}
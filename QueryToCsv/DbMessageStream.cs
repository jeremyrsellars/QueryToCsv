using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QueryToCsv.Data;
using Softek.Nigiri;
using Softek.Nigiri.Data;

namespace QueryToCsv
{
   public class DbMessageStream : IMessageStream, IDisposable
   {
      readonly IDbConnectionFactory db;
      IDataReader rdr;
      Int64 lastMessageId;
      IList<Message> page;
      readonly Int64Field messageId;
      readonly StringField format;
      readonly StringField source;
      readonly DateTimeOffsetField createdOn;
      readonly Int64Field sequence;
      readonly StringField content;

      public DbMessageStream(IDbConnectionFactory db)
      {
         this.db = db;
         BatchSize = 1000;
         messageId = new Int64Field {Name = "MessageId"};
         format = new StringField {Name = "Format"};
         source = new StringField {Name = "Source"};
         createdOn = new DateTimeOffsetField {Name = "CreatedOn"};
         sequence = new Int64Field {Name = "Sequence"};
         content = new StringField {Name = "Content"};
      }

      public int BatchSize { get; set; }

      public IEnumerable<Message> GetMessages()
      {
         return page;
      }

      public bool Read()
      {
         page = ReadPageOrNull();
         return page != null && page.Count > 0;
      }

      IList<Message> ReadPageOrNull()
      {
         return db.OpenExecuteClose(CreateNextPageCommand, ExecuteAndRead);
      }

      IDbCommand CreateNextPageCommand(IDbConnection con)
      {
         IDbCommand cmd = con.CreateCommand();
         cmd.CommandText =
@"SELECT Top(@BatchSize)
  M.MessageId, Format=MF.Name, Source=MS.Name, M.CreatedOn, M.Sequence, M.Content
FROM Nigiri.[Message] M
  INNER JOIN Nigiri.MessageFormat MF ON MF.MessageFormatId = M.MessageFormatId
  INNER JOIN Nigiri.MessageSource MS ON MS.MessageSourceId = M.MessageSourceId
WHERE MessageId > IsNull(@LastMessageId, 0)
ORDER BY MessageId ASC
";
         cmd.CommandType = CommandType.Text;
         var batchSizeParam = cmd.CreateParameter();
         batchSizeParam.ParameterName = "@BatchSize";
         batchSizeParam.DbType = DbType.Int32;
         batchSizeParam.Value = BatchSize;
         cmd.Parameters.Add(batchSizeParam);

         var lastMessageIdParam = cmd.CreateParameter();
         lastMessageIdParam.ParameterName = "@LastMessageId";
         lastMessageIdParam.DbType = DbType.Int64;
         lastMessageIdParam.Value = lastMessageId;
         cmd.Parameters.Add(lastMessageIdParam);

         return cmd;
      }

      IList<Message> ExecuteAndRead(IDbCommand cmd)
      {
         using (rdr = cmd.ExecuteReader(CommandBehavior.SingleResult))
         {
            InitFields();
            return ReadPage().ToList();
         }
      }

      void InitFields()
      {
         messageId.Init(rdr);
         format.Init(rdr);
         source.Init(rdr);
         createdOn.Init(rdr);
         sequence.Init(rdr);
         content.Init(rdr);
      }

      IEnumerable<Message> ReadPage()
      {
         while (rdr.Read())
         {
            lastMessageId = messageId.Value;
            yield return ReadMessage();
         }
      }

      Message ReadMessage()
      {
         return
            new Message(
               format: format.Value,
               source: source.Value,
               receivedOn: createdOn.Value,
               sequence: sequence.Value,
               content: content.Value
               );
      }

      public void Close()
      {
         Dispose();
      }

      public void Dispose()
      {
         if(rdr != null)
            rdr.Dispose();
      }
   }
}
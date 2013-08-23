using System.Data;

namespace QueryToCsv.Data
{
   public class Field
   {
      int ordinal;
      IDataReader rdr;

      public Field(string name = null)
      {
         Name = name;
      }

      public string Name { get; set; }

      public void Init(IDataReader reader)
      {
         rdr = reader;
         ordinal = rdr.GetOrdinal(Name);
      }

      public int Ordinal
      {
         get { return ordinal; }
      }

      public IDataReader Reader
      {
         get { return rdr; }
      }
   }

   public class Field<T> : Field
   {
      public T Value
      {
         get
         {
            return (T)Reader.GetValue(Ordinal);
         }
      }
   }
}
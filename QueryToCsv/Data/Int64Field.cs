using System;

namespace QueryToCsv.Data
{
   public class Int64Field : Field
   {
      public Int64 Value
      {
         get { return Reader.GetInt64(Ordinal); }
      }
   }
}
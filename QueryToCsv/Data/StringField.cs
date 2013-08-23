namespace QueryToCsv.Data
{
   public class StringField : Field
   {
      public string Value
      {
         get { return Reader.GetString(Ordinal); }
      }
   }
}
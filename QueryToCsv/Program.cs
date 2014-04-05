using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryToCsv
{
   static class Program
   {
      static string[] arguments;
      static void Main(string[] args)
      {
         arguments = args;
         GetAction()();
      }

      static Action GetAction()
      {
         if (arguments.Any(new List<string> { "-?", "/?", "--help" }.Contains) || arguments.Length == 0)
            return Usage;

         string mode = arguments[0];
         string[] otherArguments = arguments.Skip(1).ToArray();
         if (mode.ToLower() == "table")
            return new TableDumper(GetTableConfiguration(otherArguments)).Run;
         if (mode.ToLower() == "archive-messages")
            return new ArchiveMessageDumper(GetArchiveConfiguration(otherArguments)).Run;
         if (mode.ToLower() == "archive-content")
            return new ArchiveContentDumper(GetArchiveConfiguration(otherArguments)).Run;
         return Usage;
      }

      static void Usage()
      {
         Console.WriteLine(
@"
QueryToCsv Archive-Messages ""{0}"" output.csv

QueryToCsv Archive-Content ""{0}"" output.csv

QueryToCsv Table ""{1}"" output.csv QueryText
",
         new ArchiveMessageDumper.Configuration().ConnectionString,
         new TableDumper.Configuration().ConnectionString);
      }

      static ArchiveMessageDumper.Configuration GetArchiveConfiguration(string[] otherArguments)
      {
         return new ArchiveMessageDumper.Configuration { ConnectionString = otherArguments[0], OutputFile = otherArguments[1] };
      }

      static TableDumper.Configuration GetTableConfiguration(string[] otherArguments)
      {
         return new TableDumper.Configuration { ConnectionString = otherArguments[0], OutputFile = otherArguments[1], QueryText = otherArguments[2] };
      }
   }
}

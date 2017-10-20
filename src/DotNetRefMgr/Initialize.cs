using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetRefMgr
{
   public class Initialize : CommandLineApplication
   {
      public static void ConfigureCommand(CommandLineApplication command)
      {
         command.Description = "Initilize projects.";
         command.HelpOption("-?|-h|--help").ShowInHelpText = false;

         var input = new Input
         {
            Path = command.Argument("path", "Target folder or project file.").Value
         };

         command.OnExecute(() =>
         {
            Execute(input);
            return 0;
         });
      }

      private static void Execute(Input input)
      {
         var files = Utils.GetTargetFiles(input.Path);

         foreach (var file in files)
         {
            var modified = false;
            var lines = File.ReadLines(file).ToList();
            var types = new[] { "Package", "Project" };

            foreach (var type in types)
            {
               if (GetIndexesOf($@"<!--\s*<{type}References>\s*-->", lines).Any())
                  continue;

               modified = true;
               var indexes = GetIndexesOf($"<{type}Reference ", lines);

               if (indexes.Any())
               {
                  var last = indexes.Last();
                  var insert = last + 1;

                  var whitespaces = Utils.GetLeadingWhitespaces(lines[last]);

                  lines.InsertRange(insert, new[]
                  {
                     "",
                     $"{whitespaces}<!-- <{type}References> -->",
                     $"{whitespaces}<!-- </{type}References> -->"
                  });
               }
               else
               {
                  var last = GetIndexesOf("</PropertyGroup>", lines).Last();
                  var insert = last + 1;

                  var whitespaces = Utils.GetLeadingWhitespaces(lines[last]);

                  lines.InsertRange(insert, new[]
                  {
                     "",
                     $"{whitespaces}<ItemGroup>",
                     $"{whitespaces}{whitespaces}<!-- <{type}References> -->",
                     $"{whitespaces}{whitespaces}<!-- </{type}References> -->",
                     $"{whitespaces}</ItemGroup>",
                  });
               }
            }

            if (modified)
            {
               File.WriteAllLines(file, lines);
            }

            Console.WriteLine($"Initialized '{Path.GetFileName(file)}'.");
         }
      }

      private static IReadOnlyList<int> GetIndexesOf(string pattern, IReadOnlyList<string> lines)
      {
         return GetIndexesOf(new[] { pattern }, lines);
      }

      private static IReadOnlyList<int> GetIndexesOf(IReadOnlyCollection<string> patterns, IReadOnlyList<string> lines)
      {
         var indexes = new List<int>();

         for (var i = 0; i < lines.Count; i++)
         {
            if (!patterns.Any(x => Regex.IsMatch(lines[i], x))) continue;
            indexes.Add(i);
         }

         return indexes;
      }

      class Input
      {
         public string Path;
      }
   }
}
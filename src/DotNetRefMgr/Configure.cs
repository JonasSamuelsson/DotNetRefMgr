using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetRefMgr
{
   public class Configure : CommandLineApplication
   {
      public static void ConfigureCommand(CommandLineApplication command)
      {
         command.Description = "Configure projects to use package or project references.";
         command.HelpOption("-?|-h|--help").ShowInHelpText = false;

         var args = new
         {
            Path = command.Argument("path", "Target folder or project file."),
            Type = command.Option("-t|--type", "Type of reference (package|project).", CommandOptionType.SingleValue)
         };

         command.OnExecute(() =>
         {
            var input = new Input
            {
               Path = args.Path.Value
            };

            if (!args.Type.HasValue())
               throw new AppException("Missing reference type.");

            if (!Enum.TryParse(args.Type.Value(), true, out input.Type))
               throw new AppException($"Invalid type '{args.Type.Value()}'.");

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
            var types = new[] { ReferenceType.Package, ReferenceType.Project };
            var enable = types.Single(x => x == input.Type);
            var disable = types.Single(x => x != input.Type);
            Enable(enable, lines, ref modified);
            Disable(disable, lines, ref modified);

            if (modified)
            {
               File.WriteAllLines(file, lines);
            }

            Console.WriteLine($"Configured '{Path.GetFileName(file)}'.");
         }
      }

      private static void Disable(ReferenceType type, IList<string> lines, ref bool modified)
      {
         var startTag = $"<!-- <{type}References> -->";
         var endTag = $"<!-- </{type}References> -->";

         var process = false;

         for (var i = 0; i < lines.Count; i++)
         {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
            {
               continue;
            }

            if (Regex.IsMatch(line, startTag))
            {
               process = true;
               continue;
            }

            if (Regex.IsMatch(line, endTag))
            {
               process = false;
               continue;
            }

            if (process)
            {
               var whitespaces = Utils.GetLeadingWhitespaces(line);
               var content = line.Trim();

               if (!content.StartsWith("<!--"))
               {
                  content = "<!-- " + content;
               }

               if (!content.EndsWith("-->"))
               {
                  content += " -->";
               }

               modified = true;
               lines[i] = whitespaces + content;
            }
         }
      }

      private static void Enable(ReferenceType type, IList<string> lines, ref bool modified)
      {
         var startTag = $"<!-- <{type}References> -->";
         var endTag = $"<!-- </{type}References> -->";

         var process = false;

         for (var i = 0; i < lines.Count; i++)
         {
            var line = lines[i];

            if (Regex.IsMatch(line, startTag))
            {
               process = true;
               continue;
            }

            if (Regex.IsMatch(line, endTag))
            {
               process = false;
               continue;
            }

            if (process)
            {
               var whitespaces = Utils.GetLeadingWhitespaces(line);
               var content = line.Trim();

               if (content.StartsWith("<!--"))
               {
                  content = content.Substring(4).TrimStart();
               }

               if (content.EndsWith("-->"))
               {
                  content = content.Substring(0, content.Length - 3).TrimEnd();
               }

               modified = true;
               lines[i] = whitespaces + content;
            }
         }
      }

      private class Input
      {
         public string Path;
         public ReferenceType Type;
      }

      private enum ReferenceType
      {
         Package,
         Project
      }
   }
}
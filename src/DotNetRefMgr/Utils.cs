using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetRefMgr
{
   internal static class Utils
   {
      public static IEnumerable<string> GetTargetFiles(string path)
      {
         if (string.IsNullOrWhiteSpace(path))
         {
            path = Environment.CurrentDirectory;
         }

         if (File.Exists(path))
         {
            if (Path.GetExtension(path)?.ToLower() != ".csproj")
            {
               throw new AppException("Invalid target file type (must be .csproj).");
            }

            return new[] { path };
         }

         if (Directory.Exists(path))
         {
            return Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
         }

         throw new AppException($"File/folder '{path}' was not found.");
      }

      public static string GetLeadingWhitespaces(string s)
      {
         var index = 0;
         for (; index < s.Length && char.IsWhiteSpace(s[index]); index++) { }
         return s.Substring(0, index);
      }
   }
}
using System;

namespace DotNetRefMgr
{
   public class AppException : Exception
   {
      public AppException(string message) : base(message)
      {
      }
   }
}
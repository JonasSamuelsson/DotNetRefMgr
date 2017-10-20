using Microsoft.Extensions.CommandLineUtils;

namespace DotNetRefMgr
{
   public class Root : CommandLineApplication
   {
      public Root()
      {
         Command("initialize", Initialize.ConfigureCommand);
         Command("configure", Configure.ConfigureCommand);
         //Commands.Add(new Initialize());
         //Commands.Add(new Configure());

         HelpOption("-?|-h|--help").ShowInHelpText = false;
      }
   }
}
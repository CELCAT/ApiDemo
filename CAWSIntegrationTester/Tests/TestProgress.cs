using System;

namespace CAWSIntegrationTester.Tests
{
   public class TestProgress
   {
      protected void Progress()
      {
         // blank line
         Console.WriteLine();
      }

      protected void Progress(string msg, params object[] args)
      {
         Console.WriteLine("{0} {1}", DateTime.Now.ToShortTimeString(), string.Format(msg, args));
      }

      protected void ProgressIndented(int level, string msg, params object[] args)
      {
         Progress(string.Concat(new string(' ', level), msg), args);
      }
   }
}

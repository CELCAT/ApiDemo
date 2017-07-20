using System;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class RegisterSummary : RegisterSummaryPoco, IModel
   {
      public string GetId()
      {
         return string.Format("{0}{1}", EventId, (Week + 1).ToString("D2"));
      }

      public string GetName()
      {
         return string.Format("Event: {0}, Week: {1}", EventId, Week);
      }


      public void CheckSameValues(RegisterSummary rs)
      {
         if(!this.SameValue(rs))
         {
            throw new Exception("Register Summaries are not equal!");
         }
      }

   }
}

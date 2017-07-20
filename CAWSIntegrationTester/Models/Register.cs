using System;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class Register : RegisterPoco, IModel
   {
      public string GetId()
      {
         return string.Format("{0}{1}", EventId, (Week + 1).ToString("D2"));
      }

      public void SetId(int eventId, int weekNum)
      {
         EventId = eventId;
         Week = weekNum;
      }

      public string GetName()
      {
         return string.Format("Event: {0}, Week: {1}", EventId, Week);
      }

      public void CheckSameValues(Register reg, bool activityExistsInOriginal)
      {
         if (!this.SameValue(reg, activityExistsInOriginal))
         {
            throw new Exception("Register values are not the same!");
         }
      }
   }
}

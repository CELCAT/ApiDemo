using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   class SimpleStudentTimes : SimpleStudentTimesPoco, IModel
   {
      public string GetId()
      {
         return string.Format("{0}-{1}-{2}", StudentId, EventId, Week);
      }

      public string GetName()
      {
         return GetId();
      }
   }
}

using System;
using System.Globalization;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class StudentException : StudentExceptionPoco, IModel
   {
      public string GetId()
      {
         return StudentExceptionId.ToString(CultureInfo.InvariantCulture);
      }

      public string GetName()
      {
         return GetId();
      }

      public void CheckSameValues(StudentException se)
      {
         if(!this.SameValue(se))
         {
            throw new Exception("Student exception value not identical");
         }
      }
   }
}

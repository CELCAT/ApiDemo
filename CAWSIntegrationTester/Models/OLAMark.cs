using System;
using System.Globalization;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class OlaMark : OfflineStagingMarkPoco, IModel
   {
      public string GetId()
      {
         return Id.ToString(CultureInfo.InvariantCulture);
      }

      public string GetName()
      {
         return Id.ToString(CultureInfo.InvariantCulture);
      }

      public void CheckSameValues(OlaMark newDefn)
      {
         if (!this.SameValue(newDefn))
         {
            throw new Exception("OLA Marks not equal!");
         }
      }

   }
}

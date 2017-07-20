using System;
using System.Globalization;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class MarkDefinition : MarkDefinitionPoco, IModel
   {
      public string GetId()
      {
         return Id.ToString(CultureInfo.InvariantCulture);
      }

      public string GetName()
      {
         return Name;
      }

      public void CheckSameValues(MarkDefinition newDefn)
      {
         if(!this.SameValue(newDefn))
         {
            throw new Exception("Mark definitions not equal!");
         }
      }
   }
}

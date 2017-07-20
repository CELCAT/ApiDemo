using System;
using System.Globalization;
using Celcat.Models;

namespace CAWSIntegrationTester.Models
{
   public class StudentLedger : StudentLedgerEntryPoco, IModel
   {
      public string GetId()
      {
         return RecordId.ToString(CultureInfo.InvariantCulture);
      }

      public string GetName()
      {
         return GetId();
      }

      public void CheckSameValues(StudentLedger ledger)
      {
         if (!this.SameValue(ledger))
         {
            throw new Exception("Student ledgers not equal!");
         }
      }
   }
}

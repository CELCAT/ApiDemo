using System;
using System.Collections.Generic;
using System.Globalization;
using Celcat.Models.Helpers;

namespace CAWSIntegrationTester.Models
{
   public class ExceptionMark : IModel
   {
      public int ExceptionId { get; set; }
      public AttExceptionType ExceptionType { get; set; }
      public DateTime StartDateTime { get; set; }
      public DateTime? EndDateTime { get; set; }
      public int? MarkId { get; set; }
      public int? MinsLate { get; set; }
      public string Comments { get; set; }
      public int? OriginId { get; set; }
      public string OriginalId { get; set; }
      public DateTime DateChange { get; set; }
      public int UserId { get; set; }
      public List<StudentException> StudentExceptions { get; set; }
      public bool Modified { get; set; }
      public bool Removed { get; set; }

      public string GetId()
      {
         return ExceptionId.ToString(CultureInfo.InvariantCulture);
      }

      public string GetName()
      {
         return ExceptionId.ToString(CultureInfo.InvariantCulture);
      }

      public void CheckSameValues(ExceptionMark em)
      {
         if (!this.SameValue(em))
         {
            throw new Exception("Exception marks are not equal!");
         }
      }
   }
}

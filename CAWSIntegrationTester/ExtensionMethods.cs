using System.Collections.Generic;
using System.Globalization;
using CAWSIntegrationTester.Models;
using Celcat.Models;

namespace CAWSIntegrationTester
{
   public static class ExtensionMethods
   {

      public static bool SameValue(this ExceptionMark lhs, ExceptionMark rhs)
      {
         return
            lhs.ExceptionType == rhs.ExceptionType &&
            lhs.StartDateTime == rhs.StartDateTime &&
            lhs.EndDateTime == rhs.EndDateTime &&
            lhs.MarkId == rhs.MarkId &&
            lhs.MinsLate == rhs.MinsLate &&
            lhs.Comments == rhs.Comments &&
            lhs.OriginId == rhs.OriginId &&
            lhs.OriginalId == rhs.OriginalId &&
            SameStudentExceptions(lhs.StudentExceptions, rhs.StudentExceptions) &&
            lhs.Removed == rhs.Removed;
      }

      private static bool SameStudentExceptions(List<StudentException> lhs, List<StudentException> rhs)
      {
         if (lhs == null && rhs == null)
         {
            return true;
         }
         if (lhs == null || rhs == null)
         {
            return false;
         }

         if (lhs.Count != rhs.Count)
         {
            return false;
         }

         lhs.Sort((x1, x2) => x1.StudentExceptionId.CompareTo(x2.StudentExceptionId));
         rhs.Sort((x1, x2) => x1.StudentExceptionId.CompareTo(x2.StudentExceptionId));

         for (int n = 0; n < lhs.Count; ++n)
         {
            if (!lhs[n].SameValue(rhs[n]))
            {
               return false;
            }
         }

         return true;
      }
      
      public static bool SameValue(this OlaMark lhs, OlaMark rhs)
      {
         return
            lhs.Session == rhs.Session &&
            lhs.MarkStamp.ToString(CultureInfo.InvariantCulture) == rhs.MarkStamp.ToString(CultureInfo.InvariantCulture) &&
            lhs.MarkAbbrev == rhs.MarkAbbrev &&
            lhs.MarkingUserId == rhs.MarkingUserId &&
            lhs.MarkId == rhs.MarkId &&
            lhs.Student == rhs.Student &&
            lhs.StudentContext == rhs.StudentContext &&
            lhs.StudentId == rhs.StudentId &&
            lhs.Room == rhs.Room &&
            lhs.RoomContext == rhs.RoomContext &&
            lhs.RoomId == rhs.RoomId &&
            lhs.Status == rhs.Status &&
            lhs.EventId == rhs.EventId &&
            lhs.Week == rhs.Week &&
            lhs.Uploaded.ToString(CultureInfo.InvariantCulture) == rhs.Uploaded.ToString(CultureInfo.InvariantCulture) &&
            lhs.Processed == rhs.Processed;
      }

      public static bool SameValue(this StudentException se1, StudentException se2)
      {
         return
            se1.StudentId == se2.StudentId &&
            se1.EventId == se2.EventId &&
            se1.Removed == se2.Removed &&
            se1.OriginId == se2.OriginId &&
            se1.OriginalId == se2.OriginalId;
      }

      public static bool SameValue(this StudentLedger sl1, StudentLedger sl2)
      {
         return
            sl1.RecordId == sl2.RecordId &&
            sl1.Source == sl2.Source &&
            sl1.StudentId == sl2.StudentId &&
            sl1.EventId == sl2.EventId &&
            sl1.Week == sl2.Week &&
            sl1.MarkId == sl2.MarkId &&
            sl1.MinsLate == sl2.MinsLate &&
            sl1.Comments == sl2.Comments &&
            sl1.DateChange == sl2.DateChange &&
            sl1.UserId == sl2.UserId;
      }

      public static bool SameValue(this MarkDefinition lhs, MarkDefinition rhs)
      {
         return
            lhs.Name == rhs.Name &&
            lhs.Description == rhs.Description &&
            lhs.Abbreviation == rhs.Abbreviation &&
            lhs.ShortcutKey == rhs.ShortcutKey &&
            lhs.Colour == rhs.Colour &&
            lhs.Definition == rhs.Definition &&
            lhs.Precedence == rhs.Precedence &&
            lhs.Card == rhs.Card &&
            lhs.Notify == rhs.Notify &&
            lhs.Notification == rhs.Notification &&
            lhs.OriginId == rhs.OriginId &&
            lhs.OriginalId == rhs.OriginalId;
      }

      public static bool SameValue(this RegisterStudentTimesPoco lhs, RegisterStudentTimesPoco rhs)
      {
         return
            lhs.AttendTimeId == rhs.AttendTimeId &&
            lhs.ActivityId == rhs.ActivityId &&
            lhs.StudentId == rhs.StudentId &&
            lhs.InTime == rhs.InTime &&
            lhs.OutTime == rhs.OutTime &&
            lhs.Modified == rhs.Modified;
      }

      public static bool SameValue(this List<RegisterStudentTimesPoco> lhs, List<RegisterStudentTimesPoco> rhs)
      {
         if (lhs == null && rhs == null)
         {
            return true;
         }
         if (lhs == null || rhs == null)
         {
            return false;
         }

         if (lhs.Count != rhs.Count)
         {
            return false;
         }

         for (int n = 0; n < lhs.Count; ++n)
         {
            if (!lhs[n].SameValue(rhs[n]))
            {
               return false;
            }
         }

         return true;
      }

      public static bool SameValue(this RegisterStudentMarkPoco lhs, RegisterStudentMarkPoco rhs)
      {
         return
            lhs.ActivityId == rhs.ActivityId &&
            lhs.StudentId == rhs.StudentId &&
            lhs.MarkId == rhs.MarkId &&
            lhs.MinsLate == rhs.MinsLate &&
            lhs.Comments == rhs.Comments &&
            lhs.Source == rhs.Source &&
            lhs.UserId == rhs.UserId &&
            lhs.DateChange == rhs.DateChange &&
            lhs.Modified == rhs.Modified &&
            lhs.NotificationId == rhs.NotificationId &&
            lhs.NotificationMessage == rhs.NotificationMessage &&
            lhs.NotificationSent == rhs.NotificationSent &&
            lhs.NotificationModified == rhs.NotificationModified &&
            lhs.TimesInOut.SameValue(rhs.TimesInOut);
      }


      private static bool SameStatusValues(RegisterSummary rs1, RegisterSummary rs2)
      {
         if (rs1.Status == null && rs2.Status == null)
         {
            return true;
         }

         if (rs1.Status == null || rs2.Status == null)
         {
            return false;
         }

         if (rs1.Status.Length != rs2.Status.Length)
         {
            return false;
         }

         for (int n = 0; n < rs1.Status.Length; ++n)
         {
            if (rs1.Status[n] != rs2.Status[n])
            {
               return false;
            }
         }

         return true;
      }

      private static bool SameEntities(RegisterSummary rs1, RegisterSummary rs2)
      {
         if (rs1.Entities == null && rs2.Entities == null)
         {
            return true;
         }

         if (rs1.Entities == null || rs2.Entities == null)
         {
            return false;
         }

         if (rs1.Entities.Count != rs2.Entities.Count)
         {
            return false;
         }

         foreach (var key in rs1.Entities.Keys)
         {
            var names1 = rs1.Entities[key];
            var names2 = rs2.Entities[key];

            foreach (var keyB in names1.Keys)
            {
               var namesB1 = names1[keyB];
               var namesB2 = names2[keyB];

               if(namesB1.UniqueName != namesB2.UniqueName)
               {
                  return false;
               }
            }
         }

         return true;
      }

      public static bool SameValue(this Register lhs, Register rhs, bool activityExistsInOriginal)
      {
         return
            (!activityExistsInOriginal || lhs.ActivityId == rhs.ActivityId) &&
            lhs.EventId == rhs.EventId &&
            lhs.Week == rhs.Week &&
            lhs.RegisterNotes == rhs.RegisterNotes &&
            lhs.EventNotes == rhs.EventNotes &&
            lhs.StaffId == rhs.StaffId &&
            lhs.StaffPresent == rhs.StaffPresent &&
            lhs.Closed == rhs.Closed &&
            lhs.CloseOnSave == rhs.CloseOnSave;
      }

      public static bool SameValue(this RegisterSummary lhs, RegisterSummary rhs)
      {
          return
              SameStatusValues(lhs, rhs) &&
              lhs.Id == rhs.Id &&
              lhs.EventId == rhs.EventId &&
              lhs.Week == rhs.Week &&
              lhs.ActivityId == rhs.ActivityId &&
              lhs.StartDateTime == rhs.StartDateTime &&
              lhs.EndDateTime == rhs.EndDateTime &&
              lhs.StudentCount == rhs.StudentCount &&
              lhs.MarkCount == rhs.MarkCount &&
              SameEntities(lhs, rhs);
      }
   }
}

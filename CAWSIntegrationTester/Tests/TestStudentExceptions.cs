using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;
using Celcat.Models;
using Celcat.Models.Helpers;

namespace CAWSIntegrationTester.Tests
{
   class TestStudentExceptions : TestBase
   {
      private List<StudentException> FStudentExceptions;
      private readonly Register[] FRegisters;
      private readonly DateTime[] FSomeRegisterDates;
      private readonly MarkDefinition[] FMarkDefinitions;
      private List<ExceptionMark> FNewlyInsertedExceptions;

      public TestStudentExceptions(HttpClient client, Uri baseUri, Register[] reg, 
         DateTime[] someRegisterDates, MarkDefinition[] markDefinitions) 
         : base(client, baseUri)
      {
         FRegisters = reg;
         FSomeRegisterDates = someRegisterDates;
         FMarkDefinitions = markDefinitions;
         Array.Sort(FSomeRegisterDates);
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "StudentExceptions");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.BadRequest);
      }

      protected override async Task TestGetWithFilter()
      {
         FStudentExceptions = new List<StudentException>();

         const int ATTEMPTS = 20;
         
         for (int n = 0; n < ATTEMPTS; ++n)
         {
            await InternalFilterTest(true);
         }

         if(FStudentExceptions.Count == 0)
         {
            // try without the student filter...
            for (int n = 0; n < ATTEMPTS; ++n)
            {
               await InternalFilterTest(false);
            }

            if (FStudentExceptions.Count == 0)
            {
               throw new Exception("Unable to verify the Student exceptions API - no records found");
            }
         }
      }

      private async Task InternalFilterTest(bool incStudent)
      {
         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "StudentExceptions"), CreateFilter(incStudent));

         var response = await FClient.GetAsync(uri);

         if (response.StatusCode != HttpStatusCode.NoContent)
         {
            await CheckSuccessStatusCode(response);
            var items = await response.Content.ReadAsAsync<StudentException[]>();

            Shuffle(items);
            FStudentExceptions.AddRange(items);
         }
      }

      private AwsExceptionFilterPoco CreateFilter(bool incStudent)
      {
         Shuffle(FRegisters);

         Register r = FindRegisterWithStudentMarks();
         int studentIndex = FRandom.Next(0, r.StudentMarks.Count);

         AwsExceptionFilterPoco result = new AwsExceptionFilterPoco();

         if(incStudent)
         {
            result.StudentId = r.StudentMarks[studentIndex].StudentId;
         }
               
         result.StartDateTime = GetRandomStartDate();
         result.EndDateTime = result.StartDateTime.AddDays(FRandom.Next(1, 4));

         result.Types[(int)AttExceptionType.CetExceptionAdd] = true;
         result.Types[(int)AttExceptionType.CetExceptionRemove] = true;
         result.Types[(int)AttExceptionType.CetExceptionWithdraw] = true;
         result.Types[(int)AttExceptionType.CetExceptionCancelWithdraw] = true;
         result.Types[(int)AttExceptionType.CetExtendedAbsence] = true;
         result.Types[(int)AttExceptionType.CetWithdrawFromCollege] = true;

         if(result == null)
         {
            throw new Exception("Could not create exception filter!");
         }

         return result;
      }

      private DateTime GetRandomStartDate()
      {
         return FSomeRegisterDates[FRandom.Next(0, FSomeRegisterDates.Length)].Date;
      }

      protected override async Task TestGetItem()
      {
         if (FStudentExceptions.Count > 0)
         {
            var ex = FStudentExceptions[0];
            Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", ex.ExceptionId));

            var response = await FClient.GetAsync(uri);
            await CheckSuccessStatusCode(response);
            var se = await response.Content.ReadAsAsync<StudentException>();

            ex.CheckSameValues(se);
         }
      }

      protected override async Task TestGetNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", BAD_ID));
         
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }


      protected override async Task TestPost()
      {
         FNewlyInsertedExceptions = new List<ExceptionMark>();

         int numIters = FRandom.Next(4, 10);

         for (int i = 0; i < numIters; ++i)
         {
            int markId = GetMarkId();

            var em = new ExceptionMark
            {
               ExceptionType = AttExceptionType.CetExtendedAbsence,
               MarkId = markId,
               Comments = GetRandomString(FRandom.Next(1, 129), true),
               StudentExceptions = new List<StudentException>(),
               Modified = true
            };

            em.StartDateTime = GetRandomStartDate();
            em.EndDateTime = em.StartDateTime.AddDays(FRandom.Next(1, 60));

            int numStudents = FRandom.Next(1, 6);
            List<int> studentsUsed = new List<int>();
            for (int n = 0; n < numStudents; ++n)
            {
               int studentId = GetRandomStudentId();
               if (!studentsUsed.Contains(studentId))
               {
                  em.StudentExceptions.Add(new StudentException
                  {
                     StudentId = studentId,
                     Modified = true
                  });

                  studentsUsed.Add(studentId);
               }
            }

            Uri uri = new Uri(FBaseUri, "StudentExceptions");
            var response = await FClient.PostAsJsonAsync(uri.ToString(), em);
            await CheckStatusCodeIs(response, HttpStatusCode.Created);

            var se = await response.Content.ReadAsAsync<ExceptionMark>();
            se.CheckSameValues(em);

            FNewlyInsertedExceptions.Add(se);
         }
      }

      private int GetMarkId()
      {
         Shuffle(FMarkDefinitions);
         return FMarkDefinitions.First(x => x.Definition == "P").Id;
      }

      private Register FindRegisterWithStudentMarks()
      {
         return FRegisters.FirstOrDefault(r => r.StudentMarks != null && r.StudentMarks.Count > 0);
      }

      private int GetRandomStudentId()
      {
         Shuffle(FRegisters);
         Register reg = FindRegisterWithStudentMarks();
         if(reg == null)
         {
            throw new Exception("Could not find register with student marks!");
         }

         var sm = reg.StudentMarks[0];
         return sm.StudentId;
      }

      protected override Task TestPostDuplicate()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override async Task TestPutAtRoot()
      {
         if (FNewlyInsertedExceptions.Count > 0)
         {
            var e = FNewlyInsertedExceptions[0];
            e.Comments = GetRandomString(50, true);
            e.Modified = true;

            Uri uri = new Uri(FBaseUri, "StudentExceptions");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), FNewlyInsertedExceptions[0]);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPut()
      {
         // make changes to the newly added exceptions...

         foreach (var e in FNewlyInsertedExceptions)
         {
            // change description...
            e.Comments = GetRandomString(50, true);
            e.Modified = true;

            Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", e.ExceptionId));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), e);
            await CheckSuccessStatusCode(response);
            
            var retVal = await response.Content.ReadAsAsync<ExceptionMark>();
            e.CheckSameValues(retVal);
         }
      }

      protected override async Task TestPutNonExistingItem()
      {
         if (FNewlyInsertedExceptions.Count > 0)
         {
            var ex = FNewlyInsertedExceptions[0];

            int origId = ex.ExceptionId;
            ex.ExceptionId = BAD_INT_ID;
            Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", ex.ExceptionId));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), ex);
            ex.ExceptionId = origId;
            await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
         }
      }

      protected override async Task TestDelete()
      {
         // remove the newly inserted exceptions...
         foreach (var e in FNewlyInsertedExceptions)
         {
            Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", e.GetId()));
            var response = await FClient.DeleteAsync(uri.ToString());
            await CheckStatusCodeIs(response, HttpStatusCode.NoContent);
         }
      }

      protected override async Task TestDeleteNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("StudentExceptions/{0}", BAD_ID));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }
   }
}

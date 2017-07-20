using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;
using Celcat.Models;

namespace CAWSIntegrationTester.Tests
{
   class TestStudentMarks : TestBase
   {
      private List<StudentMarks> FStudentMarks;
      private readonly Register[] FRegisters;

      public TestStudentMarks(HttpClient client, Uri baseUri, Register[] reg) 
         : base(client, baseUri)
      {
         FRegisters = reg;            
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "StudentMarks");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.BadRequest);
      }

      private AwsMarkFilterPoco CreateMarksFilterByEvent(out int eventSelected, out int weekSelected)
      {
         AwsMarkFilterPoco result = null;

         Shuffle(FRegisters);

         eventSelected = -1;
         weekSelected = -1;

         foreach (var reg in FRegisters)
         {
            if (reg.StudentMarks != null && reg.StudentMarks.Count > 0)
            {
               result = new AwsMarkFilterPoco
               {
                  EventIds = new List<int> {reg.EventId}
               };

               result.Weeks[reg.Week] = true;

               eventSelected = reg.EventId;
               weekSelected = reg.Week;

               break;
            }
         }

         if(result == null)
         {
            throw new Exception("Could not create marks filter!");
         }

         return result;
      }

      private AwsMarkFilterPoco CreateMarksFilterByStudent(out int studentSelected, out int weekSelected)
      {
         AwsMarkFilterPoco result = null;

         Shuffle(FRegisters);

         studentSelected = -1;
         weekSelected = -1;

         foreach (var reg in FRegisters)
         {
            if (reg.StudentMarks != null && reg.StudentMarks.Count > 0)
            {
               result = new AwsMarkFilterPoco
               {
                  StudentIds = new List<int> { reg.StudentMarks.First().StudentId }
               };

               result.Weeks[reg.Week] = true;

               studentSelected = result.StudentIds.First();
               weekSelected = reg.Week;

               break;
            }
         }

         if (result == null)
         {
            throw new Exception("Could not create marks filter!");
         }

         return result;
      }

      protected override async Task TestGetWithFilter()
      {
         await FilterTest1();
         await FilterTest2();
      }

      private async Task FilterTest1()
      {
         int eventSelected;
         int weekSelected;

         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "StudentMarks"),
            CreateMarksFilterByEvent(out eventSelected, out weekSelected));

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);

         var items = await response.Content.ReadAsAsync<StudentMarks[]>();

         var arr = items.ToArray();
         Shuffle(arr);

         // store for use in later test...
         FStudentMarks = new List<StudentMarks>();
         FStudentMarks.AddRange(arr);

         if (FStudentMarks.Count == 0)
         {
            throw new Exception("Could not find any student marks for test!");
         }

         foreach(var sm in FStudentMarks)
         {
            if(sm.Week != weekSelected)
            {
               throw new Exception("Unexpected week!");
            }
            if(sm.EventId != eventSelected)
            {
               throw new Exception("Unexpected event Id!");
            }
         }
      }

      private async Task FilterTest2()
      {
         int studentSelected;
         int weekSelected;

         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "StudentMarks"),
            CreateMarksFilterByStudent(out studentSelected, out weekSelected));

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);

         var items = await response.Content.ReadAsAsync<StudentMarks[]>();

         foreach (var sm in items)
         {
            if (sm.Week != weekSelected)
            {
               throw new Exception("Unexpected week!");
            }
            if (sm.StudentId != studentSelected)
            {
               throw new Exception("Unexpected student Id!");
            }
         }
      }

      protected override async Task TestGetItem()
      {
         Uri uri = new Uri(FBaseUri, string.Format("StudentMarks/{0}", FStudentMarks[0].GetId()));
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestGetNonExistingItem()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestPost()
      {
         Uri uri = new Uri(FBaseUri, "StudentMarks");
         
         var response = await FClient.PostAsJsonAsync(uri.ToString(), FStudentMarks[0]);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPostDuplicate()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestPutAtRoot()
      {
         if (FStudentMarks.Count > 0)
         {
            var sm = FStudentMarks[0];
            Uri uri = new Uri(FBaseUri, "StudentMarks");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), sm);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPut()
      {
         if (FStudentMarks.Count > 0)
         {
            var sm = FStudentMarks[0];
            Uri uri = new Uri(FBaseUri, string.Format("StudentMarks/{0}", sm.GetId()));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), sm);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override Task TestPutNonExistingItem()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestDelete()
      {
         Uri uri = new Uri(FBaseUri, string.Format("StudentMarks/{0}", FStudentMarks[0].GetId()));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestDeleteNonExistingItem()
      {
         return Task.FromResult(0);
      }
   }
}

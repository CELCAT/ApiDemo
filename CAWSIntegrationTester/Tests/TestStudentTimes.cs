using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;
using Celcat.Models;

namespace CAWSIntegrationTester.Tests
{
   class TestStudentTimes : TestBase
   {
      private readonly Register[] FRegisters;

      public TestStudentTimes(HttpClient client, Uri baseUri, Register[] reg) 
         : base(client, baseUri)
      {
         FRegisters = reg;
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut");
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
                  EventIds = new List<int> { reg.EventId }
               };

               result.Weeks[reg.Week] = true;

               eventSelected = reg.EventId;
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
         bool found = false;
         const int NUM_ITERS = 10;

         for (int n = 0; n < NUM_ITERS && !found; ++n)
         {
            int eventSelected;
            int weekSelected;

            Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "StudentTimesInOut"),
               CreateMarksFilterByEvent(out eventSelected, out weekSelected));

            var response = await FClient.GetAsync(uri);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
               await CheckSuccessStatusCode(response);
               var items = await response.Content.ReadAsAsync<SimpleStudentTimes[]>();

               foreach(var it in items)
               {
                  if(it.EventId != eventSelected || it.Week != weekSelected)
                  {
                     throw new Exception("Unexpected event or week found in  student times IN/OUT");
                  }
               }

               found = true;
            }
         }

         if(!found)
         {
            throw new Exception("Could not find any student times IN/OUT!");
         }
      }

      protected override async Task TestGetItem()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut/123");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestGetNonExistingItem()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override async Task TestPost()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut");

         var t = new SimpleStudentTimesPoco();
         var response = await FClient.PostAsJsonAsync(uri.ToString(), t);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPostDuplicate()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override async Task TestPutAtRoot()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut");

         var t = new SimpleStudentTimesPoco();
         var response = await FClient.PutAsJsonAsync(uri.ToString(), t);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override async Task TestPut()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut/ABC");

         var t = new SimpleStudentTimesPoco();
         var response = await FClient.PutAsJsonAsync(uri.ToString(), t);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPutNonExistingItem()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override async Task TestDelete()
      {
         Uri uri = new Uri(FBaseUri, "StudentTimesInOut/123");
         
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestDeleteNonExistingItem()
      {
         // nothing to do
         return Task.FromResult(0);
      }
   }
}

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
   class TestStudentLedger : TestBase
   {
      private List<StudentLedger> FLedgers;
      private readonly Register[] FRegisters;

      public TestStudentLedger(HttpClient client, Uri baseUri, Register[] reg) 
         : base(client, baseUri)
      {
         FRegisters = reg;
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "StudentLedger");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.BadRequest);
      }

      private AwsLedgerFilterPoco CreateLedgerFilter()
      {
         AwsLedgerFilterPoco result = null;

         Shuffle(FRegisters);

         foreach(var reg in FRegisters)
         {
            if (reg.StudentMarks != null && reg.StudentMarks.Count > 0)
            {
               int studentIndex = FRandom.Next(0, reg.StudentMarks.Count);

               result = new AwsLedgerFilterPoco()
               {
                  StudentId = reg.StudentMarks[studentIndex].StudentId,
                  EventId = reg.EventId,
                  Week = reg.Week
               };

               break;
            }
         }

         if (result == null)
         {
            throw new Exception("Could not create student ledger filter!");
         }
         
         return result;
      }

      protected override async Task TestGetWithFilter()
      {
         int iter = 0;
         HttpStatusCode sc = HttpStatusCode.NoContent;
         HttpResponseMessage response = null;
         while (sc == HttpStatusCode.NoContent && iter != 5)
         {
            Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "StudentLedger"), CreateLedgerFilter());
            response = await FClient.GetAsync(uri);
            sc = response.StatusCode;
            ++iter;
         }

         if (response == null || sc == HttpStatusCode.NoContent)
         {
            throw new Exception("Could not find any student ledgers for test!");
         }

         await CheckSuccessStatusCode(response);

         var items = await response.Content.ReadAsAsync<StudentLedger[]>();

         var arr = items.ToArray();
         Shuffle(arr);

         // store for use in later test...
         FLedgers = new List<StudentLedger>();
         FLedgers.AddRange(arr);

         if (FLedgers.Count == 0)
         {
            throw new Exception("Could not find any student ledgers for test!");
         }
      }

      protected override async Task TestGetItem()
      {
         // uncomment this section if we decide to implement "GetItem":

         //foreach (var ledger in FLedgers)
         //{
         //   Uri uri = new Uri(FBaseUri, string.Format("StudentLedger/{0}", ledger.GetId()));
         //   var response = await FClient.GetAsync(uri);
         //   CheckSuccessStatusCode(response);

         //   var sl = await response.Content.ReadAsAsync<StudentLedger>();
         //   sl.CheckSameValues(ledger);
         //}

         Uri uri = new Uri(FBaseUri, string.Format("StudentLedger/{0}", FLedgers[0].GetId()));
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestGetNonExistingItem()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestPost()
      {
         Uri uri = new Uri(FBaseUri, "StudentLedger");
         
         var response = await FClient.PostAsJsonAsync(uri.ToString(), FLedgers[0]);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPostDuplicate()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestPutAtRoot()
      {
         Uri uri = new Uri(FBaseUri, "StudentLedger");
         var response = await FClient.PutAsJsonAsync(uri.ToString(), FLedgers[0]);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPut()
      {
         return Task.FromResult(0);
      }

      protected override Task TestPutNonExistingItem()
      {
         return Task.FromResult(0);
      }

      protected override async Task TestDelete()
      {
         Uri uri = new Uri(FBaseUri, string.Format("StudentLedger/{0}", FLedgers[0].GetId()));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestDeleteNonExistingItem()
      {
         return Task.FromResult(0);
      }
   }
}

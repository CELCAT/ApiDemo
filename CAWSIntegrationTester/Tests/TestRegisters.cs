using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;
using Celcat.Models;

namespace CAWSIntegrationTester.Tests
{
   class TestRegisters : TestBase
   {
      private const int MAX_REG_COUNT = 10;
      private List<Register> FRegisters;

      public Register[] Registers
      {
         get { return FRegisters.ToArray(); }
      }

      public TestRegisters(HttpClient client, Uri baseUri) 
         : base(client, baseUri)
      {
      }


      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "Registers");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.BadRequest);
      }

      // todo: create more filter tests with different types of filters
      protected override async Task TestGetWithFilter()
      {
         await GetRegisterIdsForDay(3, 3);
      }

      protected override async Task TestGetItem()
      {
         IEnumerable<string> regIds = await GetRegisterIds();

         // store some registers for use in later test...
         FRegisters = new List<Register>();

         int numWithActivities = 0;
         int numWithoutActivities = 0;

         foreach (var regId in regIds)
         {
            Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", regId));

            var response = await FClient.GetAsync(uri);
            await CheckSuccessStatusCode(response);

            var reg = await response.Content.ReadAsAsync<Register>();
            Trace.Assert(reg.GetId() == regId);

            // ensure that we get some registers with activities and some without...
            
            if (reg.ActivityId > 0)
            {
               if (numWithActivities < MAX_REG_COUNT / 2)
               {
                  ++numWithActivities;
                  FRegisters.Add(reg);
               }
            }
            else
            {
               if (numWithoutActivities < MAX_REG_COUNT / 2)
               {
                  ++numWithoutActivities;
                  FRegisters.Add(reg);
               }
            }

            if (numWithActivities + numWithoutActivities == MAX_REG_COUNT)
            {
               break;
            }
         }

         if (FRegisters.Count == 0)
         {
            throw new Exception("Could not find any registers for test!");
         }
      }

      protected override async Task TestGetNonExistingItem()
      {
         // nb - last 2 digits represent week number
         {
            const string BAD_ID = "12345604";
            Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", BAD_ID));
            var response = await FClient.GetAsync(uri);
            await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
         }

         await TestGetItemBadRegNumber();
      }

      protected async Task TestGetItemBadRegNumber()
      {
         // nb - here we supply an invalid reg number (week 74!)
         const string BAD_ID = "12345674";
         Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", BAD_ID));
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.InternalServerError);
      }

      protected override async Task TestPost()
      {
         var reg = FRegisters[0];
         Uri uri = new Uri(FBaseUri, "Registers");
         var response = await FClient.PostAsJsonAsync(uri.ToString(), reg);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPostDuplicate()
      {
         // unused
         return Task.FromResult(0);
      }

      protected override async Task TestPut()
      {
         var regWith = GetRegisterWithActivity();
         if (regWith == null)
         {
            throw new Exception("Could not find register with activity!");
         }

         var regWithout = GetRegisterWithoutActivity();
         if (regWithout == null)
         {
            throw new Exception("Could not find register without activity!");
         }

         await TestPut1(regWith, true);
         await TestPut1(regWithout, false);
         await TestPut2();
      }

      protected override async Task TestPutAtRoot()
      {
         if (FRegisters.Count > 0)
         {
            var reg = FRegisters[0];
            reg.Modified = true;

            Uri uri = new Uri(FBaseUri, "Registers");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), reg);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPutNonExistingItem()
      {
         if (FRegisters.Count > 0)
         {
            var reg = FRegisters[0];

            int origEventId = reg.EventId;
            int origWeek = reg.Week;

            reg.RegisterNotes = GetRandomString(50, true);
            reg.Modified = true;
            
            reg.SetId(123456, 4);

            Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", reg.GetId()));

            var response = await FClient.PutAsJsonAsync(uri.ToString(), reg);

            reg.EventId = origEventId;
            reg.Week = origWeek;

            await CheckStatusCodeIs(response, HttpStatusCode.InternalServerError);  // week or event ID not valid
         }
      }

      protected override async Task TestDelete()
      {
         if (FRegisters.Count > 0)
         {
            var reg = FRegisters[0];
            Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", reg.GetId()));
            var response = await FClient.DeleteAsync(uri.ToString());
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override Task TestDeleteNonExistingItem()
      {
         // unused
         return Task.FromResult(0);
      }

      private Register GetRegisterWithActivity()
      {
         return FRegisters.FirstOrDefault(x => x.ActivityId > 0);
      }

      private Register GetRegisterWithoutActivity()
      {
         return FRegisters.FirstOrDefault(x => x.ActivityId == 0);
      }

      private async Task TestPut1(Register reg, bool activityExistsInOriginal)
      {
         if (reg != null)
         {
            // change register notes...
            reg.RegisterNotes = GetRandomString(50, true);
            reg.Modified = true;

            Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", reg.GetId()));

            var response = await FClient.PutAsJsonAsync(uri.ToString(), reg);
            await CheckSuccessStatusCode(response);

            var regRetVal = await response.Content.ReadAsAsync<Register>();
            regRetVal.CheckSameValues(reg, activityExistsInOriginal);
         }
      }

      private async Task TestPut2()
      {
         var reg = FindRegisterWithStudentMarks();
         if(reg == null)
         {
            throw new Exception("Could not find register with student marks!");
         }

         var sm = reg.StudentMarks[0];
         sm.Comments = GetRandomString(40, true);
         sm.Modified = true;

         Uri uri = new Uri(FBaseUri, string.Format("Registers/{0}", reg.GetId()));
         var response = await FClient.PutAsJsonAsync(uri.ToString(), reg);
         await CheckSuccessStatusCode(response);

         var regRetVal = await response.Content.ReadAsAsync<Register>();
         regRetVal.CheckSameValues(reg, reg.ActivityId > 0);
      }

      private Register FindRegisterWithStudentMarks()
      {
         return FRegisters.FirstOrDefault(r => r.StudentMarks.Count > 0);
      }

      private AwsRegisterFilterPoco CreateSingleDayFilter(int day, int week)
      {
         // filter on day and week...

         AwsRegisterFilterPoco result = new AwsRegisterFilterPoco();

         result.DowFilter[day] = true;
         result.WeekFilter[week] = true;
         
         return result;
      }

      private async Task<IEnumerable<string>> GetRegisterIdsForDay(int day, int week)
      {
         List<string> result = new List<string>();

         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "RegisterSummaries"), CreateSingleDayFilter(day, week));

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);
         var items = await response.Content.ReadAsAsync<RegisterSummary[]>();
         result.AddRange(items.Select(item => item.GetId()));

         return result.ToArray();
      }
   
      private async Task<IEnumerable<string>> GetRegisterIds()
      {
         var results1 = await GetRegisterIdsForDay(3, 3);
         var results2 = await GetRegisterIdsForDay(4, 4);

         List<string> result = new List<string>();
         result.AddRange(results1);
         result.AddRange(results2);

         var arr = result.ToArray();
         Shuffle(arr);
         return arr;
      }




   }
}

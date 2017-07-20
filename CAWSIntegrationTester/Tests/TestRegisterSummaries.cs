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
   class TestRegisterSummaries : TestBase
   {
      private const int MAX_REG_COUNT = 20;
      private List<RegisterSummary> FRegSummaries;

      public RegisterSummary[] RegSummaries
      {
         get { return FRegSummaries.ToArray(); }
      }

      public DateTime[] SomeRegisterDates
      {
         get
         {
            return FRegSummaries.Select(reg => reg.StartDateTime.Date).ToArray();
         }
      }

      public TestRegisterSummaries(HttpClient client, Uri baseUri) 
         : base(client, baseUri)
      {
         
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "RegisterSummaries");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.BadRequest);
      }

      // todo: create more filter tests with different types of filters
      protected override async Task TestGetWithFilter()
      {
         const int DAY = 3;
         const int WEEK = 3;

         await TestGetWithFilter(DAY, WEEK);
      }

      public async Task TestGetWithFilter(int day, int week)
      {
         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, "RegisterSummaries"), CreateSingleDayFilter(day, week));

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);

         var items = await response.Content.ReadAsAsync<RegisterSummary[]>();

         var arr = items.ToArray();
         Shuffle(arr);

         // store for use in later test...
         FRegSummaries = new List<RegisterSummary>();
         FRegSummaries.AddRange(arr.Take(MAX_REG_COUNT));

         if (FRegSummaries.Count == 0)
         {
            throw new Exception("Could not find any register summaries for test!");
         }
      }

      protected override async Task TestGetItem()
      {
         foreach (var r in FRegSummaries)
         {
            RegisterSummary reg = await TestGetIndividualRegisterSummary(r.Id);
            reg.CheckSameValues(r);
         }
      }

      protected override async Task TestGetNonExistingItem()
      {
         {
            const string BAD_ID = "12345604";
            Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", BAD_ID)),
               CreateRegisterSummariesFilterBasic());
            var response = await FClient.GetAsync(uri);
            await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
         }

         {
            // this  bad ID has an invalid week number
            const string BAD_ID = "12345699";
            Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", BAD_ID)),
               CreateRegisterSummariesFilterBasic());
            var response = await FClient.GetAsync(uri);
            await CheckStatusCodeIs(response, HttpStatusCode.InternalServerError);
         }
      }

      protected override async Task TestPost()
      {
         var summary = FRegSummaries[0];

         Uri uri = new Uri(FBaseUri, "RegisterSummaries");
         var response = await FClient.PostAsJsonAsync(uri.ToString(), summary);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }

      protected override Task TestPostDuplicate()
      {
         // unused
         return Task.FromResult(0);
      }

      protected override async Task TestPutAtRoot()
      {
         if (FRegSummaries.Count > 0)
         {
            var summary = FRegSummaries[0];
            summary.StartDateTime = summary.StartDateTime.AddHours(-1);
            
            Uri uri = new Uri(FBaseUri, "RegisterSummaries");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), summary);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPut()
      {
         if (FRegSummaries.Count > 0)
         {
            var summary = FRegSummaries[0];
            summary.StartDateTime = summary.StartDateTime.AddHours(-1);

            Uri uri = new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", summary.GetId()));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), summary);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPutNonExistingItem()
      {
         if (FRegSummaries.Count > 0)
         {
            var summary = FRegSummaries[0];

            string origId = summary.Id;
            summary.Id = "12345678";

            Uri uri = new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", summary.GetId()));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), summary);
            summary.Id = origId;
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestDelete()
      {
         if (FRegSummaries.Count > 0)
         {
            var summary = FRegSummaries[0];
            Uri uri = new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", summary.GetId()));
            var response = await FClient.DeleteAsync(uri.ToString());
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestDeleteNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", BAD_ID));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
      }
      
      private async Task<RegisterSummary> TestGetIndividualRegisterSummary(string id)
      {
         Uri uri = GetUriWithFilterAdded(new Uri(FBaseUri, string.Format("RegisterSummaries/{0}", id)), CreateRegisterSummariesFilterBasic());

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);
         return await response.Content.ReadAsAsync<RegisterSummary>();
      }

      private AwsRegisterFilterBasicPoco CreateRegisterSummariesFilterBasic()
      {
         AwsRegisterFilterBasicPoco result = new AwsRegisterFilterBasicPoco();

         result.RegisterListInfo.Add(TctEntity.TypeId.Module);
         result.RegisterListInfo.Add(TctEntity.TypeId.Room);
         result.RegisterListInfo.Add(TctEntity.TypeId.Group);
         result.RegisterListInfo.Add(TctEntity.TypeId.Course);

         return result;
      }

      private AwsRegisterFilterPoco CreateSingleDayFilter(int day, int week)
      {
         // filter on day and week...

         AwsRegisterFilterPoco result = new AwsRegisterFilterPoco();

         result.DowFilter[day] = true;
         result.WeekFilter[week] = true;

         result.RegisterListInfo.Add(TctEntity.TypeId.Module);
         result.RegisterListInfo.Add(TctEntity.TypeId.Room);
         result.RegisterListInfo.Add(TctEntity.TypeId.Group);
         result.RegisterListInfo.Add(TctEntity.TypeId.Course);

         return result;
      }
      

   }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;

namespace CAWSIntegrationTester.Tests
{
   class TestOLA : TestBase
   {
      private List<OlaMark> FNewlyInsertedOLAMarks;

      public TestOLA(HttpClient client, Uri baseUri) 
         : base(client, baseUri)
      {
         
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "OLAStaging");
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);  // not BadRequest since there is no way to use GET on root (there is no way to specify a filter for example)
      }

      protected override Task TestGetWithFilter()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override Task TestGetItem()
      {
         // nothing to do (we don't know which records are there yet
         // so we test GET from within the testPost function)
         return Task.FromResult(0);
      }

      protected override async Task TestGetNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", BAD_ID));
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }

      protected override async Task TestPost()
      {
         FNewlyInsertedOLAMarks = new List<OlaMark>();
         
         Uri uri = new Uri(FBaseUri, "OLAStaging");

         int numIters = FRandom.Next(4, 10);

         for (int n = 0; n < numIters; ++n)
         {
            var newDefn = new OlaMark
            {
               Session = FRandom.Next(),
               MarkStamp = GetRandomDate(),
               MarkAbbrev = GetRandomString(1, false),
               MarkingUserId = FRandom.Next(),
               MarkId = FRandom.Next(),
               Student = GetRandomString(FRandom.Next(1, 129), true),
               StudentContext = 1,
               Room = GetRandomString(FRandom.Next(1, 129), true),
               RoomContext = 1,
               Status = FRandom.Next(),
               Uploaded = DateTime.Now,
               Processed = FRandom.Next(0, 100)
            };

            if(FRandom.Next(0, 2) == 1)
            {
               newDefn.StudentId = FRandom.Next();
            }

            if(FRandom.Next(0, 2) == 1)
            {
               newDefn.RoomId = FRandom.Next();
            }

            if(FRandom.Next(0, 2) == 1)
            {
               newDefn.EventId = FRandom.Next();
            }

            if(FRandom.Next(0, 2) == 1)
            {
               newDefn.Week = FRandom.Next();
            }

            var response = await FClient.PostAsJsonAsync(uri.ToString(), newDefn);
            await CheckStatusCodeIs(response, HttpStatusCode.Created);

            var md = await response.Content.ReadAsAsync<OlaMark>();
            md.CheckSameValues(newDefn);

            FNewlyInsertedOLAMarks.Add(md);
         }

         // perform the GETItem check here...
         foreach (var m in FNewlyInsertedOLAMarks)
         {
            uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", m.Id));

            var response = await FClient.GetAsync(uri);
            await CheckSuccessStatusCode(response);
            var md = await response.Content.ReadAsAsync<OlaMark>();

            md.CheckSameValues(m);
         }
      }

      protected override async Task TestPostDuplicate()
      {
         if (FNewlyInsertedOLAMarks.Count > 0)
         {
            Uri uri = new Uri(FBaseUri, "OLAStaging");

            // try to add a defn that already exists...
            var md = FNewlyInsertedOLAMarks[0];
            var response = await FClient.PostAsJsonAsync(uri.ToString(), md);
            await CheckStatusCodeIs(response, HttpStatusCode.InternalServerError);
         }
      }

      protected override async Task TestPutAtRoot()
      {
         if (FNewlyInsertedOLAMarks.Count > 0)
         {
            var m = FNewlyInsertedOLAMarks[0];
            m.Session = FRandom.Next();
            
            Uri uri = new Uri(FBaseUri, "OLAStaging");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), m);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPut()
      {
         // make changes to the newly added defs...

         foreach (var md in FNewlyInsertedOLAMarks)
         {
            // change session...
            md.Session = FRandom.Next();

            Uri uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", md.Id));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), md);
            await CheckSuccessStatusCode(response);

            var mdRetVal = await response.Content.ReadAsAsync<OlaMark>();
            md.CheckSameValues(mdRetVal);
         }
      }

      protected override async Task TestPutNonExistingItem()
      {
         // make changes to a non-existing def...

         if (FNewlyInsertedOLAMarks.Count > 0)
         {
            var md = FNewlyInsertedOLAMarks[0];
            int origId = md.Id;
            md.Id = BAD_INT_ID;
            Uri uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", md.Id));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), md);
            md.Id = origId;
            await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
         }
      }

      protected override async Task TestDelete()
      {
         // remove the newly inserted defs...
         foreach (var md in FNewlyInsertedOLAMarks)
         {
            Uri uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", md.GetId()));
            var response = await FClient.DeleteAsync(uri.ToString());
            await CheckStatusCodeIs(response, HttpStatusCode.NoContent);
         }
      }

      protected override async Task TestDeleteNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("OLAStaging/{0}", BAD_ID));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }
   }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CAWSIntegrationTester.Models;

namespace CAWSIntegrationTester.Tests
{
   public class TestMarkDefinitions : TestBase
   {
      private List<MarkDefinition> FExistingDefinitions;
      private List<MarkDefinition> FNewlyInsertedDefinitions;
      private const string POSSIBLE_SHORTCUT_KEYS = "abcdefghijklmnopqrstuvwxyz0123456789";

      public TestMarkDefinitions(HttpClient client, Uri baseUri)
         : base(client, baseUri)
      {

      }

      public MarkDefinition[] MarkDefinitions
      {
         get { return FExistingDefinitions.ToArray(); }
      }

      protected override async Task TestGetAll()
      {
         Uri uri = new Uri(FBaseUri, "MarkDefinitions");

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);
         
         var items = await response.Content.ReadAsAsync<MarkDefinition[]>();
         FExistingDefinitions = new List<MarkDefinition>();

         foreach (var item in items)
         {
            // store the existing mark definitions to assist with 
            // the Insert test...
            FExistingDefinitions.Add(item);
         }
      }

      protected override Task TestGetWithFilter()
      {
         // nothing to do
         return Task.FromResult(0);
      }

      protected override async Task TestGetItem()
      {
         foreach (var md in FExistingDefinitions)
         {
            MarkDefinition md2 = await GetItem(md.GetId());
            md.CheckSameValues(md2);
         }
      }

      protected override async Task TestGetNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", BAD_ID));
         var response = await FClient.GetAsync(uri);
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }

      protected override async Task TestPost()
      {
         FNewlyInsertedDefinitions = new List<MarkDefinition>();

         Uri uri = new Uri(FBaseUri, "MarkDefinitions");

         int numIters = FRandom.Next(4, 10);

         for (int n = 0; n < numIters; ++n)
         {
            string notifyText = GetRandomStringOrEmpty(FRandom.Next(10, 255), true);

            var newDefn = new MarkDefinition
            {
               Abbreviation = GetUniqueAbbreviation(),
               Name = GetUniqueName(),
               Card = false,
               Colour = GetRandomColour(),
               Definition = GetRandomDef(),
               Description = GetRandomStringOrEmpty(FRandom.Next(10, 128), true),
               Notification = notifyText,
               Notify = !string.IsNullOrWhiteSpace(notifyText),
               OriginId = null,
               OriginalId = string.Empty,
               Precedence = false,
               ShortcutKey = GetUniqueShortcutKey()
            };

            var response = await FClient.PostAsJsonAsync(uri.ToString(), newDefn);
            await CheckStatusCodeIs(response, HttpStatusCode.Created);
            
            var md = await response.Content.ReadAsAsync<MarkDefinition>();
            md.CheckSameValues(newDefn);

            FNewlyInsertedDefinitions.Add(md);
         }
      }

      protected override async Task TestPostDuplicate()
      {
         if (FNewlyInsertedDefinitions.Count > 0)
         {
            Uri uri = new Uri(FBaseUri, "MarkDefinitions");

            // try to add a defn that already exists...
            var md = FNewlyInsertedDefinitions[0];
            var response = await FClient.PostAsJsonAsync(uri.ToString(), md);
            await CheckStatusCodeIs(response, HttpStatusCode.InternalServerError);  // we left md.Id non-zero

            int origId = md.Id;
            md.Id = 0;

            response = await FClient.PostAsJsonAsync(uri.ToString(), md);
            await CheckStatusCodeIs(response, HttpStatusCode.Conflict);  

            md.Id = origId;
         }
      }

      protected override async Task TestPutAtRoot()
      {
         if(FNewlyInsertedDefinitions.Count > 0)
         {
            var md = FNewlyInsertedDefinitions[0];
            md.Description = GetRandomString(50, true);

            Uri uri = new Uri(FBaseUri, "MarkDefinitions");
            var response = await FClient.PutAsJsonAsync(uri.ToString(), md);
            await CheckStatusCodeIs(response, HttpStatusCode.MethodNotAllowed);
         }
      }

      protected override async Task TestPut()
      {
         // make changes to the newly added defs...

         foreach (var md in FNewlyInsertedDefinitions)
         {
            // change description...
            md.Description = GetRandomString(50, true);

            Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", md.Id));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), md);
            await CheckSuccessStatusCode(response);

            var mdRetVal = await response.Content.ReadAsAsync<MarkDefinition>();
            md.CheckSameValues(mdRetVal);
         }
      }

      protected override async Task TestPutNonExistingItem()
      {
         // make changes to a non-existing def...

         if (FNewlyInsertedDefinitions.Count > 0)
         {
            var md = FNewlyInsertedDefinitions[0];
            int origId = md.Id;
            md.Id = BAD_INT_ID;
            Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", md.Id));
            var response = await FClient.PutAsJsonAsync(uri.ToString(), md);
            md.Id = origId;
            await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
         }
      }

      protected override async Task TestDelete()
      {
         // remove the newly inserted defs...
         foreach (var md in FNewlyInsertedDefinitions)
         {
            Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", md.GetId()));
            var response = await FClient.DeleteAsync(uri.ToString());
            await CheckStatusCodeIs(response, HttpStatusCode.NoContent);
         }
      }

      protected override async Task TestDeleteNonExistingItem()
      {
         const string BAD_ID = "12345678";
         Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", BAD_ID));
         var response = await FClient.DeleteAsync(uri.ToString());
         await CheckStatusCodeIs(response, HttpStatusCode.NotFound);
      }
      
      private string GetUniqueShortcutKey()
      {
         bool allShortcutKeysInUse = true;
         foreach(var key in POSSIBLE_SHORTCUT_KEYS)
         {
            bool found = FExistingDefinitions.Any(
               md => md.ShortcutKey.Equals(key.ToString(CultureInfo.InvariantCulture), 
                  StringComparison.OrdinalIgnoreCase));

            if(!found)
            {
               found = FNewlyInsertedDefinitions.Any(
                  md => md.ShortcutKey.Equals(key.ToString(CultureInfo.InvariantCulture), 
                     StringComparison.OrdinalIgnoreCase));

               if (!found)
               {
                  allShortcutKeysInUse = false;
                  break;
               }
            }
         }

         if(allShortcutKeysInUse)
         {
            throw new Exception("All shortcut keys are in use! You should revert to the built-in mark definitions and then rerun the test");
         }
         
         return GetUniqueString(1, false, FExistingDefinitions.Select(x => x.ShortcutKey).Union(FNewlyInsertedDefinitions.Select((x => x.ShortcutKey))));
      }

      private string GetRandomDef()
      {
         const string DEFS = "PAL";
         char ch = DEFS[FRandom.Next(DEFS.Count())];
         return ch.ToString(CultureInfo.InvariantCulture);
      }

      private string GetUniqueName()
      {
         return GetUniqueString(FRandom.Next(2, 10), true, FExistingDefinitions.Select(x => x.Name).Union(FNewlyInsertedDefinitions.Select(x => x.Name)));
      }

      private string GetUniqueAbbreviation()
      {
         return GetUniqueString(FRandom.Next(2, 10), false, FExistingDefinitions.Select(x => x.Abbreviation).Union(FNewlyInsertedDefinitions.Select(x => x.Name)));
      }

      private async Task<MarkDefinition> GetItem(string defId)
      {
         Uri uri = new Uri(FBaseUri, string.Format("MarkDefinitions/{0}", defId));

         var response = await FClient.GetAsync(uri);
         await CheckSuccessStatusCode(response);
         return await response.Content.ReadAsAsync<MarkDefinition>();
      }

   }
}

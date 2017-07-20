using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CAWSIntegrationTester.Tests
{
   public abstract class TestBase : TestProgress
   {
      protected HttpClient FClient;
      protected Uri FBaseUri;
      protected Random FRandom;
      protected const int BAD_INT_ID = Int32.MaxValue;
      private bool FPassed;

      protected TestBase(HttpClient client, Uri baseUri)
      {
         FClient = client;
         FBaseUri = baseUri;
         FRandom = new Random();
      }

      // GET...
      protected abstract Task TestGetAll();
      protected abstract Task TestGetWithFilter();
      protected abstract Task TestGetItem();
      protected abstract Task TestGetNonExistingItem();

      // POST (insert)...
      protected abstract Task TestPost();
      protected abstract Task TestPostDuplicate();

      // PUT (update)...
      protected abstract Task TestPut();
      protected abstract Task TestPutNonExistingItem();
      protected abstract Task TestPutAtRoot();

      // DELETE...
      protected abstract Task TestDelete();
      protected abstract Task TestDeleteNonExistingItem();


      protected void Shuffle<T>(T[] array)
      {
         int n = array.Length;
         while (n > 1)
         {
            int k = FRandom.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
         }
      }

      protected DateTime GetRandomDate()
      {
         return DateTime.Now.AddDays(FRandom.Next(-50, 50)).
                  AddHours(FRandom.Next(0, 10)).
                     AddMinutes(FRandom.Next(0, 30));
      }

      protected int GetRandomColour()
      {
         return FRandom.Next(16777215);
      }

      protected string GetUniqueString(int numChars, bool allowSpaces, IEnumerable<string> tabuList)
      {
         string tryString = GetRandomString(numChars, allowSpaces);

         var enumerable = tabuList as string[] ?? tabuList.ToArray();
         while (enumerable.Contains(tryString, StringComparer.OrdinalIgnoreCase))
         {
            tryString = GetRandomString(numChars, allowSpaces);
         }

         return tryString;
      }

      protected string GetRandomString(int numChars, bool allowSpaces)
      {
         string replacement = allowSpaces ? " " : "A";
         byte[] randBuffer = new byte[numChars * 3];
         RandomNumberGenerator.Create().GetBytes(randBuffer);

         return Convert.ToBase64String(randBuffer).
            Replace("/", replacement).
            Replace("=", replacement).
            Replace("+", replacement).Remove(numChars);
      }

      protected string GetRandomStringOrEmpty(int numChars, bool allowSpaces)
      {
         if (FRandom.Next(0, 2) == 1)
         {
            return string.Empty;
         }

         return GetRandomString(numChars, allowSpaces);
      }

      protected void CheckNotNull(object o)
      {
         if (o == null)
         {
            throw new Exception("Unexpected failed operation");
         }
      }

      protected async Task CheckStatusCodeIs(HttpResponseMessage response, HttpStatusCode code)
      {
         if (response == null)
         {
            throw new Exception("Response object is null");
         }

         if (response.StatusCode != code)
         {
            var responseText = await response.Content.ReadAsStringAsync();
            if (responseText != null)
            {
               responseText = responseText.Replace("{", "{{").Replace("}", "}}");
            }

            throw new Exception(string.Format("Unexpected response status code: {0} ({1})", 
               response.StatusCode, responseText ?? "-"));
         }
      }

      protected async Task CheckSuccessStatusCode(HttpResponseMessage response)
      {
         await CheckStatusCodeIs(response, HttpStatusCode.OK);
      }

      protected Uri GetUriWithFilterAdded(Uri uri, object filter)
      {
         if (filter != null)
         {
            JavaScriptSerializer j = new JavaScriptSerializer();

            StringBuilder sb = new StringBuilder(uri.ToString().TrimEnd('/'));
            sb.Append("?jsonparam=");
            sb.Append(HttpUtility.UrlEncode(j.Serialize(filter)));

            return new Uri(sb.ToString());
         }

         return uri;
      }

      public async Task<bool> Run()
      {
         FPassed = true;

         Progress(); // blank line
         Type tp = GetType();
         Progress(string.Format("Test class: {0}", tp.Name));

         // GET...
         ProgressIndented(2, "Testing GET verb");

         await DoTest("GET", TestGetAll);
         await DoTest("GET filtered", TestGetWithFilter);
         await DoTest("GET item", TestGetItem);
         await DoTest("GET non-existing item", TestGetNonExistingItem);
         
         // POST (insert)...
         ProgressIndented(2, "Testing POST verb");

         await DoTest("POST", TestPost);
         await DoTest("Post duplicate", TestPostDuplicate);

         // PUT (update)...
         ProgressIndented(2, "Testing PUT verb");

         await DoTest("PUT", TestPut);
         await DoTest("PUT non-existing item", TestPutNonExistingItem);
         await DoTest("PUT at root", TestPutAtRoot);

         // DELETE...
         ProgressIndented(2, "Testing DELETE verb");

         await DoTest("DELETE", TestDelete);
         await DoTest("DELETE non-existing item", TestDeleteNonExistingItem);

         return FPassed;
      }

      private async Task DoTest(string testTitle, Func<Task> test)
      {
         ProgressIndented(4, testTitle);

         try
         {
            await test();
         }
         catch(Exception ex)
         {
            FPassed = false;
            ProgressIndented(6, string.Format("error - {0}", ex.Message));
         }
      }
   }
}

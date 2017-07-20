using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CAWSIntegrationTester.Auth;
using CAWSIntegrationTester.Tests;


namespace CAWSIntegrationTester
{
   class MainApp
   {
      private readonly Uri FBaseUri;
      private bool FPassed;

      public MainApp(Uri baseUri)
      {
         FBaseUri = baseUri;
         TrustAllCertificates();
      }

      private void TrustAllCertificates()
      {
         ServicePointManager.ServerCertificateValidationCallback =
            ((sender, certificate, chain, sslPolicyErrors) => true);
      }

      private string GetOAuthToken(string ttName, string username, string password, string userRole)
      {
         Uri tokenUri = new Uri(FBaseUri, "token");
         return OAuthToken.GetToken(tokenUri, username, password, ttName, userRole);
      }

      public bool Execute(string ttName, string username, string password, string userRole)
      {
         FPassed = true;
         InternalExecute(ttName, username, password, userRole).Wait();
         return FPassed;
      }

      // EXECUTE TESTS
      private async Task InternalExecute(string ttName, string username, string password, string userRole)
      {
         try
         {
            string token = GetOAuthToken(ttName, username, password, userRole);

            using (HttpClient client = new HttpClient())
            {
               client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

               // Mark Definitions...
               TestMarkDefinitions test1 = new TestMarkDefinitions(client, FBaseUri);
               FPassed = await test1.Run() && FPassed;

               // Register Summaries...
               TestRegisterSummaries test2 = new TestRegisterSummaries(client, FBaseUri);
               FPassed = await test2.Run() && FPassed;

               // Registers...
               TestRegisters test3 = new TestRegisters(client, FBaseUri);
               FPassed = await test3.Run() && FPassed;

               // Student Ledgers...
               TestStudentLedger test4 = new TestStudentLedger(client, FBaseUri, test3.Registers);
               FPassed = await test4.Run() && FPassed;

               // Student Marks...
               TestStudentMarks test5 = new TestStudentMarks(client, FBaseUri, test3.Registers);
               FPassed = await test5.Run() && FPassed;

               // Student Exceptions...
               TestStudentExceptions test6 = 
                  new TestStudentExceptions(client, FBaseUri, test3.Registers, test2.SomeRegisterDates, test1.MarkDefinitions);
               FPassed = await test6.Run() && FPassed;

               // OLA Staging...
               TestOLA test7 = new TestOLA(client, FBaseUri);
               FPassed = await test7.Run() && FPassed;

               // Student in/out times
               TestStudentTimes test8 = new TestStudentTimes(client, FBaseUri, test3.Registers);
               FPassed = await test8.Run() && FPassed;

               TestConcurrency finalTest = new TestConcurrency(client, FBaseUri);
               finalTest.Run();
            }
         }

         catch (WebException wex)
         {
            FPassed = false;

            var resp = wex.Response;
            if (resp != null)
            {
               var responseStream = resp.GetResponseStream();
               if (responseStream != null)
               {
                  using (var reader = new StreamReader(responseStream))
                  {
                     Console.WriteLine(wex.Message + " " + reader.ReadToEnd());
                  }
               }
            }
         }

         catch (Exception ex)
         {
            FPassed = false;
            Console.WriteLine(ex.Message);
         }
      }


   }
}

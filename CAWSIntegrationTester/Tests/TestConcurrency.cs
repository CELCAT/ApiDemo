using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CAWSIntegrationTester.Tests
{
   class TestConcurrency : TestProgress
   {
      protected HttpClient FClient;
      protected Uri FBaseUri;
      private readonly Random FRandom;

      public TestConcurrency(HttpClient client, Uri baseUri)
      {
         FClient = client;
         FBaseUri = baseUri;
         FRandom = new Random();
      }

      public void Run()
      {
         Progress(); // blank line
         Type tp = GetType();
         Progress(string.Format("Test class: {0}", tp.Name));


         const int NUM_ITERS = 50;
         Parallel.For(0, NUM_ITERS, i => SingleRun());
      }

      private void SingleRun()
      {
         var test = new TestRegisterSummaries(FClient, FBaseUri);
         test.TestGetWithFilter(FRandom.Next(0, 7), FRandom.Next(0, 20)).Wait();  // assume at least 20 weeks in tt
      }
   }
}

using System;

namespace CAWSIntegrationTester
{
   class Program
   {
      //const string BASE_URI = @"https://localhost:44300/CAWS/";
      //const string TIMETABLE_ALIAS = "deleteme";
      //const string TIMETABLE_USER_NAME = "Administrator";
      //const string TIMETABLE_PASSWORD = "password";
      //const string USER_ROLE_NAME = "Administrator";

      static void Main(string[] args)
      {
         if (!ArgsOk(args))
         {
            ShowUsage();
            Environment.ExitCode = 2;
         }
         else
         {
            string baseUri = args[0];
            string timetableAlias = args[1].Trim();
            string userName = args[2].Trim();
            string pwd = args[3].Trim();
            string roleName = args[4].Trim();

            MainApp app = new MainApp(new Uri(baseUri));
            bool passed = app.Execute(timetableAlias, userName, pwd, roleName);
            
            if (!passed)
            {
               Console.WriteLine("");
               Console.WriteLine("Errors found. Press a key to finish");
               Console.ReadKey();
               Environment.ExitCode = 1;
            }
         }
      }

      private static bool ArgsOk(string[] args)
      {
         if(args.Length != 5)
         {
            return false;
         }

         return true;
      }

      private static void ShowUsage()
      {
         Console.WriteLine("USAGE: CAWSIntegrationTester [Url] [Alias] [User] [Pwd] [Role]");
         Console.WriteLine();
         Console.WriteLine("'Url' is the address of the web service, e.g. https://webserver/CAWS");
         Console.WriteLine("'Alias' is the timetable alias name as define in the CAWS config tool");
         Console.WriteLine("'User' is the timetabler user name");
         Console.WriteLine("'Pwd' is the user password");
         Console.WriteLine("'Role' is the name of the user role");
      }
   }
}

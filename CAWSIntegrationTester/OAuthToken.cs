using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CAWSIntegrationTester
{
   internal static class OAuthToken
   {
      static public string GetToken(Uri tokenUri, string username, string password, string timetableName, string roleName)
      {
         string result = null;
      
         StringBuilder data = new StringBuilder();
         data.Append("grant_type=password");
         data.AppendFormat("&username={0}", Uri.EscapeDataString(username));
         data.AppendFormat("&password={0}", Uri.EscapeDataString(password));
         data.AppendFormat("&Timetable={0}", Uri.EscapeDataString(timetableName));
         data.AppendFormat("&Role={0}", Uri.EscapeDataString(roleName));

         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUri);
         request.Method = "POST";
         request.ContentType = "application/x-www-form-urlencoded";

         using (Stream postStream = request.GetRequestStream())
         using (StreamWriter writer = new StreamWriter(postStream, Encoding.ASCII))
         {
            writer.Write(data);
         }

         using (var response = (HttpWebResponse)request.GetResponse())
         using (var rs = response.GetResponseStream())
         {
            if (rs != null)
            {
               using (StreamReader reader = new StreamReader(rs))
               {
                  string json = reader.ReadLine();
                  if (json != null)
                  {
                     JavaScriptSerializer ser = new JavaScriptSerializer();
                     Dictionary<string, object> x = (Dictionary<string, object>)ser.DeserializeObject(json);
                     result = x["access_token"].ToString();
                  }
               }
            }
         }

         return result;
      }

   }
}

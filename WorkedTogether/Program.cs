using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CredentialManagement;

namespace WorkedTogether
{
    class Program
    {
        public static Credential proxyCredential;

        


        public static string GetProxy(string url)
        {
            WebProxy proxy = WebProxy.GetDefaultProxy();
            //WebProxy proxy = new WebProxy();
            Uri resource = new Uri(url);
            Uri proxiedUri = proxy.GetProxy(resource);
            string proxiedHost = proxiedUri.Host;
            string resourceHost = resource.Host;

            if (resourceHost != proxiedHost)
            {
                return proxiedHost;
            }
            else
            {
                return null;
            }

        }
        public static void initProxyCredentials()
        {
            proxyCredential = new Credential();
        }
        public static void GetProxyCredentials(string proxiedHost)
        {
            CredentialSet set = new CredentialSet();
            set.Load();
            if (set.Count > 0)
            {
                Credential[] credentialList = set.ToArray();
                foreach (Credential c in credentialList)
                {
                    //if (c.Target == proxyHost)
                    if (c.Type == CredentialType.Generic && c.Target == proxiedHost)
                    //if (c.Type == CredentialType.Generic )
                    {
                        Console.WriteLine("Target: " + c.Target);
                        Console.WriteLine("Username: " + c.Username);
                        if (c.Password != null)
                        { Console.WriteLine("Password is ok"); }
                        else
                        {
                            Console.WriteLine("---No Password");
                        }

                        Console.WriteLine("Type: " + c.Type);
                        Console.WriteLine("generic credentials found");
                        Console.WriteLine("");


                        proxyCredential.Username = c.Username;
                        proxyCredential.Password = c.Password;
                        proxyCredential.Target = c.Target;
                        proxyCredential.Type = c.Type;

                    }
                    else
                    {
                        //just a placeholder
                    }
                }
            }
            else
            {
                Console.WriteLine("You are not athenticated yet.");
                Console.Read();
            }
        }

        //with out proxy
        public static int doWebRequest(string url)
        {
            int good = 200;
            int bad = 407;
            WebRequest req = WebRequest.Create(url);
            try
            {
                WebResponse response = req.GetResponse();

                if (response == null)
                {
                    return bad;
                }
                else
                {
                    Stream objStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(objStream);
                    string text = reader.ReadToEnd();
                    Console.Write(text);
                    return good;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Status);
                Console.WriteLine(ex.Response);
                Console.WriteLine(ex.StackTrace);
                return bad;
            }
            



        }

        //with Proxy
        public static int badRequest(string url, string user, string password, string proxyAddress)
        {
            int good = 200;
            int bad = 407;
            Stream objStream;
            try
            {
                WebProxy proxyObject = WebProxy.GetDefaultProxy();
                proxyObject.Credentials = new NetworkCredential(user, password);
                WebRequest req = WebRequest.Create(url);
                if (req != null)
                {
                    req.Proxy = proxyObject;
                    WebResponse response = req.GetResponse();
                    objStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(objStream);
                    string text = reader.ReadToEnd();
                    Console.Write(text);
                    Console.Read();
                    Console.WriteLine(good);
                    return good;
                }
                else
                {
                    Console.WriteLine("Check URL");
                    Console.Read();
                    return bad;
                }

            }
            catch (WebException e)
            {
                Console.WriteLine("Status: " + e.Status);
                Console.WriteLine("Message: " + e.Message);
                Console.WriteLine("Response: " + e.Response);
                Console.WriteLine("StackTrace: " + e.StackTrace);
                Console.WriteLine("Source: " + e.Source);
                Console.WriteLine("Helplink: " + e.HelpLink);
                //Console.ReadKey();
                //Console.WriteLine(bad);
                return bad;
            }
        }

        public static int do_request(string url, string user, string password, string proxyAddress)
        {
            int good = 200;
            int bad = 407;
            int result = 0;
            if (proxyAddress != null)
            {
                result = badRequest(url, user, password, proxyAddress);
                if (result == 200)
                    return good;
                else
                {
                    return bad;
                }
            }
            else
            {
                result = doWebRequest(url);

                if (result == 200)
                    return good;
                else
                {
                    return bad;
                }
            }
        }
        
       
        
        static void Main(string[] args)
        {

            string url = "https://cap.mobilit.fgov.be/FodMob/rest/candidate/titularis/7001705409?APIKey=3682a9ac-16ec-45f6-8dba-034a05326617";
            string url1 = "https://cap.mobilit.fgov.be/FodMob/rest/candidate/titularis/83082940341?APIKey=3682a9ac-16ec-45f6-8dba-034a05326617";
            //string url1 = "https://cap.mobilit.fgov.be/FodMob/rest/candidate/84031709318?APIKey=3682a9ac-16ec-45f6-8dba-034a05326617";
            string proxy = GetProxy(url1);
            initProxyCredentials();
            if (proxy != null)
            {
                GetProxyCredentials(proxy);
            }
            do_request(url1, proxyCredential.Username, proxyCredential.Password, proxy);
            Console.Read();
            Console.ReadKey();


        }

    }
}

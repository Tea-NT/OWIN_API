using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace OWIN_API
{
    class Program
    {
        static void Main(string[] args)
        {

            //if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            //{
            //    // RazorEngine cannot clean up from the default appdomain...
            //    Console.WriteLine("Switching to secound AppDomain, for RazorEngine...");
            //    AppDomainSetup adSetup = new AppDomainSetup();
            //    adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //    var current = AppDomain.CurrentDomain;
            //    // You only need to add strongnames when your appdomain is not a full trust environment.
            //    var strongNames = new StrongName[0];

            //    var domain = AppDomain.CreateDomain(
            //        "MyMainDomain", null,
            //        current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
            //        strongNames);
            //    var exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            //    // RazorEngine will cleanup. 
            //    AppDomain.Unload(domain);
            //    //return exitCode;
            //}
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();

            string baseAddress = "https://+:9001/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                //HttpClient client = new HttpClient();

                //var response = client.GetAsync("https://localhost:9001/" + "Fanuc/values/get").Result;

                //Console.WriteLine(response);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine(baseAddress);
                Console.ReadLine();
            }
        }
    }
}

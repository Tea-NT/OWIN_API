using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OWIN_API.Controller
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public HttpContent index()
        {
            string template = "<div>Hello @Model.Name! Welcome to Web API and Razor!</div>";
            string result = Razor.Parse(template, new { Name = "World" });
            var result1 = Engine.Razor.RunCompile(template, "templateKey", null, new { Name = "World" });
            StringContent stringContent= new StringContent(result1, System.Text.Encoding.UTF8, "text/html");
            return stringContent;
            //return result;
        }

        public IHttpActionResult Add()
        {
            return Ok();
        }
    }
}

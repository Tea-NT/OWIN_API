using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace OWIN_API
{
    public class Startup
    {
        private static string _siteDir="";

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = httpConfiguration();

            appBuilder.UseErrorPage();
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.MapSignalR();

            appBuilder.UseWebApi(config);
            //appBuilder.UseWelcomePage();

            //appBuilder.Use((context, fun) =>
            //{
            //    return myhandle(context, fun);
            //});

            appBuilder.Use<SampleMiddleware>();

        }

        public HttpConfiguration httpConfiguration()
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
               "Action Api", "Fanuc/{controller}/{action}/{id}",
               new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    "Default", "{controller}/{action}",
            //    new { controller = "Home", action = "Index" });
            //config.Routes.MapHttpRoute(
            //   "DefaultApi", "Fanuc/{controller}/{id}",
            //   new { id = RouteParameter.Optional }
            //);

            //config.Formatters.Clear();
            //config.Formatters.Add(new JsonMediaTypeFormatter());
            //config.Formatters.JsonFormatter.SerializerSettings =
            //new JsonSerializerSettings
            //{
            //    //ContractResolver = new DefaultContractResolver(),
            //    //Formatting = Formatting.Indented
            //    //StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            //};

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //默认返回 json
            config.Formatters
                .JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));
            //返回格式选择
            config.Formatters
                .XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "xml", "application/xml"));


            //json 序列化设置
            config.Formatters
                .JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

            return config;
        }

        public Task myhandle(IOwinContext context, Func<Task> next)
        {
            //获取物理文件路径
            var path = GetFilePath(context.Request.Path.Value);

            //验证路径是否存在
            //if ()
            //{
            //    return SetResponse(context, path);
            //}
            //else
            //{
            try
            {
                return SetResponse(context, path);
            }
            catch (Exception e)
            {
                throw e;
                return next();
            }
            //}

            //不存在返回下一个请求
        }

        public static string GetFilePath(string relPath)
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory
                , _siteDir
                , relPath.TrimStart('/').Replace('/', '\\'));
        }
        
        public Task SetResponse(IOwinContext context, string path)
        {
            var perfix = Path.GetExtension(path);
            if (perfix == ".html")
                context.Response.ContentType = "text/html; charset=utf-8";
            else if (perfix == ".js")
                context.Response.ContentType = "application/x-javascript";
            else if (perfix == ".js")
                context.Response.ContentType = "atext/css";
            return context.Response.WriteAsync(File.ReadAllText(path));
        }
    }

    /// <summary>
    /// 自定义中间件，继承OwinMiddleware
    /// </summary>
    public class SampleMiddleware : OwinMiddleware
    {
        //object m_Option;
        public SampleMiddleware(OwinMiddleware next):base(next)
        {
            //m_Option = m_Option;
        }
        public override Task Invoke(IOwinContext context)
        {
            var path = Startup.GetFilePath(context.Request.Path.Value);
            try
            {
                var perfix = Path.GetExtension(path);
                if (perfix == ".html")
                    context.Response.ContentType = "text/html; charset=utf-8";
                else if (perfix == ".js")
                    context.Response.ContentType = "application/x-javascript";
                else if (perfix == ".css")
                    context.Response.ContentType = "text/css";
                return context.Response.WriteAsync(File.ReadAllText(path));
                //return SetResponse(context, path);
            }
            catch (Exception e)
            {
                return Next.Invoke(context);
            }
        }
    }

    public class MyHub:Hub
    {
        public void Send(string name,string message)
        {
            var ipAddress = Context.Request.Environment["server.RemoteIpAddress"];
            Clients.All.addMessage(name, message+"     Ip地址:" +ipAddress);
        }
    }
}
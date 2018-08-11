using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Security.Claims;
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

            var hubConfig = new HubConfiguration();
            hubConfig.EnableDetailedErrors = true;
            appBuilder.MapSignalR(hubConfig);

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
                else if (perfix == ".png")
                { context.Response.ContentType = "image/png";
                    //var imgByte = File.ReadAllBytes(path);
                    //var resp = new HttpResponseMessage(HttpStatusCode.OK)
                    //{
                    //    Content = new ByteArrayContent(imgByte)
                    //    //或者
                    //    //Content = new StreamContent(stream)
                    //};
                    //return context.Response.Body
                }

                return context.Response.WriteAsync(File.ReadAllBytes(path));
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
        public static IHubCallerConnectionContext<dynamic>  GlobalHubClients;
        public MyHub() : base()
        {
            GlobalHubClients = Clients;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string name,string message)
        {
            var ipAddress = Context.Request.Environment["server.RemoteIpAddress"];
            IPAddress iPAddress = IPAddress.Parse(ipAddress.ToString());
            iPAddress.MapToIPv4().ToString();
            Clients.All.addMessage(name, message+"     Ip地址:" + iPAddress.MapToIPv4().ToString());
            Clients.Group("SignalR Users").addMessage("Group chanle", message);
        }

        /// <summary>
        /// 查询所有客户端
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClientInfo> GetAllClients()
        {
            var ipAddress = Context.Request.Environment["server.RemoteIpAddress"];
            string iPAddress = IPAddress.Parse(ipAddress.ToString()).MapToIPv4().ToString();
            if (ClientManager.db.ClientsInfo.Any(x => x.IpAddress == iPAddress))
                return ClientManager.db.ClientsInfo;
            else
                return null;
        }

        /// <summary>
        /// 增加客户端
        /// </summary>
        /// <param name="clientInfo"></param>
        /// <returns></returns>
        public string AddClient(ClientInfo clientInfo)
        {
            if (ClientManager.db.ClientsInfo.Any(x => x.IpAddress == clientInfo.IpAddress))
                return "已经添加过此IP地址，请勿重复添加";
            try
            {
                ClientManager.db.ClientsInfo.Add(clientInfo);
                ClientManager.db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "新增失败";
            }
            
            return "OK";
        }

        //ClientManager clientManager= new ClientManager();
        public override async Task OnConnected()
        {
           //clientManager = new ClientManager();

            var ipAddress = Context.Request.Environment["server.RemoteIpAddress"];
            IPAddress iPAddress = IPAddress.Parse(ipAddress.ToString());
            string tempip = iPAddress.MapToIPv4().ToString();
            //if (clientManager.IpList.Contains(iPAddress.MapToIPv4().ToString()))
            //if (clientManager.allClient.Any(x => x.IpAddress == tempip &&
            //!clientManager.DicClientID.Keys.Contains(x.UserName)))
            //{
            //    var client = clientManager.allClient.Find(x => x.IpAddress == tempip);
            //    await Groups.Add(Context.ConnectionId, "All Users");
            //    await Groups.Add(Context.ConnectionId, client.PartName);//加到部门组，部门聊天
            //    clientManager.DicClientID.Add(client.UserName, Context.ConnectionId);//加到连接字典，私聊
            //}
            //else
            //{
            //    Clients.Caller.stopClient();
            //}
            int errorcode = 0;
            if (!ClientManager.db.ClientsInfo.Any(x => x.IpAddress == tempip))
            { errorcode = 1; }
            var client = errorcode == 0 ? ClientManager.db.ClientsInfo.FirstOrDefault(x => x.IpAddress == tempip) : null;
            if (errorcode == 0 && ClientManager.DicClientID.Keys.Contains(client?.UserName))
            { errorcode = 2; }
            if (errorcode != 0)
                Clients.Caller.stopClient(errorcode);
            else
            {
                client.ConnectedID = Context.ConnectionId;
                await Groups.Add(Context.ConnectionId, "All Users");
                await Groups.Add(Context.ConnectionId, client.PartName);//加到部门组，部门聊天
                ClientManager.DicClientID.Add(client.UserName, Context.ConnectionId);//加到连接字典，私聊
                ClientManager.db.SaveChanges();
                await base.OnConnected();
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var ipAddress = Context.Request.Environment["server.RemoteIpAddress"];
            IPAddress iPAddress = IPAddress.Parse(ipAddress.ToString());
            string tempip = iPAddress.MapToIPv4().ToString();
            var client = ClientManager.db.ClientsInfo.FirstOrDefault(x => x.IpAddress == tempip);

            try
            {
                Groups.Remove(Context.ConnectionId, "All Users");
            }
            catch (Exception e)
            {

                throw e;
            }

              Groups.Remove(Context.ConnectionId, client.PartName);//加到部门组，部门聊天
            if (client.ConnectedID == Context.ConnectionId)
            { ClientManager.DicClientID.Remove(client.UserName);
                client.ConnectedID = "";
                ClientManager.db.SaveChanges();
            }//加到连接字典，私聊
            string offlinereason = stopCalled ? "关闭连接" : "超时";
            Console.WriteLine("Clinet off line because " + offlinereason + "  User:" + client.UserName);
            await base.OnDisconnected(stopCalled);
        }
    }

    public static class ClientManager
    {
        public static List<string> IpList = new List<string>(0);
        public static Dictionary<string, string> DicClientID = new Dictionary<string, string>();
        public static List<ClientInfo> allClient = new List<ClientInfo>();

        public static ClientsDbContext db;

        //todo: 添加客户端ip管理，用户名管理，以及部门管理
        static ClientManager()
        {
            IpList.Add("192.168.113.4");
            IpList.Add("172.30.252.49");
            IpList.Add("192.168.113.8");
            allClient.Add(new ClientInfo { IpAddress = "192.168.113.4", UserName = "刘涛", PartName = "研发" });
            allClient.Add(new ClientInfo { IpAddress = "172.30.252.49", UserName = "刘涛", PartName = "研发" });
            allClient.Add(new ClientInfo { IpAddress = "192.168.113.3", UserName = "刘涛台机", PartName = "研发" });
            allClient.Add(new ClientInfo { IpAddress = "13.104.154.234", UserName = "刘涛笔记本", PartName = "研发" });


            db = new ClientsDbContext();
            var ss = db.ClientsInfo.FirstOrDefault(x => x.UserName == "刘涛台机");
            try
            {
                ss.IpAddress = "192.168.113.8";
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
            //foreach(var cl in  db.ClientsInfo)
            //{
            //    cl.CreatedTime = DateTime.Now;
            //}
            //db.SaveChanges();
            
            //using (ClientsDbContext db = new ClientsDbContext())
            {
                /////测试增加数据
                //db.ClientsInfo.Add(new ClientInfo { IpAddress = "192.168.113.4", UserName = "刘涛", PartName = "研发" });
                //db.ClientsInfo.Add(new ClientInfo { IpAddress = "172.30.252.49", UserName = "刘涛", PartName = "研发" });
                //db.ClientsInfo.Add(new ClientInfo { IpAddress = "192.168.113.3", UserName = "刘涛台机", PartName = "研发" });
                //db.ClientsInfo.Add(new ClientInfo { IpAddress = "13.104.154.234", UserName = "刘涛笔记本", PartName = "研发" });
                //db.SaveChanges();

            }
        }




    }

    /// <summary>
    /// ip地址，用户名，部门名，认证信息
    /// </summary>
    public class ClientInfo
    {
        private string ipAddress;

        private string userName;

        private string partName;

        private string connectedID;

        private int id;
        public string IpAddress { get => ipAddress; set => ipAddress = value; }
        public string UserName { get => userName; set => userName = value; }
        public string PartName { get => partName; set => partName = value; }
        public DateTime CreatedTime { get; set; }
        public string ConnectedID { get => connectedID; set => connectedID = value; }
        public int Id { get => id; set => id = value; }
    }

    public enum ClientGroup
    {
        UnCert=0,

        Cert=1

    }
    //public class CustomUserIdProvider:IUserIdProvider
    //{
    //    public virtual string GetUserId(HubConnectionContext connection)
    //    {
    //        return connection.Users?.FindFirst(ClaimTypes.Email)?.Value;
    //    }
    //}


}
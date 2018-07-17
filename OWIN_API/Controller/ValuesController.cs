using FANUC;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace OWIN_API
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Index()
        {
            ErrorMsg errorMsg = new ErrorMsg { Msg = "value1", DeviceCode = "value2" };
            return base.Json<ErrorMsg>(new ErrorMsg { Msg = "value1", DeviceCode = "value2" });
        }

        public IHttpActionResult Get()
        {
            lock (ValuesController.errorMsg)
            {
                if (ValuesController.errorMsg.num > 0)
                {
                    ValuesController.errorMsg.num -= 1;
                    Thread.Sleep(200);
                    return Ok("get success!" + ValuesController.errorMsg.num.ToString());
                }
                else
                {
                    return Ok("num is null");
                }
            }
            ErrorMsg errorMsg = new ErrorMsg { Msg="value1", DeviceCode="value2" };
            return base.Json<ErrorMsg>(new ErrorMsg { Msg = "value1", DeviceCode = "value2" });
        }

        public static int x=1;
        public static ErrorMsg errorMsg = new ErrorMsg() { num = 0 };
        // GET api/values/5 
        public async Task<object> Get(int id)
        {
            string ip;
            lock (errorMsg)
            {
                ip = "192.168.1." + errorMsg.num.ToString();
                errorMsg.num += 1;
            }
            string port = "8193";
            string timeout = "5";
            var result =  ConcectAsync1(ip, port, timeout);

            return result.Result;

            //return new string[] { ret.ToString() };
        }

        public async Task<object> ConcectAsync(string ip, string port, string timeout)
        {
            //await Task.Delay(2);
            var result = await ConcectAsync1(ip, port, timeout).ConfigureAwait(false);
            return result;
        }
        public async Task<object> ConcectAsync1(string ip, string port, string timeout)
        {
            //await Task.Delay(20);
            int ret = Fanuc.cnc_allclibhndl3(ip, Convert.ToUInt16(port), Convert.ToInt32(timeout), out Fanuc.h);
            if (ret != Fanuc.EW_OK)
            {
                ErrorMsg a = new ErrorMsg { Msg = "连接设备失败", DeviceCode = ret.ToString() };
                //MyHub.GlobalHubClients.All.addMessage("控制中心", "连接设备失败，请检查设备网络状态");
                var context= GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                context.Clients.All.addMessage("控制中心", "连接设备失败，请检查设备网络状态");
                return new  { Msg = "连接设备失败", DeviceCode = ret.ToString(), IP = ip}; 
            }
            else
            {
                return new { Time = "123" };
            }
        }
        [HttpGet]
        public async  Task<object> AddDevice([FromUri]string ip, [FromUri]string port, [FromUri]string deviceName, [FromUri]string remark)
        //public object AddDevice(string ip)
        {
            var res = await ConcectAsync1(ip, port, "5");
            //return new { Msg = "test", Code = "OK", Data = "123" };
            return res;
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }

    public class ErrorMsg
    {
        public string Msg;
        public string DeviceCode;
        public int num;

    }
}
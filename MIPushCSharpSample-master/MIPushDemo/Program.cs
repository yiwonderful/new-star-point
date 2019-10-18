using com.xiaomi.xmpush.server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MIPushCSharpSample
{
    class Program
    {
        private const string baseUri = "http://192.168.100.102:47171/";
        private string BaseUri;
        public Program(string baseUri)
        {
            this.BaseUri = baseUri;
        }
        static void Main(string[] args)
        {
            #region 小米推送
            //MIpush();
            #endregion
            #region webApi
            //post();
            #endregion

            #region 传统形式
            Program pro = new Program(baseUri);
            string uri = "api/Configuration/GetDictSelect?Dictid=1463";
            pro.Get(uri);
            #endregion
            Console.ReadLine();
        }

        static void MIpush()
        {
            try
            {

                Constants.useOfficial();//正式环境
                //Constants.useSandbox();//测试环境，只针对IOS
                string messagePayload = "这是一个消息";
                string title = "通知标题";
                string description = "通知说明" + DateTime.Now;

                #region 安卓发送

                //Sender androidSender = new Sender("YNaLUDPuBZSmNgrtaptqBw==");//你的AppSecret
                Sender androidSender = new Sender("LobylJQk0G6fNfZecXhyog==");//你的AppSecret

                com.xiaomi.xmpush.server.Message androidMsg = new com.xiaomi.xmpush.server.Message.Builder()
                    .title(title)
                    .description(description)//通知栏展示的通知描述
                    .payload(messagePayload)//透传消息
                    .passThrough(0)//设置是否透传1:透传, 0通知栏消息
                    .notifyId(new java.lang.Integer(Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds)))//取时间戳，避免通知覆盖
                    .restrictedPackageName("com.ican.smartspace")//包名
                    .notifyType(new java.lang.Integer(1)) //使用默认提示音提示
                    .notifyType(new java.lang.Integer(2)) //使用默认震动
                    .notifyType(new java.lang.Integer(3)) //使用默认LED灯光
                    .timeToLive(3600000 * 336)//服务器默认保留两周（毫秒）
                    .extra("data", "测试extra11111")//字符数不能超过1024最多十组
                    .build();
                //广播
                com.xiaomi.xmpush.server.Result androidPushResult = androidSender.broadcastAll(androidMsg, 3);

                //针对每个用户注册的registerid
                string regId = "";
                string accout = string.Empty;
                com.xiaomi.xmpush.server.Result androidPushResult1 = androidSender.send(androidMsg, regId, 3);
                com.xiaomi.xmpush.server.Result androidPushResult3 = androidSender.sendToUserAccount(androidMsg, accout, 3);
                com.xiaomi.xmpush.server.Result androidPushResult2 = androidSender.sendToUserAccount(androidMsg, regId, 3);
                #endregion
                //result.rows = androidPushResult;
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        #region WEB API 测试
        static void post()
        {
            var client = new HttpClient();
            //基本的API URL
            client.BaseAddress = new Uri("http://192.168.100.102:47171/");
            //默认希望响应使用Json序列化(内容协商机制，我接受json格式的数据)
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //运行client接收程序
            Run(client);
            Console.ReadLine();
        }

        static async void Run(HttpClient client)
        {
            //post 请求插入数据
            var result = await AddPerson(client);
            Console.WriteLine($"添加结果：{result}"); //添加结果：true

            //get 获取数据
            var person = await GetPerson(client);
            //查询结果：Id=1 Name=test Age=10 Sex=F
            Console.WriteLine($"查询结果：{person}");

            //put 更新数据
            result = await PutPerson(client);
            //更新结果：true
            Console.WriteLine($"更新结果：{result}");

            //delete 删除数据
            result = await DeletePerson(client);
            //删除结果：true
            Console.WriteLine($"删除结果：{result}");
        }

        
        //post
        static async Task<bool> AddPerson(HttpClient client)
        {
            //向Person发送POST请求，Body使用Json进行序列化
            return await client.PostAsJsonAsync("api/Person", new Person() { Age = 10, Id = 1, Name = "test", Sex = "F" })
                                //返回请求是否执行成功，即HTTP Code是否为2XX
                                .ContinueWith(x => x.Result.IsSuccessStatusCode);
        }

        //get
        static async Task<ResponData> GetPerson(HttpClient client)
        {
            //向Person发送GET请求
            return await await client.GetAsync("api/Configuration/GetDictSelect?Dictid=1463")
                                     //获取返回Body，并根据返回的Content-Type自动匹配格式化器反序列化Body内容为对象
                                     .ContinueWith(x => x.Result.Content.ReadAsAsync<ResponData>(
                    new List<MediaTypeFormatter>() {new JsonMediaTypeFormatter()/*这是Json的格式化器*/
                                                   ,new XmlMediaTypeFormatter()/*这是XML的格式化器*/}));
        }

        //put
        static async Task<bool> PutPerson(HttpClient client)
        {
            //向Person发送PUT请求，Body使用Json进行序列化
            return await client.PutAsJsonAsync("api/Person/1", new Person() { Age = 10, Id = 1, Name = "test1Change", Sex = "F" })
                                .ContinueWith(x => x.Result.IsSuccessStatusCode);  //返回请求是否执行成功，即HTTP Code是否为2XX
        }
        //delete
        static async Task<bool> DeletePerson(HttpClient client)
        {
            return await client.DeleteAsync("api/Person/1") //向Person发送DELETE请求
                               .ContinueWith(x => x.Result.IsSuccessStatusCode); //返回请求是否执行成功，即HTTP Code是否为2XX
        }

        #endregion

        #region 传统web形式的请求
        #region Get请求
        public string Get(string uri)
        {
            //先根据用户请求的uri构造请求地址
            string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);
            //创建Web访问对  象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();
            Console.WriteLine(returnXml);
            return returnXml;
        }
        #endregion

        #region Post请求
        public string Post(string data, string uri)
        {
            //先根据用户请求的uri构造请求地址
            string serviceUrl = string.Format("{0}/{1}", this.BaseUri, uri);
            //创建Web访问对象
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //把用户传过来的数据转成“UTF-8”的字节流
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

            myRequest.Method = "POST";
            myRequest.ContentLength = buf.Length;
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            //发送请求
            Stream stream = myRequest.GetRequestStream();
            stream.Write(buf, 0, buf.Length);
            stream.Close();

            //获取接口返回值
            //通过Web访问对象获取响应内容
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
            string returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
            reader.Close();
            myResponse.Close();
            Console.WriteLine(returnXml);
            return returnXml;

        }
        #endregion
        #endregion
    }
}

using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using PictureManager.Views;

namespace PictureManager.SocialNetworkAccess
{
    public class VkWallPost
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string user_id { get; set; }
        public static string Token { get; set; }
        public static int Author_id { get; set; }
        public static bool flag = false;
        public static string Code;


        /// <summary>
        /// Отримання дозволу на доступ до стіни і фото
        /// </summary>
        public  void GetCode()
        {
            string reqStrTemplate =
                "http://api.vkontakte.ru/oauth/authorize?client_id={0}&scope=offline,wall,photos";

            System.Diagnostics.Process.Start(
                String.Format(reqStrTemplate, Publics.Vk_AppID));

        }

        /// <summary>
        /// Отримання acces_token і id користоувача
        /// </summary>
        /// <param name="Code">код, який користувач повинен скопіювати з посилання в браузері</param>
        /// <returns></returns>
        public  string GetToken(string Code)
        {

            string reqStrTemplate =
              "https://api.vkontakte.ru/oauth/access_token?client_id={0}&client_secret={1}&code={2}";

            string reqStr = String.Format(reqStrTemplate, Publics.Vk_AppID, Publics.Vk_Secret, Code);

            WebClient webClient = new WebClient();
            string response = webClient.DownloadString(reqStr);

            JavaScriptSerializer s = new JavaScriptSerializer();
            VkWallPost jsonResponse = s.Deserialize<VkWallPost>(response);
            Token = jsonResponse.access_token;
            Author_id = Convert.ToInt32(jsonResponse.user_id);
            return Token;
        }


        private static string Response(string request_path)
        {
            string response = String.Empty;
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(request_path);
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            Stream receiveStream = Response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);

            while (count > 0)
            {
                String line = new String(read, 0, count);
                response += line + "\r\n";
                count = readStream.Read(read, 0, 256);
            }

            Response.Close();
            readStream.Close();

            return response;
        }

        private static string photosGetWallUploadServer(int group_id)
        {
            string request_path = "https://api.vk.com/method/photos.getWallUploadServer?";
            request_path += "group_id" + group_id;
            request_path += "&v=5.34";
            request_path += "&access_token=" + Token;

            var json = JObject.Parse(Response(request_path));
            return json["response"]["upload_url"].ToString();
        }

        private static JObject photosUploadPhotoToURL(string URL, string file_path)
        {
            WebClient myWebClient = new WebClient();
            byte[] responseArray = myWebClient.UploadFile(URL, file_path);
            var json = JObject.Parse(Encoding.ASCII.GetString(responseArray));

            return json;
        }


        private  static JObject photosSaveWallPhoto(string server, string photo, string hash)
        {
            string request_path = "https://api.vk.com/method/photos.saveWallPhoto?";
            request_path += "server=" + server;
            request_path += "&photo=" + photo;
            request_path += "&hash=" + hash;
            request_path += "&v=5.21";
            request_path += "&access_token=" + Token;

            var json = JObject.Parse((Response(request_path).Replace("[", String.Empty)).Replace("]", String.Empty));
            return json;
        }


        private static string wallPost(int owner_id, int friends_only = 0, int from_group = 1, string message = "", string attachments = "")
        {
            if (message == "" && attachments == "") return "Error: message and attachments is empty!";

            string request_path = "https://api.vk.com/method/wall.post?";
            request_path += "owner_id=" + owner_id;
            request_path += "&friends_only=" + friends_only;
            request_path += "&from_group=" + from_group;
            if (message != String.Empty) request_path += "&message=" + message;
            if (attachments != String.Empty) request_path += "&attachments=" + attachments;
            request_path += "&v=5.21";
            request_path += "&access_token=" + Token;

            return Response(request_path);
        }

        /// <summary>
        /// Для створення запису на стіні
        /// </summary>
        /// <param name="message">Повідомлення, яке буде на стіні разом з фото.</param>
        /// <param name="attachment">Шлях до фото</param>
        /// <returns></returns>
        public  string AddWallPost(string message = "", string attachment = "")
        {
            int gid = Author_id;
            if (message == "" && attachment == "") return "Error";


            if (attachment != "")
            {
                string img_path = photosGetWallUploadServer(gid);
                var resp = photosUploadPhotoToURL(img_path, attachment);
                resp = photosSaveWallPhoto(resp["server"].ToString(), resp["photo"].ToString(), resp["hash"].ToString());
                attachment = "photo" + resp["response"]["owner_id"] + "_" + resp["response"]["id"];
            }

            return wallPost(gid, message: message, attachments: attachment);
        }
    }


    class Publics
    {
        public static string Vk_AppID = "4957703";
        public static string Vk_Secret = "PXKCdPOxKduPkijw5GJI";
    }
}

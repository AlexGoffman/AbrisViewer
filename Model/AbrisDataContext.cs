using System;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace WpfApp1.Model
{
    public class AbrisDataContext
    {
        private string PHPSESSID = string.Empty;
        /// <summary>
        ///  Метод GET запроса на сайт для получения сессии
        /// </summary>
        /// <returns>Сессия</returns>
        public string GetSessionId()
        {
            ServicePointManager.Expect100Continue = false;
            string SessionId = string.Empty;
            HttpWebRequest GetSessionIdRequest = (HttpWebRequest)WebRequest.Create("http://demo.abris.site/?demo562");
            HttpWebResponse GetSessionIdresponse = (HttpWebResponse)GetSessionIdRequest.GetResponse();
            using (Stream stream = GetSessionIdresponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    SessionId = reader.ReadToEnd();
                }
            }
            SessionId = GetSessionIdresponse.Headers.Get("Set-Cookie");
            PHPSESSID = SessionId;
            GetSessionIdresponse.Close();
          
            return SessionId;
        }

        /// <summary>
        ///  Метод авторизации на сайте с нужной сессией
        /// </summary>
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        public bool RequestWithLogin(string Login, string Password)
        {
            string result = string.Empty;
            bool IsLogInSuccessed = false;

            HttpWebRequest RequestWithLogin = (HttpWebRequest)WebRequest.Create("http://demo.abris.site/Server/request.php");
            RequestWithLogin.Headers.Add(HttpRequestHeader.Cookie, GetSessionId());
            RequestWithLogin.Method = "POST"; // для отправки используется метод Post
            //Thirdrequest.Headers.Add(HttpRequestHeader.Cookie, cookie.ToString());
            string postParameters = "method=authenticate&params=%5B%7B%22usename%22%3A%22" + Login +
                "%22%2C%22passwd%22%3A%22" + Password + "%22%7D%5D";
            // преобразуем данные в массив байтов
            byte[] byteArray = Encoding.UTF8.GetBytes(postParameters);
            // устанавливаем тип содержимого - параметр ContentType
            RequestWithLogin.ContentType = "application/x-www-form-urlencoded";
            RequestWithLogin.Referer = "http://demo.abris.site/?demo562";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            RequestWithLogin.ContentLength = byteArray.Length;
            //Thirdrequest.KeepAlive = true;

            //записываем данные в поток запроса
            using (Stream datastream = RequestWithLogin.GetRequestStream())
            {
                datastream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse LoginResponse = (HttpWebResponse)RequestWithLogin.GetResponse();
            using (Stream stream = LoginResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            if (result.Contains("\"usename\":\"admin\""))
            {
                IsLogInSuccessed = true;
            }
            return IsLogInSuccessed;
        }
        
        /// <summary>
        ///  Метод запроса метаданных по базе с сервера
        /// </summary>
        /// <returns></returns>
        public string RequestForMetaData(string TableName)
        {
            string result = string.Empty;

            HttpWebRequest RequestForMetaData = (HttpWebRequest)WebRequest.Create("http://demo.abris.site/Server/request.php");
            RequestForMetaData.Headers.Add(HttpRequestHeader.Cookie, PHPSESSID);
            RequestForMetaData.Method = "POST"; // для отправки используется метод Post
            //Thirdrequest.Headers.Add(HttpRequestHeader.Cookie, cookie.ToString());
            string postmetadetaparams = "method=getAllModelMetadata&params=%5B%7B%7D%5D";
            // преобразуем данные в массив байтов
            byte[] bytemeta = Encoding.UTF8.GetBytes(postmetadetaparams);
            // устанавливаем тип содержимого - параметр ContentType
            RequestForMetaData.ContentType = "application/x-www-form-urlencoded";
            RequestForMetaData.Referer = "http://demo.abris.site/?demo562";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            RequestForMetaData.ContentLength = bytemeta.Length;
            //Thirdrequest.KeepAlive = true;

            //записываем данные в поток запроса
            using (Stream datastream = RequestForMetaData.GetRequestStream())
            {
                datastream.Write(bytemeta, 0, bytemeta.Length);
            }

            HttpWebResponse newresponse = (HttpWebResponse)RequestForMetaData.GetResponse();
            using (Stream stream = newresponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            newresponse.Close();

            //int k = result.Length;
            //k = result.IndexOf("\"" + TableName + "\":{\"projection_name\":");

            //result = result.Remove(0,k);
            //k = result.Length;
            //k = result.IndexOf('}');
            //result = result.Remove(k);

            JObject stuff = JObject.Parse(result);
            //JObject array = (JObject)stuff.result.projections.song.properties;
            //var b = array["title"].Value<string>();
            //var a = array.PropertyValues().Children<JToken>().Values();
            List<string> Properties = new List<string>();
            result = string.Empty;
            string row;
            string p;
            foreach (JToken tkn in stuff["result"]["projections"][TableName]["properties"])
            {
                foreach (IJEnumerable<JToken> ter in tkn.Values())
                {
                    row = ter.ToString().Replace("\"","");
                    if (row.Contains("title"))
                    {
                        p = row.Replace(" ", "").Replace("title", "").Replace(":", "");
                        Properties.Add(p + "\n");
                        result += p + "\n";
                    }
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace work2
{
    public class Program
    {
        static CookieContainer cookie = new CookieContainer();
        static HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookie };
        static HttpClient client = new HttpClient(handler);

        static void Main(string[] args)
        {
            bool relogin = true;
            while (true)
            {
                if (relogin)
                {
                    Login();
                    GetData();
                }
            }
            handler.Dispose();
            client.Dispose();
        }

        static void Login()
        {
            string url = "https://www.howkteam.vn/account/login?ReturnUrl=%2F";
            string page_login = "";
            string login_token = "";


            HttpResponseMessage response = client.GetAsync(url).Result;
            HttpContent content = response.Content;
            page_login = content.ReadAsStringAsync().Result;


            var res = Regex.Matches(page_login, @"(?<=__RequestVerificationToken"" type=""hidden"" value="").*?(?="")", RegexOptions.Singleline);
            if (res.Count > 0)
            {
                login_token = res[0].ToString();
            }

            var form = new Dictionary<string, string>
                    {
                        {"Email", "tranvanphong26598"},
                        {"Password", "Tvp26598@"},
                        {"__RequestVerificationToken", login_token},
                        {"RememberMe", "false" },
                    };
            var formContent = new FormUrlEncodedContent(form); // thiết lập formContent
            var tokenResponse = client.PostAsync(url, formContent).Result; // gửi post đến server                  
        }

        static void GetData()
        {
            var thoi_gian_bat_dau = DateTime.Now;
            var resp = client.GetAsync("https://www.howkteam.vn/").Result;
            var html = resp.Content.ReadAsStringAsync().Result;
            Console.WriteLine(html);

            if (DateTime.Now.Subtract(thoi_gian_bat_dau).TotalSeconds > 60 * 10)
            {

                //relogin = true;
                thoi_gian_bat_dau = DateTime.Now;
            }
            Thread.Sleep(120);
        }
   }
}

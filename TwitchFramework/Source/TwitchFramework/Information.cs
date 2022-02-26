using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TwitchFramework
{
    static class Information
    {
        public static async Task<bool> IsAuthenticated()
        {
            if (TwitchFramework.settings.TwitchAccessToken == "") return false;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://id.twitch.tv/oauth2/validate");
            request.Headers.Add("Authorization", $"Bearer {TwitchFramework.settings.TwitchAccessToken}");



            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
#if DEBUG
                TwitchFramework.Log($"Authentication Response: {response.StatusCode}");
#endif

                return response.StatusCode == HttpStatusCode.OK;
            }
        }
    }
}

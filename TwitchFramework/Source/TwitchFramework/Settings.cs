using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TwitchFramework
{
    public class Settings : ModSettings
    {
        const string TWITCH_AUTH_URI = "https://id.twitch.tv/oauth2/authorize?client_id=ebzbb5smiifjyfwdtrtmw844oepedo&redirect_uri=http://localhost:5000/&response_type=token&scope=viewing_activity_read";

        private string _twitchAccessToken = "";
        
        public string TwitchAccessToken
        {
            get
            {
                return _twitchAccessToken;
            }

            set
            {
                _twitchAccessToken = value;
                this.Write();
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref _twitchAccessToken, "AccessToken", "");
            base.ExposeData();
        }

        public static void AuthInTwitch()
        {
            LocalServer.Start();
            System.Diagnostics.Process.Start(TWITCH_AUTH_URI);
        }

    }
}

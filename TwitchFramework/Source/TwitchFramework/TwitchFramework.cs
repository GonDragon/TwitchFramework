using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Verse;
using System.IO;

namespace TwitchFramework
{
    public class TwitchFramework : Mod
    {
        public static string Author => "GonDragon";
        public static string Name => Assembly.GetName().Name;
        public static string Id => Author + "." + Name;

        public static string Version => Assembly.GetName().Version.ToString();

        private static Assembly Assembly
        {
            get
            {
                return Assembly.GetAssembly(typeof(TwitchFramework));
            }
        }

        public static readonly Harmony Harmony = new Harmony(Id);

        public static Settings settings;

        public TwitchFramework(ModContentPack content) : base(content)
        {
            settings = GetSettings<Settings>();
            Harmony.PatchAll();

            Information.IsAuthenticated().ContinueWith((task) => {
                if(!task.Result)
                {
                    settings.TwitchAccessToken = "";
                }
            });

        }

        public static string ReadTextResourceFromAssembly(string filename)
        {
            string resourceName = Assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));

            using (Stream stream = Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            if(TwitchFramework.settings.TwitchAccessToken.Length == 0)
            {
                if (listingStandard.ButtonTextLabeled("Twitch: ", "Log in"))
                {
                    Settings.AuthInTwitch();
                }
            }

#if DEBUG
            this.Debug_functions(listingStandard);
#endif

            listingStandard.End();
        }

#if DEBUG
        public void Debug_functions(Listing_Standard listingStandard)
        {
            listingStandard.LabelDouble("Current Code:", TwitchFramework.settings.TwitchAccessToken);

            if (listingStandard.ButtonText("Is Authenticated"))
            {
                Information.IsAuthenticated().ContinueWith((task) => {
                    if (task.Result) Log("Authenticated");
                    else Log("Not Authenticated");
                });
            }
        }
#endif

        public override string SettingsCategory()
        {
            return "Twitch Framework";
        }
        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));

        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

        public static void ErrorOnce(string message, string key) => Verse.Log.ErrorOnce(PrefixMessage(message), key.GetHashCode());

        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}

using Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
    public static class LoadService
    {
        public static TrelloConfig Config { get; set; }

        public static void Setup()
        {


            if (File.Exists(Constants.Errorfile))
                File.Delete(Constants.Errorfile);
            if (File.Exists(Constants.ConfigFile))
            {
                Config = JsonConvert.DeserializeObject<TrelloConfig>(File.ReadAllText(Constants.ConfigFile));
            }
            else
            {
                Config = new TrelloConfig
                {
                    Key = "",
                    Token = ""


                };
            }

            if (File.Exists(Constants.Logfile)) File.Delete(Constants.Logfile);


        }

        public static void SaveConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Config));
        }

    }
}

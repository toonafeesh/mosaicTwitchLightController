using System;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace lightApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var previousTimeline = 0;
            var timeline = 12;
            var ipAddress = "192.168.1.200";
            var duration = 0;
            var mode = "mp";
            var command = "!redAlert";

            if (args.Any()){
                for(var i = 0; i < args.Length; i++){
                    if(args[i] == "-t")
                        timeline = int.Parse(args[i + 1]);
                    if(args[i] == "-i")
                        ipAddress = args[i + 1];
                    if(args[i] == "-d" && int.Parse(args[i + 1]) > 0)
                        duration = int.Parse(args[i + 1]);
                    if(args[i] == "-m")
                        mode = args[i + 1];
                    if(args[i] == "-c")
                        command = args[i + 1];
                }
            }

            if(mode == "mp"){
                IrcClient ircClient = new IrcClient();
                ircClient.SendMessage(command, "toonafeesh");
                ircClient.SendMessage(command, "shipwreckxshaun");
                ircClient.CloseIrcConntection();
            } 
            else 
            {
                previousTimeline = getCurrentTimeline(ipAddress);

                timelineSwitch(ipAddress, timeline, "release");
                timelineSwitch(ipAddress, timeline, "start");

                if(duration > 0)
                {
                    Thread.Sleep(duration * 1000);
                    timelineSwitch(ipAddress, previousTimeline, "release");
                    timelineSwitch(ipAddress, previousTimeline, "start");
                }
            }
        }

        static int getCurrentTimeline(string ipAddress){
            string webAddr="http://" + ipAddress + "/api/timeline";
            
            var httpWebRequest = WebRequest.CreateHttp(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";  

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Console.WriteLine(responseText);

                JObject o = JObject.Parse(responseText);
                // var valueArray = (JArray) o["value"];

                foreach(JObject x in o["timelines"]){
                    //Console.WriteLine(x);
                    if(x["onstage"].ToString() == "True"){
                        return x["num"].ToObject<int>();
                    }
                }
            }

            return 0;
        }

        static void timelineSwitch(string ipAddress, int timeline, string action){
            try{
                string webAddr="http://" + ipAddress + "/api/timeline";

                var httpWebRequest = WebRequest.CreateHttp(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";    

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "";
                    if (action == "release")
                        json = "{ \"action\" : \"release\" }";
                    else if (action == "start")
                        json = "{ \"action\" : \"start\", \"num\" : " + timeline + " }";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            }catch(WebException ex){
                Console.WriteLine(ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace Janky.Service
{
    public class JenkinsStatusService
    {
        private string _jenkinsBaseUrl;


        public JenkinsStatusService(string baseUrl)
        {
            _jenkinsBaseUrl = baseUrl;
        }


        public ServerStatus GetServerStats()
        {
            try
            {
                using (var w = new WebClient())
                {
                    var json_data = string.Empty;

                    json_data = w.DownloadString(CreateCommand(_jenkinsBaseUrl, Commands.ServerStatus));

                    return JsonConvert.DeserializeObject<ServerStatus>(json_data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public ShortJobStatus GetJobStatus(string jobName)
        {
            string url = string.Empty;
            try
            {
                url = _jenkinsBaseUrl + "/job/" + jobName;
                url = Uri.EscapeUriString(url);

                using (var w = new WebClient())
                {
                    var json_data = string.Empty;

                    string command = CreateCommand(url, Commands.JobStatus);
                    json_data = w.DownloadString(command);

                    return JsonConvert.DeserializeObject<ShortJobStatus>(json_data);
                }
            }
            catch (Exception ex)
            {
                return new ShortJobStatus()
                    {
                        Building = false,
                        Job =  new Job() { Color = "red", Name = url, Url = url},
                        Result = "error"
                    };
            }
        }


        private string CreateCommand(string baseUrl,  string command)
        {
            return baseUrl + command;
        }


        private static class Commands
        {
            public static string ServerStatus = @"/api/json";
            public static string JobStatus = @"/lastBuild/api/json";

        }
    }
}

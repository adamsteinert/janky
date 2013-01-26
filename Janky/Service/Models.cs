using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janky.Service
{
    public class ServerStatus
    {
        public List<Job> Jobs { get; set; }
    }

    public class Job
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Color { get; set; }
    }

    public class ShortJobStatus
    {
        private static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public Job Job { get; set; }

        public bool Building { get; set; }
        public string Result { get; set; }
        public List<Culprit> Culprits { get; set; }
        public long TimeStamp { get; set; }

        public string UserFormattedBuildResult
        {
            get
            {
                if (Building)
                    return "Build in progress";
                
                if (DidSucceed)
                    return "Success";

                if (string.IsNullOrEmpty(Result))
                    return "Unknown or no status";

                return Result;
            }
        }

        public bool DidSucceed
        {
            get { return Building || Result == "SUCCESS"; }
        }

        public DateTime BuildDate
        {
            get { return Epoch.AddMilliseconds(TimeStamp); }
        }

        public string CulpritString
        {
            get {
                string result = string.Empty;
                foreach (var c in Culprits)
                {
                    result += string.Format("{0} ", c.FullName);
                }

                return String.IsNullOrEmpty(result) ? null : result;
            }
        }
    }

    public class Culprit
    {
        public string FullName { get; set; }
    }


    // hmm?  interesting though
    // http://jenkins.yaharasoftware.com:8100/job/wids/api/json/lastBuild
    public class Health
    {
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int Score { get; set; }
    }

    public class LastBuild
    {
        public string Name { get; set; }
        public bool Buildable { get; set; }
        public Health HealthReport { get; set; }
    }
}

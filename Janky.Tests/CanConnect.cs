using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Janky.Service;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Janky.Tests
{
    [TestFixture]
    public class CanConnect
    {
        private const string TestUrl = @"https://ci.jenkins-ci.org/api/json";
        private const string TestJobBaseUrl = @"https://ci.jenkins-ci.org/job/lib-jira-api/";
        private const string TestJobDataCommand = @"lastBuild/api/json";

        [Test]
        public void CanSeeServer()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                json_data = w.DownloadString(TestUrl);

                Assert.That(!string.IsNullOrEmpty(json_data));
            }
        }

        [Test]
        public void CanParseJobs()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                json_data = w.DownloadString(TestUrl);
                var result = JsonConvert.DeserializeObject<ServerStatus>(json_data);

                Assert.NotNull(result);
                Assert.IsNotEmpty(result.Jobs);
            }
        }

        [Test]
        public void TestJobStatus()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;

                string url = TestJobBaseUrl + TestJobDataCommand;
                json_data = w.DownloadString(url);

                var result = JsonConvert.DeserializeObject<ShortJobStatus>(json_data);

                Assert.NotNull(result);
                Assert.True(result.DidSucceed);
            }            
        }
    }
}

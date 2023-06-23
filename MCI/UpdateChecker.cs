using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;

namespace MCI
{
    static class UpdateChecker
    {
        public static void CheckForUpdate()
        {
            try {
                needsUpdate = TaskUpdate().GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }

        public static async Task<bool> TaskUpdate()
        {
            HttpClient http = new();
            http.DefaultRequestHeaders.Add("User-Agent", "MCI-Agent");
            var response = await http.GetAsync(new Uri("https://api.github.com/repos/MyDragonBreath/AmongUs.MultiClientInstancing/releases/latest"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null) return false;

            string json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GitHubApiObject>(json);
            string tagname = data.tag_name;
            if (tagname == null) return false;
            Version ver = Version.Parse(tagname.Replace("v", ""));
            int diff = MCIPlugin.vVersion.CompareTo(ver);
            return diff < 0;
        }

        public static bool needsUpdate;
        class GitHubApiObject
        {
            [JsonPropertyName("tag_name")]
            public string tag_name { get; set; }
            [JsonPropertyName("assets")]
            public GitHubApiAsset[] assets { get; set; }
        }

        class GitHubApiAsset
        {
            [JsonPropertyName("browser_download_url")]
            public string browser_download_url { get; set; }
        }
    }
}

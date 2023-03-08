using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MCI
{
    static class UpdateChecker
    {
        public static void checkForUpdate()
        {
            try {
                needsUpdate = taskUpdate().GetAwaiter().GetResult();
            } catch (Exception e) {}
        }

        public static async Task<bool> taskUpdate()
        {
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "MCI-Agent");
            var response = await http.GetAsync(new System.Uri("https://api.github.com/repos/MyDragonBreath/AmongUs.MultiClientInstancing/releases/latest"), HttpCompletionOption.ResponseContentRead);

            if (response.StatusCode != HttpStatusCode.OK || response.Content == null) return false;
 
            string json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GitHubApiObject>(json);
            string tagname = data.tag_name;
            if (tagname == null) return false;
            int diff = 0;
            System.Version ver = System.Version.Parse(tagname.Replace("v", ""));
            diff = MCIPlugin.vVersion.CompareTo(ver);
            if (diff < 0) return true;
            return false;
        }


        public static bool needsUpdate = false;



        
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

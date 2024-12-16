
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PROTATool
{
    internal class GitUtils
    {
        public static string GitHubToken;
        internal class CArtifact
        {
            public string url;
            public string path;
        }

        internal class CommitInfo
        {
            public List<CArtifact> artifacts = new List<CArtifact>();
        }

        static async Task<CArtifact> DownloadArtifactAsync(string pr, string sha, string url, string artifactName)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GitHubToken);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OpenBK7231T_PullRequestLister", "1.0"));

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error downloading artifact {artifactName}: {response.StatusCode}");
                return null;
            }

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), $"{pr}/{sha}");
            Directory.CreateDirectory(directoryPath);

            string filePath = Path.Combine(directoryPath, $"{artifactName}.zip");
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await response.Content.CopyToAsync(fs);
            }

            Console.WriteLine($"      Artifact downloaded to {filePath}");

            // Return CArtifact to save it later
            return new CArtifact
            {
                url = url,
                path = filePath
            };
        }

        public static async Task<CommitInfo> fetchCommitInfo(string owner, string repo,
            string sha, string pr)
        {
            CommitInfo ret = new CommitInfo();

            while (true)
            {
                var workflowRuns = await GetWorkflowRunsAsync(owner, repo, sha);
                if (workflowRuns != null)
                {
                    foreach (var run in workflowRuns)
                    {
                        if (run.Name.StartsWith("Semantic") == false)
                            continue;
                        if (run.Status == "in_progress")
                        {
                            Form1.setState("Waiting for build to finish - " + run.Name + "");
                            LogUtil.log("Waiting for build to finish - " + run.Name + "");
                            await Task.Delay(5000);
                            break;
                        }
                        Console.WriteLine($"    Workflow Run: {run.Name} (Status: {run.Status})");

                        var artifacts = await GetArtifactsAsync(owner,
                            repo, run.getSecondID());

                        foreach (var artifact in artifacts)
                        {
                            string downloadUrl = artifact.archive_download_url;
                            Console.WriteLine($"      Artifact: {artifact.name} has URL {downloadUrl}");

                            // Download artifact and add to the list
                            var downloadedArtifact = await DownloadArtifactAsync(pr, sha, downloadUrl, artifact.name);
                            if (downloadedArtifact != null)
                            {
                                ret.artifacts.Add(downloadedArtifact);
                            }
                        }
                    }
                }
                if (ret.artifacts.Count > 0)
                {
                    break;
                }
                else
                {
                    Form1.setState("Waiting for build");
                    LogUtil.log("Waiting for build");
                    await Task.Delay(5000);
                }
            }
            return ret;
        }

        static async Task<List<CheckRun>> GetWorkflowRunsAsync(string owner, string repo, string sha)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GitHubToken);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OpenBK7231T_PullRequestLister", "1.0"));

            string url = $"https://api.github.com/repos/{owner}/{repo}/commits/{sha}/check-runs";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching workflow runs: {response.StatusCode}");
                return new List<CheckRun>(); // Return an empty list on error
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            var result = JsonSerializer.Deserialize<CheckRunsResponse>(jsonString);

            return result?.CheckRuns ?? new List<CheckRun>();
        }
        public class CheckRunsResponse
        {
            [JsonPropertyName("check_runs")]
            public List<CheckRun> CheckRuns { get; set; } = new List<CheckRun>();
        }

        public class CheckRun
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("status")]
            public string Status { get; set; }
            public string html_url { get; set; }
            public string details_url { get; set; }

            public string getSecondID()
            {
                string pattern = @"runs/(\d+)";

                var match = Regex.Match(details_url, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }

                return null;
            }
        }

        static async Task<Artifact[]> GetArtifactsAsync(string owner, string repo,
            string runId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GitHubToken);
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OpenBK7231T_PullRequestLister", "1.0"));
            //12305496423
            // Correct URL: https://api.github.com/repos/openshwprojects/OpenBK7231T_App/actions/runs/12305496423/artifacts
            string url = $"https://api.github.com/repos/{owner}/{repo}/actions/runs/{runId}/artifacts";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching artifacts: {response.StatusCode}");
                return Array.Empty<Artifact>();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(jsonString);
            var result = JsonSerializer.Deserialize<ArtifactResponse>(jsonString);
            return result.artifacts;
        }

    }

    public class ArtifactResponse
    {
        public Artifact[] artifacts { get; set; }
    }

    public class Artifact
    {
        public string name { get; set; }
        public string archive_download_url { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Octokit;
using static PROTATool.GitUtils;

namespace PROTATool
{
    public partial class PROTATool : Form
    {
        Settings settings;
        public PROTATool()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thanks!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        string prevTopCommit;

        bool bBusy;
        void startRefreshIfPossible()
        {
            if (bBusy)
                return;
            bBusy = true;
            RefreshCommits();
        }
        private async void RefreshCommits()
        {
            PROTATool.setState("Entering...");
            LogUtil.log("Entering refresh commits");
            string prLink = textBoxPR.Text;

            // Extract owner, repo name, and pull request number from the link
            var regex = new Regex(@"https://github\.com/(?<owner>[^/]+)/(?<repo>[^/]+)/pull/(?<number>\d+)");
            var match = regex.Match(prLink);

            GitUtils.GitHubToken = textBoxPassword.Text;
            if (match.Success)
            {
                string owner = match.Groups["owner"].Value;
                string repo = match.Groups["repo"].Value;
                int pullRequestNumber = int.Parse(match.Groups["number"].Value);

                try
                {
                    PROTATool.setState("Getting commits...");
                    LogUtil.log($"Fetching commits for PR: {owner}/{repo}/pull/{pullRequestNumber}");

                    var client = new GitHubClient(new Octokit.ProductHeaderValue("PROTATool"));
                    var tokenAuth = new Credentials(GitUtils.GitHubToken);
                    client.Credentials = tokenAuth;

                    LogUtil.log("Authenticating and connecting to GitHub...");

                    var commits = await client.PullRequest.Commits(owner, repo, pullRequestNumber);
                    LogUtil.log($"Fetched {commits.Count} commits");

                    var sortedCommits = commits
                        .OrderByDescending(commit => commit.Commit.Author.Date)
                        .ToList();

                    listView1.Items.Clear();
                    LogUtil.log("Cleared previous commit list and updating with new data...");

                    foreach (var commit in sortedCommits)
                    {
                        string author = commit.Commit.Author.Name;
                        string message = commit.Commit.Message;
                        string date = commit.Commit.Author.Date.ToString("yyyy-MM-dd HH:mm:ss");
                        string sha = commit.Sha;

                        var listItem = new ListViewItem(date);
                        listItem.SubItems.Add(message);
                        listItem.SubItems.Add(author);
                        listItem.SubItems.Add(sha);

                        listView1.Items.Add(listItem);
                    }

                    var youngestCommit = commits
                        .OrderByDescending(commit => commit.Commit.Author.Date)
                        .FirstOrDefault();

                    LogUtil.log($"Youngest commit found: {youngestCommit?.Sha}");

                    if (checkBoxFlashOnChange.Checked)
                    {
                        string youngestSha = youngestCommit.Sha;
                        if (prevTopCommit != youngestSha)
                        {
                            PROTATool.setState("Fetching artifacts...");
                            LogUtil.log("Commit has changed, fetching artifacts...");
                            CommitInfo c = await GitUtils.fetchCommitInfo(owner, repo,
                                youngestSha, pullRequestNumber.ToString());
                            await sendToDevices(c);
                            prevTopCommit = youngestSha;
                            LogUtil.log($"Artifacts sent for commit: {youngestSha}");
                        }
                        else
                        {
                            LogUtil.log("No new commit detected, skipping artifact fetch.");
                        }
                    }

                    LogUtil.log("Commit refresh completed successfully.");

                }
                catch (Exception ex)
                {
                    LogUtil.log($"Error fetching commits: {ex.Message}");
                    MessageBox.Show($"Error fetching commits: {ex.Message}", "Error");
                }
            }
            else
            {
                LogUtil.log("Invalid PR link format detected.");
                MessageBox.Show("Invalid PR link format. Please enter a valid link, e.g., https://github.com/owner/repo/pull/1234.", "Invalid Input");
            }

            PROTATool.setState("Done...");
            bBusy = false;
            LogUtil.log("Exiting refresh commits.");
        }
        private async Task sendToDevices(CommitInfo c)
        {
            string[] ips = textBoxIPs.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
            LogUtil.log("Found " + ips.Length + " devices...");
            for (int i = 0; i < ips.Length; i++)
            {
                string ip = ips[i];
                try
                {
                    await sendToDevice(c, ip);
                    LogUtil.log("Sending to " + ip + " success!");
                }
                catch(Exception ex)
                {
                    LogUtil.log("Sending to " + ip + " failed with " + ex.ToString()+"!");
                }
            }

        }
        private async Task sendToDevice(CommitInfo c, string ip)
        {
            PROTATool.setState("Sending OTA to " + ip + "...");
            LogUtil.log("Starting OTA  " + ip + "process...");
            string prefix = "OpenBK7231T_";
            string ext = "rbl";

            OBKDevice d = new OBKDevice();
            d.setAddress(ip);
            OBKDevice.Root obk = await d.GetStatusAsync();

            string hardware = "";
            hardware = obk.StatusFWR.Hardware;


            var p = Platforms.findForChip(hardware);
            if(p == null)
            {
                MessageBox.Show("Unknown platform " + hardware);
                return;
            }
            ext = p.extension;
            prefix = p.name;
            LogUtil.log($"Chosen prefix: {prefix}");
            LogUtil.log($"Chosen ext: {ext}");
            string path = c.artifacts[0].path;

            // open zip file
            LogUtil.log($"Opening zip file: {path}");
            using (ZipArchive zip = ZipFile.OpenRead(path))
            {
                // find matching file
                var fileEntry = zip.Entries.FirstOrDefault(e => e.FullName.StartsWith(prefix) && e.FullName.EndsWith(ext));

                if (fileEntry != null)
                {
                    LogUtil.log($"Found matching file: {fileEntry.FullName}");

                    // read it to byte[]
                    byte[] fileData;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fileEntry.Open().CopyTo(ms);
                        fileData = ms.ToArray();
                    }

                    // send data via POST to otaURL
                    string otaURL = $"http://{ip}/api/ota";
                    LogUtil.log($"Sending OTA data to {otaURL}");
                    using (HttpClient client = new HttpClient())
                    {
                        var content = new ByteArrayContent(fileData);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        try
                        {
                            HttpResponseMessage response = await client.PostAsync(otaURL, content);
                            if (response.IsSuccessStatusCode)
                            {
                                LogUtil.log("OTA update sent successfully.");
                            }
                            else
                            {
                                LogUtil.log($"OTA update failed with status: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtil.log($"Error sending OTA update: {ex.Message}");
                        }

                        // send empty post to rebootURL
                        string rebootURL = $"http://{ip}/api/reboot";
                        LogUtil.log($"Sending reboot request to {rebootURL}");
                        try
                        {
                            HttpResponseMessage rebootResponse = await client.PostAsync(rebootURL, null);

                            if (rebootResponse.IsSuccessStatusCode)
                            {
                                LogUtil.log("Device rebooted successfully.");
                            }
                            else
                            {
                                LogUtil.log($"Reboot failed with status: {rebootResponse.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtil.log($"Error sending reboot request: {ex.Message}");
                        }
                    }
                }
                else
                {
                    LogUtil.log("No matching file found in the zip archive.");
                    MessageBox.Show("No matching file found in the zip archive.");
                }
            }
            LogUtil.log("OTA process completed.");
        }

        public static void setState(string s)
        {
            if (Singleton.InvokeRequired)
            {             
                Singleton.Invoke(new Action<string>(setState), s);
            }
            else
            {
                Singleton.labelState.Text = "State: " + s;
            }
        }
        static PROTATool Singleton;
        private void PROTATool_Load(object sender, EventArgs e)
        {
            settings = Settings.Load();
            settings.BindTextBox(textBoxPassword, "ApiKey");
            settings.BindTextBox(textBoxIPs, "Ips");
            settings.BindTextBox(textBoxPR, "Url");

            Singleton = this;
            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Date", 150);
                listView1.Columns.Add("Message", 300);
                listView1.Columns.Add("Author", 100);
                listView1.Columns.Add("SHA", 100);
                listView1.View = View.Details;
                listView1.FullRowSelect = true;
                listView1.GridLines = true;
            }
        }

        private void checkBoxFlashOnChange_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxFlashOnChange.Enabled)
            {
                timer1.Enabled = true;
                startRefreshIfPossible();
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            startRefreshIfPossible();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LogUtil.OpenLog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

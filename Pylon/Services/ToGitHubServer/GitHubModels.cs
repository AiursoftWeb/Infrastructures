using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Services.ToGitHubServer
{
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }

    public class GitHubPlan
    {
        public string name { get; set; }
        public int space { get; set; }
        public int collaborators { get; set; }
        public int private_repos { get; set; }
    }

    public class GitHubUserDetail
    {
        public string login { get; set; }
        public int id { get; set; }
        public string node_id { get; set; }
        public string avatar_url { get; set; }
        public string gravatar_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string followers_url { get; set; }
        public string following_url { get; set; }
        public string gists_url { get; set; }
        public string starred_url { get; set; }
        public string subscriptions_url { get; set; }
        public string organizations_url { get; set; }
        public string repos_url { get; set; }
        public string events_url { get; set; }
        public string received_events_url { get; set; }
        public string type { get; set; }
        public string site_admin { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string blog { get; set; }
        public string location { get; set; }
        public string email { get; set; }
        public string hireable { get; set; }
        public string bio { get; set; }
        public int public_repos { get; set; }
        public int public_gists { get; set; }
        public int followers { get; set; }
        public int following { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int private_gists { get; set; }
        public int total_private_repos { get; set; }
        public int owned_private_repos { get; set; }
        public int disk_usage { get; set; }
        public int collaborators { get; set; }
        public string two_factor_authentication { get; set; }
        public GitHubPlan plan { get; set; }
    }
}

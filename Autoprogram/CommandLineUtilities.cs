using System;
using System.Threading.Tasks;
using Octokit;

public class CommandLineUtilities
{
    private string _githubToken;

    public CommandLineUtilities(string githubToken)
    {
        _githubToken = githubToken;
    }

    public async Task CreateBranch(string repositoryOwner, string repositoryName, string branchName)
    {
        var github = new GitHubClient(new ProductHeaderValue("AutoProgram"))
        {
            Credentials = new Credentials(_githubToken)
        };

        try
        {
            var repo = await github.Repository.Get(repositoryOwner, repositoryName);
            var baseBranch = await github.Repository.Branch.Get(repositoryOwner, repositoryName, repo.DefaultBranch);
            var newBranch = await github.Git.Reference.Create(repositoryOwner, repositoryName, new NewReference($"refs/heads/{branchName}", baseBranch.Commit.Sha));

            Console.WriteLine($"New branch '{branchName}' created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating branch: {ex.Message}");
        }
    }

    public async Task CreatePullRequest(string repositoryOwner, string repositoryName, string branchName, string baseBranch)
    {
        var github = new GitHubClient(new ProductHeaderValue("AutoProgram"))
        {
            Credentials = new Credentials(_githubToken)
        };

        try
        {
            var newPullRequest = new NewPullRequest($"Pull request for {branchName}", branchName, baseBranch)
            {
                Body = $"This pull request contains changes made to the `{branchName}` branch."
            };

            var pullRequest = await github.PullRequest.Create(repositoryOwner, repositoryName, newPullRequest);

            Console.WriteLine($"Pull request created: {pullRequest.HtmlUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating pull request: {ex.Message}");
        }
    }
}
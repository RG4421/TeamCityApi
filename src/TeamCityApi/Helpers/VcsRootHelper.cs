﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCityApi.Clients;
using TeamCityApi.Domain;
using TeamCityApi.Logging;
using TeamCityApi.UseCases;

namespace TeamCityApi.Helpers
{
    public interface IVcsRootHelper
    {
        Task<IGitRepository> CloneAndBranch(string buildId, string branchName);
        bool PushAndDeleteLocalFolder(IGitRepository gitRepository,string branchName);
    }

    public class VcsRootHelper : IVcsRootHelper
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(VcsRootHelper));

        private readonly BuildClient _buildClient;
        private readonly VcsRootClient _vcsRootClient;
        private readonly GitRepositoryFactory _gitRepositoryFactory;

        public VcsRootHelper(BuildClient buildClient, VcsRootClient vcsRootClient, GitRepositoryFactory gitRepositoryFactory)
        {
            _buildClient = buildClient;
            _vcsRootClient = vcsRootClient;
            _gitRepositoryFactory = gitRepositoryFactory;
        }

        public async Task<VcsCommit> GetCommitInformationByBuildId(string buildId)
        {
            Log.Info(string.Format("Get Commit Information for Build: {0}",buildId));
            Build build = await _buildClient.ById(buildId);

            Log.Debug("Build Loaded from TeamCity");

            string commitSha = build.Revisions.First().Version;

            Log.Debug(string.Format("Commit SHA from first Revision: {0}",commitSha));

            VcsRootInstance vcsRootInstance = build.Revisions.First().VcsRootInstance;

            Log.Debug(string.Format("Get VCSRoot by Id: {0}", vcsRootInstance.Id));
            VcsRoot vcsRoot = await _vcsRootClient.ById(vcsRootInstance.Id.ToString());

            Log.Debug(string.Format("VCSRoot: {0}",vcsRoot));
            VcsCommit commit = new VcsCommit(vcsRoot, commitSha);

            return commit;

        }

        public async Task<IGitRepository> CloneAndBranch(string buildId, string branchName)
        {
            VcsCommit commit = await GetCommitInformationByBuildId(buildId);

            IGitRepository gitRepository = _gitRepositoryFactory.Clone(commit);

            if (gitRepository != null)
            {
                gitRepository.AddBranch(branchName, commit.CommitSha);
            }

            return gitRepository;
        }

        public bool PushAndDeleteLocalFolder(IGitRepository gitRepository,string branchName)
        {
            bool success = false;
            if (gitRepository.Push(branchName))
            {
                gitRepository.DeleteFolder();
                success = true;
            }
            return success;
        }
    }
}

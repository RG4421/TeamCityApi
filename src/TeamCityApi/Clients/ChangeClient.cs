﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCityApi.Domain;

namespace TeamCityApi.Clients
{
    public interface IChangeClient
    {
        Task<List<ChangeSummary>> GetAll();
        Task<List<ChangeSummary>> GetForBuild(long buildId);
        Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId);
        Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId, long sinceChangeId);
        Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId, bool pending);
        Task<Change> GetById(string changeId);
    }

    public class ChangeClient : IChangeClient
    {
        private readonly IHttpClientWrapper _http;

        private const string _baseUri = "/app/rest/changes";

        public ChangeClient(IHttpClientWrapper http)
        {
            _http = http;
        }

        public async Task<List<ChangeSummary>> GetAll()
        {
            var changes = await _http.Get<List<ChangeSummary>>(_baseUri);

            return changes;
        }

        public async Task<List<ChangeSummary>> GetForBuild(long buildId)
        {
            var changes = await _http.Get<List<ChangeSummary>>($"{_baseUri}?locator=build:(id:{buildId})");

            return changes;
        }

        public async Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId)
        {
            var changes = await _http.Get<List<ChangeSummary>>($"{_baseUri}?locator=buildType:(id:{buildConfigurationId})");

            return changes;
        }

        public async Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId, long sinceChangeId)
        {
            var changes = await _http.Get<List<ChangeSummary>>($"{_baseUri}?locator=buildType:(id:{buildConfigurationId}),sinceChange:(id:{sinceChangeId})");

            return changes;
        }

        public async Task<List<ChangeSummary>> GetForBuildConfig(string buildConfigurationId, bool pending)
        {
            var changes = await _http.Get<List<ChangeSummary>>($"{_baseUri}?locator=buildType:(id:{buildConfigurationId}),pending:{pending.ToString().ToLower()}");

            return changes;
        }

        public async Task<List<ChangeSummary>> GetForBuildConfig()
        {
            var changes = await _http.Get<List<ChangeSummary>>(_baseUri);

            return changes;
        }

        public async Task<Change> GetById(string changeId)
        {
            string requestUri = string.Format("{0}/id:{1}",_baseUri,  changeId);

            var change = await _http.Get<Change>(requestUri);

            return change;
        }
    }
}
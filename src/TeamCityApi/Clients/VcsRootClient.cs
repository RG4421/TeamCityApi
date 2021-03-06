﻿using System.Threading.Tasks;
using TeamCityApi.Domain;

namespace TeamCityApi.Clients
{
    public interface IVcsRootClient
    {
        Task<VcsRoot> ById(string projectId);
    }

    public class VcsRootClient : IVcsRootClient
    {
        private readonly IHttpClientWrapper _http;
        public VcsRootClient(IHttpClientWrapper http)
        {
            _http = http;
        }

        public async Task<VcsRoot> ById(string id)
        {
            string requestUri = string.Format("/app/rest/vcs-roots/id:{0}", id);

            return await _http.Get<VcsRoot>(requestUri);
        }
    }
}
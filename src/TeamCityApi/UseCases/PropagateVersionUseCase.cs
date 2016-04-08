﻿using System.Linq;
using System.Threading.Tasks;
using TeamCityApi.Helpers;
using TeamCityApi.Logging;

namespace TeamCityApi.UseCases
{
    public class PropagateVersionUseCase
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(PropagateVersionUseCase));

        private readonly ITeamCityClient _client;

        public PropagateVersionUseCase(ITeamCityClient client)
        {
            _client = client;
        }

        public async Task Execute(string buildConfigId, bool simulate = false)
        {
            Log.Info("Propagate version.");

            var buildConfig = await _client.BuildConfigs.GetByConfigurationId(buildConfigId);
            var majorVersion = buildConfig.Parameters[ParameterName.MajorVersion].Value;
            var minorVersion = buildConfig.Parameters[ParameterName.MinorVersion].Value;
            var buildConfigChain = new BuildConfigChain(_client.BuildConfigs, buildConfig);
            await PropagateVersion(buildConfigChain, majorVersion, minorVersion, simulate);
        }

        private async Task PropagateVersion(BuildConfigChain buildConfigChain, string majorVersion, string minorVersion, bool simulate)
        {
            foreach (var node in buildConfigChain.Nodes)
            {
                Log.Info($"Setting {node.Value.Id} to version {majorVersion}.{minorVersion}");
                if (!simulate)
                {
                    await _client.BuildConfigs.SetParameterValue(
                        l => l.WithId(node.Value.Id),
                        ParameterName.MajorVersion,
                        majorVersion,
                        true
                    );

                    await _client.BuildConfigs.SetParameterValue(
                        l => l.WithId(node.Value.Id),
                        ParameterName.MinorVersion,
                        minorVersion,
                        true
                    );
                }
            }
        }
    }
}
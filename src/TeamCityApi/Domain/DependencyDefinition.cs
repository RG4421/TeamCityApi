﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeamCityApi.Domain
{
    public class DependencyDefinition
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public Properties Properties { get; set; }

        [JsonProperty("source-buildType")]
        public BuildConfigSummary SourceBuildConfig { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Type: {1}, Properties: {2}", Id, Type, Properties);
        }
    }
}
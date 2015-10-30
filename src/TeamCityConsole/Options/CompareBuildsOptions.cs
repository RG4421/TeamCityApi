﻿using CommandLine;

namespace TeamCityConsole.Options
{
    [Verb(Verbs.CompareBuilds, HelpText = "Compare Builds.")]
    public class CompareBuildsOptions
    {
        [Option('o', "oldBuildId", Required = true, HelpText = "Old Build Id to compare.")]
        public long BuildId1 { get; set; }

        [Option('n', "newBuildId", Required = true, HelpText = "New Build Id to compare.")]
        public long BuildId2 { get; set; }

        [Option('b', "BeyondCompare", Required = false, HelpText = "Display comparison in BayondCompare app.")]
        public bool BCompare { get; set; }
    }
}
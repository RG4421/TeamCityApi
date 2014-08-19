﻿using System;
using System.Collections.Generic;
using System.Linq;
using TeamCityApi.Clients;
using TeamCityApi.Domain;
using TeamCityApi.Locators;
using Xunit;

namespace TeamCityApi.Tests
{
    public class Sandbox
    {
        [Fact]
        public void Build_ByLocator()
        {
            var buildClient = CreateBuildClient();

            List<BuildSummary> buildSummaries = buildClient.ByBuildLocator(x => x.SinceDate(new DateTime(2014, 05, 11))).Result;
        }

        [Fact]
        public void Build_ById()
        {
            var buildClient = CreateBuildClient();

            Build build = buildClient.ById("45").Result;
        }

        [Fact]
        public void Build_Artifacts()
        {
            var buildClient = CreateBuildClient();

            Build build = buildClient.ById("45").Result;

            List<File> files = build.ArtifactsReference.GetFiles().Result;
        }

        [Fact]
        public void GetFiles()
        {
            BuildClient buildClient = CreateBuildClient();

            List<File> result = buildClient.GetFiles(48).Result;

            List<File> files = result.First().GetChildren().Result;
        }

        public class BuildConfig
        {
            [Fact]
            public void ById()
            {
                var client = CreateBuildConfigClient();
                var buildConfig = client.GetByConfigurationId("Installers_29_Xu_297dev").Result;
            }
        }

        public class Project
        {
            [Fact]
            public void ById()
            {
                var client = CreateProjectClient();
                var project = client.GetById("id29_BIFLoader").Result;
            }
        }

        public class Change
        {
            [Fact]
            public void GetAll()
            {
                var client = CreateChnageClient();
                var changes = client.GetAll().Result;
            }

            [Fact]
            public void GetById()
            {
                var client = CreateChnageClient();
                var change = client.GetById("4").Result;
            }
        }

        public class Dependencies
        {
            [Fact]
            public void Snapshot()
            {
                var client = CreateBuildConfigClient();
                var summaries = client.GetAll().Result;
                //var buildConfig = client.GetByConfigurationId("FooCore_Master").Result;
                var buildConfig = summaries.First(x => x.Id == "FooCore_Master");
                var dependencyDefinition = new DependencyDefinition
                {
                    Id = buildConfig.Id,
                    Type = "snapshot_dependency",
                    Properties = new List<Property>
                    {
                        new Property() { Name = "run-build-if-dependency-failed", Value = "false" },
                        new Property() { Name = "take-successful-builds-only", Value = "true" },
                        //new Property() { Name = "run-build-on-the-same-agent", Value = "true" },
                        //new Property() { Name = "take-started-build-with-same-revisions", Value = "true" },
                    },
                    SourceBuildConfig = buildConfig
                };
                client.CreateDependency("foo_service_Master", dependencyDefinition).Wait();
            }

            [Fact]
            public void Snapshot_()
            {
                var client = CreateBuildConfigClient();
                var dependency = new CreateSnapshotDependency("foo_service_Master","FooCore_Master");
                client.CreateSnapshotDependency(dependency).Wait();
            }
        }

        private static ChangeClient CreateChnageClient()
        {
            var http = CreateHttpClientWrapper();
            var client = new ChangeClient(http);
            return client;
        }

        private static ProjectClient CreateProjectClient()
        {
            var http = CreateHttpClientWrapper();
            var projectClient = new ProjectClient(http);
            return projectClient;
        }

        private static BuildConfigClient CreateBuildConfigClient()
        {
            var http = CreateHttpClientWrapper();
            var buildConfigClient = new BuildConfigClient(http);
            return buildConfigClient;
        }

        private static HttpClientWrapper CreateHttpClientWrapper()
        {
            var http = new HttpClientWrapper("localhost:8090", "administrator", "admin");
            return http;
        }

        private static BuildClient CreateBuildClient()
        {
            var http = new HttpClientWrapper("localhost:8090", "administrator", "admin");
            var buildClient = new BuildClient(http);
            return buildClient;
        }
    }
}
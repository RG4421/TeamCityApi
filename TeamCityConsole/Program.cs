﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Funq;
using TeamCityApi;
using TeamCityConsole.Commands;
using TeamCityConsole.Options;
using TeamCityConsole.Utils;

namespace TeamCityConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = SetupContainer();

            DisplayAssemblyInfo();

            ParserResult<object> result = Parser.Default.ParseArguments(args, typeof(GetArtifactOptions), typeof(GetDependenciesOptions));

            if (result.Errors.OfType<HelpRequestedError>().Any())
            {
                return;
            }

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    Console.Out.WriteLine(error);
                }
                return;
            }

            dynamic options = result.Value;

            ICommand command = container.ResolveNamed<ICommand>(GetVerb(options));

            Task displayTask = Task.Run(async () =>
            {
                Console.Out.WriteLine("Downloading");
                while (true)
                {
                    Console.Out.Write(".");
                    await Task.Delay(400);
                }
            });


            try
            {
                Task downloadTask = command.Execute(options);
                downloadTask.Wait();
            }
            catch (AggregateException e)
            {
                foreach (Exception innerException in e.Flatten().InnerExceptions)
                {
                    Console.Out.WriteLine(innerException);
                }
            }
        }

        private static string GetVerb(object options)
        {
            return options.GetType().GetCustomAttribute<VerbAttribute>().Name;
        }

        private static void DisplayAssemblyInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.Out.WriteLine("Product: {0}", fvi.ProductName);
            Console.Out.WriteLine("Company: {0}", fvi.CompanyName);
            Console.Out.WriteLine("Assembly version: {0}", assembly.GetName().Version);
            Console.Out.WriteLine("File version: {0}", fvi.FileVersion);
        }

        private static Container SetupContainer()
        {
            var container = new Container();

            container.Register<IHttpClientWrapper>(new HttpClientWrapper(Settings.TeamCityUri, Settings.Username,
                Settings.Password));

            container.Register<ITeamCityClient>(x => new TeamCityClient(x.Resolve<IHttpClientWrapper>()));

            container.Register<IFileDownloader>(x => new FileDownloader(x.Resolve<IHttpClientWrapper>()));

            container.Register<IFileSystem>(new FileSystem());

            container.Register<ICommand>(Verbs.GetDependencies,
                x => new ResolveDependencyCommand(x.Resolve<ITeamCityClient>(), x.Resolve<IFileDownloader>(), x.Resolve<IFileSystem>()));

            container.Register<ICommand>(Verbs.GetArtifacts, x => new DownloadArtifactCommand(x.Resolve<IFileSystem>()));
            return container;
        }
    }
}

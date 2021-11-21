using System;
using System.Text;
using CliWrap;
using CliWrap.Builders;
using JetBrains.Util;
using ReSharperPlugin.RiderEfCore.Cli.Net6.Models;

namespace ReSharperPlugin.RiderEfCore.Cli.Net6
{
    public abstract class EfCoreToolsSectionBase
    {
        protected readonly ILogger Logger;

        public EfCoreToolsSectionBase(ILogger logger)
        {
            Logger = logger;
        }

        protected EfCoreCommandResult ExecuteLogged(Command command, int expectedStatusCode = 0)
        {
            Logger.Info("Executing CLI command: {0}", command);

            var sb = new StringBuilder();

            command |= sb;
            command |= output => Logger.Info($"dotnet ef: {output}");

            var cliCommand = command.ToString();
            var commandResult = command.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Logger.Info("Exit code: {0}", commandResult.ExitCode);

            return new EfCoreCommandResult(cliCommand, commandResult.ExitCode == expectedStatusCode, commandResult.ExitCode, sb.ToString());
        }

        // TODO: Add other common options
        protected Command CreateEfCoreCommand(
            EfCoreCommandName commandName,
            EfCoreCommonOptions options,
            Action<ArgumentsBuilder> argsBuilder = null,
            bool requireZeroStatusCode = true)
        {
            var command = CliWrap.Cli.Wrap("dotnet").WithArguments(args =>
            {
                args
                    .Add("ef")
                    .Add(commandName.CommandParts)
                    .Add("--project").Add(options.MigrationsProject)
                    .Add("--startup-project").Add(options.StartupProject);

                if (options.NoBuild)
                {
                    args.Add("--no-build");
                }

                argsBuilder?.Invoke(args);
            });

            return requireZeroStatusCode
                ? command.WithValidation(CommandResultValidation.ZeroExitCode)
                : command;
        }
    }
}
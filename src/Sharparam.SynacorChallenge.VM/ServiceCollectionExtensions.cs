namespace Sharparam.SynacorChallenge.VM
{
    using Commands;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVmServices<TOutputWriter, TInputReader>(this IServiceCollection services)
            where TOutputWriter : class, IOutputWriter
            where TInputReader : class, IInputReader
        {
            services.AddSingleton<IOutputWriter, TOutputWriter>();
            services.AddSingleton<IInputReader, TInputReader>();

            services.AddTransient<ICommand, SaveStateCommand>()
                .AddTransient<ICommand, LoadStateCommand>()
                .AddTransient<ICommand, AddressCommand>()
                .AddTransient<ICommand, ExitCommand>();

            return services.AddTransient<Cpu>().AddTransient<CommandManager>();
        }
    }
}

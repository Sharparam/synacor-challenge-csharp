namespace Sharparam.SynacorChallenge.VM
{
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
            return services.AddTransient<Cpu>();
        }
    }
}

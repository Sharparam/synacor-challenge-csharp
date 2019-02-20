namespace Sharparam.SynacorChallenge.Console
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using VM;

    using static System.Console;

    using VmProgram = Sharparam.SynacorChallenge.VM.Program;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(lb => lb.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));

            services.AddVmServices<ConsoleOutputWriter, ConsoleInputReader>();

            var serviceProvider = services.BuildServiceProvider();

            var path = args.Length > 0 ? args[0] : "challenge.bin";

            var program = VmProgram.FromFile(path);

            var cpu = serviceProvider.GetRequiredService<Cpu>();

            cpu.LoadProgram(program);
            ////cpu.LoadProgram(new ushort[] { 9, 32768, 32769, 4, 19, 32768 });
            cpu.Run();

            WriteLine();

            WriteLine("Press any key to exit");
            ReadKey();
        }
    }
}

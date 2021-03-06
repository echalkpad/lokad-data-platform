using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Platform.TestClient.Commands
{
    public class StartLocalServerProcessor : ICommandProcessor
    {
        public string Key { get { return "START"; } }
        public string Usage { get { return @"START [args]
    Starts local server as a separate process, passing it storage and port parameters."; } }


        public bool Execute(CommandProcessorContext context, CancellationToken token, string[] args)
        {
            var file = @"..\server\Platform.Node.exe";
            if (!File.Exists(file))
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                
                file = Path.Combine(dir, file);
            }
            if (!File.Exists(file))
            {
                context.Log.Error("Not found {0}. Did you compile?", file);
                return false;
            }

            var ip = context.Client.Options.Ip;
            if (ip != "localhost" && ip != "127.0.0.1")
            {
                context.Log.Error("Client IP should be localhost or 127.0.0.1. Was {0}", ip);
                return false;
            }
            var all = string.Join(" ", args);
            var arguments = string.Format("-h {0} -s {1} {2}", context.Client.Options.HttpPort, context.Client.Options.StoreLocation, all);
            context.Log.Debug("Starting {0} with args {1}", file, arguments);
            var proc = Process.Start(new ProcessStartInfo(file, arguments));

            token.WaitHandle.WaitOne(1000 * 2);
            context.Log.Debug("Consider as started");
            return true;
        }
    }
}
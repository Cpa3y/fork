using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public sealed class FileConnection : IConnection
    {
        public bool IsAlive { get; private set; }

        private readonly StreamWriter writer;
        private readonly ILogger logger;

        public FileConnection(string filename, ILogger logger)
        {
            this.logger = logger;
            try
            {
                this.writer = File.AppendText(filename);
                IsAlive = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unable to open file");
                IsAlive = false;
            }
        }

        public Task<ReadResult> Read(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Write(string line)
        {
            try
            {
                await writer.WriteAsync($"[{DateTime.Now:HH:mm:ss.fff}] ");
                await writer.WriteLineAsync(line);
                await writer.FlushAsync();
                IsAlive = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error writing to file");
                IsAlive = false;
                return false;
            }
        }

        public void Dispose()
        {
            writer?.Dispose();
        }
    }
}

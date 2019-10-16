using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Connections
{
    public class SerialPortConnection : IConnection
    {
        private readonly SerialPort serial;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public SerialPortConnection(SerialPort serial)
        {
            this.serial = serial;
            this.reader = new StreamReader(serial.BaseStream);
            this.writer = new StreamWriter(serial.BaseStream);
        }

        public bool IsAlive => serial.IsOpen;

        public Task<string> Read() => reader.ReadLineAsync();

        public async Task Write(string line)
        {
            await writer.WriteLineAsync(line);
            await writer.FlushAsync();
        }
    }
}

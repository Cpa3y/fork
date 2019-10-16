using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Connections
{
    public class SerialPortFactory
    {
        public static IConnection Open(string connection)
        {
            var p = connection.Split(';');
            var portName = p[0];
            var baudRate = p.Length >= 2 ? int.Parse(p[1]) : 9600; 
            var parity = p.Length >= 3 ? (Parity)Enum.Parse(typeof(Parity), p[2], true) : Parity.None;
            var databits = p.Length >= 4 ? int.Parse(p[3]) : 8;
            var stopbits = p.Length >= 5 ? (StopBits)Enum.Parse(typeof(StopBits), p[4], true) : StopBits.One;

            var port = new SerialPort(portName, baudRate, parity, databits, stopbits);
            port.Open();

            return new SerialPortConnection(port);
        }
    }
}

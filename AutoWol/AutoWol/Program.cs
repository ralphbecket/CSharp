using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoWol
{
    /// <summary>
    /// AutoWol {MACAddress} [{ProgName}] [{InitialPoll=10}] [{LaterPoll=60}]
    /// 
    /// Automatically send a Wake-on-Lan magic packet to the given MAC
    /// address whenever a particular program starts.
    /// 
    /// If only {MACAddress} is supplied then the WoL packed will be sent and
    /// the program will terminate.
    /// 
    /// Otherwise the program will poll the current running process every
    /// InitialPoll seconds
    /// </summary>
    static class Program
    {
        static void Main(string[] args)
        {
            var numArgs = args.Length;
            var macStr = (0 < numArgs ? args[0] : "");
            var progStr = (1 < numArgs ? args[1] : "");
            var initialPollStr = (2 < numArgs ? args[2] : "10");
            var laterPollStr = (3 < numArgs ? args[3] : "60");
            var initialPoll = 0.0;
            var laterPoll = 0.0;
            byte[] mac;
            Regex progRegex;

            try
            {
                mac = macStr
                    .Split(':')
                    .Select(x => Convert.ToByte(x, 16))
                    .ToArray();
                progRegex = new Regex(progStr, RegexOptions.IgnoreCase);
                initialPoll = double.Parse(initialPollStr);
                laterPoll = double.Parse(laterPollStr);
                if (mac.Length != 6) throw new ApplicationException("D'oh!");
                if (initialPoll < 1) throw new ApplicationException("D'oh!");
                if (laterPoll < 1) throw new ApplicationException("D'oh!");
            }
            catch
            {
                Console.WriteLine("usage: AutoWol <MACAddress> [<ProgName>] " +
                    "[<InitialPoll=10>] [<LaterPoll=60>]");
                return;
            }

            // If there's no program pattern to look for, just send the packet.
            if (string.IsNullOrEmpty(progStr))
            {
                SendMagicPacket(mac);
                return;
            }

            // Otherwise, it's a polling we will go.
            while (true)
            {
                var haveProgMatch = HaveProgMatch(progRegex);
                var delay = initialPoll;
                if (haveProgMatch)
                {
                    delay = laterPoll;
                    SendMagicPacket(mac);
                }
                Task.Delay((int)(delay * 1000)).Wait();
            }

        }

        internal static bool HaveProgMatch(Regex progRegex)
        {
            return System.Diagnostics.Process.GetProcesses()
                .Any(x => progRegex.IsMatch(x.ProcessName));
        }

        internal static void SendMagicPacket(byte[] mac)
        {
            var magicPacket =
                Enumerable.Repeat((byte)0xff, 6)
                    .Concat(Enumerable.Range(0, 16).SelectMany(i => mac))
                    .ToArray();
            Console.WriteLine("Sending magic wake-on-lan packet...");
            using (var udpClient = new System.Net.Sockets.UdpClient())
            {
                udpClient.Connect(System.Net.IPAddress.Broadcast, 7); // Lucky port.
                udpClient.Send(magicPacket, magicPacket.Length);
            }
        }
    }
}

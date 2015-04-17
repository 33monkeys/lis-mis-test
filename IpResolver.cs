using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lis.Test
{
    [TestFixture]
    class IpResolver
    {
        [Test]
        public void GetHostIp()
        {
            var ipAndHost = DnsHelper.GetHostIpAndPort();
            Assert.That(ipAndHost, Is.EqualTo("http://localhost:50883"));
        }
    }

    internal class DnsHelper
    {
        public static string GetHostIpAndPort()
        {
            var hostName = Dns.GetHostName();
            var hostIps = Dns.GetHostAddresses(hostName);
            foreach (var ipAddress in hostIps)
            {
                if (ipAddress.IsIPv6LinkLocal)
                {
                    return ipAddress.ToString();
                }
            }

            return null;
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public class ModAuthPubTktAlgorithm : IDisposable
    {
        public static readonly DateTimeOffset UNIX_EPOCH = new DateTimeOffset(1970, 01, 01, 0, 0, 0, TimeSpan.Zero);

        protected readonly X509Certificate2 _cert;

        private ModAuthPubTktAlgorithm()
        {
            this.HashAlgorithmName = HashAlgorithmName.SHA1;
        }

        public ModAuthPubTktAlgorithm(string certificateFile, string password) : this()
        {            
            _cert = new X509Certificate2(certificateFile, password, X509KeyStorageFlags.Exportable);
        }

        public ModAuthPubTktAlgorithm(byte[] certificate, string password) : this()
        {            
            _cert = new X509Certificate2(certificate, password, X509KeyStorageFlags.Exportable);
        }

        public HashAlgorithmName HashAlgorithmName { get; set; }

        public bool Verify(string ticket)
        {
            var rsa = _cert.GetRSAPublicKey();
            var idx = ticket.IndexOf(";sig=");
            var pair = new string[]{ ticket.Substring(0, idx), ticket.Substring(idx+5) };

            byte[] bsignature = Convert.FromBase64String(pair[1]);
            byte[] bdata = System.Text.UTF8Encoding.UTF8.GetBytes(pair[0]);

            return rsa.VerifyData(bdata, bsignature, this.HashAlgorithmName, RSASignaturePadding.Pkcs1);
        }
        public string Sign(IDictionary<string, string> elements)
        {
            var ticketString = new StringBuilder();
            var rsa = _cert.GetRSAPrivateKey();
            ticketString.Append(string.Join(";", elements.Select(s => string.Concat(s.Key, "=", s.Value))));

            var sbytes = System.Text.UTF8Encoding.UTF8.GetBytes(ticketString.ToString());
            var hash = rsa.SignData(sbytes, this.HashAlgorithmName, RSASignaturePadding.Pkcs1);

            ticketString.Append(";sig=").Append(Convert.ToBase64String(hash));

            return ticketString.ToString();
        }

        public string Sign(string uid, DateTimeOffset expires, TimeSpan? gracePeriod = null, string cip = null, IEnumerable<string> tokens = null, string udata = null, string bauth = null)
        {
            var ticketString = new StringBuilder();
            var ticketElements = new Dictionary<string, string>();

            ticketElements.Add("uid", uid);
            ticketElements.Add("validuntil", expires.Subtract(UNIX_EPOCH).TotalSeconds.ToString("0"));

            if (gracePeriod.HasValue && gracePeriod.Value > TimeSpan.Zero)
            {
                ticketElements.Add("graceperiod", expires.Subtract(gracePeriod.Value).Subtract(UNIX_EPOCH).TotalSeconds.ToString("0"));
            }

            if (cip != null)
            {
                ticketElements.Add("cip", cip);
            }

            if (tokens != null)
            {
                ticketElements.Add("tokens", string.Join(",", tokens));
            }

            if (udata != null)
            {
                ticketElements.Add("udata", udata);
            }          
            
            if (bauth != null)
            {
                ticketElements.Add("bauth", bauth);
            }

            return Sign(ticketElements);
        }

        public void Dispose()
        {
            _cert.Dispose();
        }
    }
}

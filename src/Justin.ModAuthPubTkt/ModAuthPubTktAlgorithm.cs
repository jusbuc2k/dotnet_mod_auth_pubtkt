using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace Justin.ModAuthPubTkt
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
            ticketString.Append(string.Join(";", elements.Where(x => x.Key != "sig").Select(s => string.Concat(s.Key, "=", s.Value))));

            var sbytes = System.Text.UTF8Encoding.UTF8.GetBytes(ticketString.ToString());
            var hash = rsa.SignData(sbytes, this.HashAlgorithmName, RSASignaturePadding.Pkcs1);

            ticketString.Append(";sig=").Append(Convert.ToBase64String(hash));

            return ticketString.ToString();
        }

        public string Sign(ModAuthPubTkt tkt)
        {
            return Sign(tkt.GetElements());
        }

        public void Dispose()
        {
            _cert.Dispose();
        }
    }
}

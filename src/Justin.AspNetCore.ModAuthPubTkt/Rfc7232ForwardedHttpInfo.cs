using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public struct Rfc7232ForwardedHttpInfo
    {
        public string By { get; set; }
        public string For { get; set; }
        public string Host { get; set; }
        public string Proto { get; set; }
    }
}

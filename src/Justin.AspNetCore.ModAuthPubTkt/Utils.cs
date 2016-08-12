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
    public static class Utils
    {
        public static IDictionary<string, string> ParseTicket(string ticket)
        {
            var dictionary = new Dictionary<string, string>();
            var elements = ticket.Split(';');
            string[] pair;

            foreach (var element in elements)
            {
                pair = element.Split('=');
                dictionary.Add(pair[0],pair[1]);
            }

            return dictionary;           
        }
    }
}
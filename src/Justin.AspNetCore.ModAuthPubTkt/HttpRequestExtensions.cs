using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Justin.AspNetCore.ModAuthPubTkt
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets the first value from a header matching the given keys, and then returns the first element of the value if the value is a comma seperated list of values.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        internal static string GetFirstValue(this IHeaderDictionary headers, params string[] keys)
        {
            Microsoft.Extensions.Primitives.StringValues values;
            string returnValue;

            foreach (var key in keys)
            {
                if (headers.TryGetValue(key, out values))
                {
                    returnValue = values.FirstOrDefault(val => !string.IsNullOrEmpty(val));
                    if (returnValue != null)
                    {
                        return returnValue.Split(',')[0];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets HTTP forwarding information using the RFC7232 standard.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetRfc7232Info(this IHeaderDictionary headers, out Rfc7232ForwardedHttpInfo result)
        {
            result = new Rfc7232ForwardedHttpInfo();
            var headerValue = headers.GetFirstValue("Forwarded");

            if (headerValue == null)
            {
                return false;
            }

            string[] elements = headerValue.Split(';');
            string[] pair;

            foreach (var element in elements)
            {
                pair = element.Split(new char[] { '=' }, 2);
                if (pair[0].Equals("By", StringComparison.OrdinalIgnoreCase))
                {
                    result.By = pair[1];
                }
                else if (pair[0].Equals("For", StringComparison.OrdinalIgnoreCase))
                {
                    result.For = pair[1];
                }
                else if (pair[0].Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    result.Host = pair[1];
                }
                else if (pair[0].Equals("Proto", StringComparison.OrdinalIgnoreCase))
                {
                    result.Proto = pair[1];
                }
            }

            return true;
        }
    }
}

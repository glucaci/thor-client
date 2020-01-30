using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Thor.Core.Abstractions;

namespace Thor.Hosting.AspNetCore
{
    internal static class HttpRequestExtensions
    {
        public static Guid? GetActivityId(this HttpContext context)
        {
            var request = context.Request;

            if (request != null &&
                request.Headers.TryGetValue(MessageHeaderKeys.ActivityId, out StringValues rawActivityIds) &&
                rawActivityIds.Count > 0 &&
                Guid.TryParse(rawActivityIds[0], out Guid id) &&
                id != Guid.Empty)
            {
                return id;
            }

            return null;
        }

        public static Guid? GetRootId(this HttpContext context)
        {
            var request = context.Request;

            if (request != null &&
                request.Headers.TryGetValue(MessageHeaderKeys.RootId, out StringValues rawActivityIds) &&
                rawActivityIds.Count > 0 &&
                Guid.TryParse(rawActivityIds[0], out Guid id) &&
                id != Guid.Empty)
            {
                return id;
            }

            return null;
        }
    }
}
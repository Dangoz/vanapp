using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user) {
            Console.WriteLine(user.FindFirst(ClaimTypes.Name)?.Value);
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using SevenDays.Api.Entities;

namespace SevenDaysApi.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<SimpleUser> WithoutPasswords(this IEnumerable<SimpleUser> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static SimpleUser WithoutPassword(this SimpleUser user)
        {
            user.Password = null;
            return user;
        }
    }
}

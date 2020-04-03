using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.Api.Helpers
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        string Hash2(string password);

        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
        bool Check2(string hash, string password);

    }
}

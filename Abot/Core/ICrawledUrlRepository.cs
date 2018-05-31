using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abot.Core
{
    public interface ICrawledUrlRepository : IDisposable
    {
        bool Contains(Uri uri);
        bool AddIfNew(Uri uri);
    }
}

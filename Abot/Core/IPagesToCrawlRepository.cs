using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abot.Poco;

namespace Abot.Core
{
    public interface IPagesToCrawlRepository : IDisposable
    {
        void Add(PageToCrawl page);
        PageToCrawl GetNext();
        void Clear();
        int Count();

    }
}

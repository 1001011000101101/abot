﻿using System;
using System.Collections.Concurrent;

namespace Abot.Core
{
    [Serializable]
    public class InMemoryCrawledUrlRepository : ICrawledUrlRepository
    {
        ConcurrentDictionary<string, byte> _urlRepository = new ConcurrentDictionary<string, byte>();

        public bool Contains(Uri uri)
        {
            return _urlRepository.ContainsKey(uri.AbsoluteUri);
        }

        public bool AddIfNew(Uri uri)
        {
            return _urlRepository.TryAdd(uri.AbsoluteUri, 0);
        }

        public virtual void Dispose()
        {
            _urlRepository = null;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Abot.Core
{
    /// <summary>
    /// Implementation that stores a numeric hash of the url instead of the url itself to use for lookups. This should save space when the crawled url list gets very long. 
    /// </summary>
    public class CompactCrawledUrlJsonRepository : ICrawledUrlRepository
    {
        private ConcurrentDictionary<long, byte> m_UrlRepository;
        string _path;
        int _interval;

        public CompactCrawledUrlJsonRepository()
        {
            _interval = 100;
            m_UrlRepository = new ConcurrentDictionary<long, byte>();
        }

        public CompactCrawledUrlJsonRepository(string FullPath, int SerializeInterval)
        {
            _path = FullPath;
            _interval = SerializeInterval;

            m_UrlRepository = File.Exists(_path) ? Deserialize(_path) : new ConcurrentDictionary<long, byte>();

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(_path));
            if (!di.Exists)
            {
                di.Create();
            }
        }

        /// <inheritDoc />
        public bool Contains(Uri uri)
        {
            return m_UrlRepository.ContainsKey(ComputeNumericId(uri.AbsoluteUri));
        }

        /// <inheritDoc />
        public bool AddIfNew(Uri uri)
        {
            bool result = m_UrlRepository.TryAdd(ComputeNumericId(uri.AbsoluteUri), 0);

            if (result && m_UrlRepository.Count % _interval == 0)
            {
                Serialize(m_UrlRepository, _path);
            }

            return result;
        }

        /// <inheritDoc />
        public virtual void Dispose()
        {
            m_UrlRepository = null;
        }

        protected long ComputeNumericId(string p_Uri)
        {
            byte[] md5 = ToMd5Bytes(p_Uri);

            long numericId = 0;
            for (int i = 0; i < 8; i++)
            {
                numericId += (long)md5[i] << (i * 8);
            }

            return numericId;
        }

        protected byte[] ToMd5Bytes(string p_String)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.Default.GetBytes(p_String));
            }
        }

        private void Serialize(ConcurrentDictionary<long, byte> M_UrlRepository, string FullPath)
        {
            string serialized = JsonConvert.SerializeObject(M_UrlRepository);
            File.WriteAllText(FullPath, serialized);
        }

        private ConcurrentDictionary<long, byte> Deserialize(string FullPath)
        {
            return JsonConvert.DeserializeObject<ConcurrentDictionary<long, byte>>(File.ReadAllText(_path));
        }
    }
}

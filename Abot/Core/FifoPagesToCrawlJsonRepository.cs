using Abot.Poco;
using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Abot.Core;

namespace Abot.Core
{
    [Serializable]
    public class FifoPagesToCrawlJsonRepository : IPagesToCrawlRepository
    {
        ConcurrentQueue<PageToCrawl> _urlQueue = new ConcurrentQueue<PageToCrawl>();
        string _path;
        int _interval;

        public FifoPagesToCrawlJsonRepository()
        {
            _interval = 100;
        }

        public FifoPagesToCrawlJsonRepository(string FullPath, int SerializeInterval)
        {
            _path = FullPath;
            _interval = SerializeInterval;
            Deserialize();
        }

        public void Add(PageToCrawl page)
        {
            _urlQueue.Enqueue(page);
            Serialize();
        }

        public PageToCrawl GetNext()
        {
            PageToCrawl pageToCrawl;
            _urlQueue.TryDequeue(out pageToCrawl);

            return pageToCrawl;
        }

        public void Clear()
        {
            _urlQueue = new ConcurrentQueue<PageToCrawl>();
        }

        public int Count()
        {
            return _urlQueue.Count;
        }

        public virtual void Dispose()
        {
            _urlQueue = null;
        }

        private void Serialize()
        {
            if (_path == string.Empty) return;

            if (Count() % _interval == 0)
            {
                string serialized = JsonConvert.SerializeObject(_urlQueue);
                File.WriteAllText(_path, serialized);
            }
        }

        private void Deserialize()
        {
            if (_path == string.Empty) return;

            var di = new DirectoryInfo(Path.GetDirectoryName(_path));
            if (!di.Exists)
            {
                di.Create();
                return;
            }

            if (File.Exists(_path))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new ResultConverter());
                _urlQueue = JsonConvert.DeserializeObject<ConcurrentQueue<PageToCrawl>>(File.ReadAllText(_path), settings);
            }
        }
    }
}

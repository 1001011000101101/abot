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
        ConcurrentQueue<PageToCrawl> _urlQueue;
        string _path;
        int _interval;

        public FifoPagesToCrawlJsonRepository()
        {
            _urlQueue = new ConcurrentQueue<PageToCrawl>();
            _interval = 100;
        }

        public FifoPagesToCrawlJsonRepository(string FullPath, int SerializeInterval)
        {
            _path = FullPath;
            _interval = SerializeInterval;

            _urlQueue = File.Exists(_path) ? Deserialize(_path) : new ConcurrentQueue<PageToCrawl>();

            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(_path));
            if (!di.Exists)
            {
                di.Create();
            }
        }

        public void Add(PageToCrawl page)
        {
            _urlQueue.Enqueue(page);

            if (Count() % _interval == 0)
            {
                Serialize(_urlQueue, _path);
            }
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

        private void Serialize(ConcurrentQueue<PageToCrawl> UrlQueue, string FullPath)
        {
            string serialized = JsonConvert.SerializeObject(UrlQueue);
            File.WriteAllText(FullPath, serialized);
        }

        private ConcurrentQueue<PageToCrawl> Deserialize(string FullPath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ResultConverter());
            return JsonConvert.DeserializeObject<ConcurrentQueue<PageToCrawl>>(File.ReadAllText(FullPath), settings);
        }
    }
}

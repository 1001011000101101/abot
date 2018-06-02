using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Abot.Poco;
using System.Dynamic;

namespace Abot.Core
{
    //https://stackoverflow.com/questions/23017716/json-net-how-to-deserialize-without-using-the-default-constructor
    class ResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(PageToCrawl));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            PageToCrawl result = new PageToCrawl();

            //This is bad because if PageToCrawl will be changed (added new field for example) 
            //deserialization will break and it hard to catch this type of error!
            result.Uri = (Uri)jo["Uri"];
            result.ParentUri = (Uri)jo["ParentUri"];
            result.IsRetry = (bool)jo["IsRetry"];
            result.RetryAfter = (double?)jo["RetryAfter"];
            result.RetryCount = (int)jo["RetryCount"];
            result.LastRequest = (DateTime?)jo["LastRequest"];
            result.IsRoot = (bool)jo["IsRoot"];
            result.IsInternal = (bool)jo["IsInternal"];
            result.CrawlDepth = (int)jo["CrawlDepth"];
            result.RedirectedFrom = jo["RedirectedFrom"].ToObject<CrawledPage>();
            result.RedirectPosition = (int)jo["RedirectPosition"];
            result.PageBag = jo["PageBag"]?.ToObject<ExpandoObject>() ?? new ExpandoObject();
            

            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

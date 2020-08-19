using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Models;
using Newtonsoft.Json;

namespace Core
{
    public static class CachingService
    {
        public static void Save<T>(IEnumerable<T> content, CachingType type)
        {

            if (File.Exists($"{type}")) File.Delete($"{type}");
            File.WriteAllText($"{type}", Newtonsoft.Json.JsonConvert.SerializeObject(content));

        }

        public static T Load<T>(CachingType type)
        {
            if (File.Exists($"{type}")) return JsonConvert.DeserializeObject<T>(File.ReadAllText($"{type}"));

            return default(T);
        }
    }
}

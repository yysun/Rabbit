using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

namespace Rabbit
{
    /// <summary>
    /// Summary description for ContentStore
    /// </summary>
    public class Repository
    {
        private static Dictionary<string, object> cache = new Dictionary<string, object>();
        private static object locker = new object();

        public string type { get; private set; }

        private static string BaseFolder
        {
            get { return HttpContext.Current.Server.MapPath("~/App_Data/Rabbit/"); }
        }

        public Repository(string type)
        {
            this.type = type;
        }

        public string Save(string id, ExpandoObject data)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(id));
            Assert.IsTrue(data != null);

            id = GetSafeId(id);

            var cachekey = type + "." + id;
            var fileName = Path.Combine(BaseFolder + type, id);
            var text = data.ToJson();

            lock (locker)
            {
                cache[cachekey] = data;
                File.WriteAllText(fileName, text);
            }

            return id;
        }

        private string GetSafeId(string id)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                id = id.Replace(c, '-');
            }
            foreach (char c in @" ~`!@#$%^&+=,;""".ToCharArray())
            {
                id = id.Replace(c, '-');
            }
            return id;
        }

        public ExpandoObject Load(string id)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(id));
            id = GetSafeId(id);

            var cachekey = type + "." + id;

            if (cache.ContainsKey(cachekey))
            {
                return cache[cachekey] as dynamic;
            }

            lock (locker)
            {
                var fileName = Path.Combine(BaseFolder + type, id);
                if (!File.Exists(fileName)) return null;

                dynamic data = new ExpandoObject();
                var text = File.ReadAllText(fileName);
                data = text.ToDynamic();

                cache[cachekey] = data;
                return data;
            }
        }

        public bool Exists(string id)
        {
            return Load(id) != null;
        }

        public void Delete(string id)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(id));
            id = GetSafeId(id);

            var cachekey = type + "." + id;

            var fileName = Path.Combine(BaseFolder + type, id);
            if (File.Exists(fileName)) File.Delete(fileName);

            if (cache.ContainsKey(cachekey))
            {
                cache.Remove(cachekey);
            }
        }

        public IEnumerable<ExpandoObject> List(Func<ExpandoObject, bool> filter = null)
        {
            var folder = Path.Combine(BaseFolder, type);
            if (!Directory.Exists(folder)) return null;
            var list = Directory.EnumerateFiles(folder)
                                .Select(f => Load(Path.GetFileName(f)));

            if (filter != null) list = list.Where(p => filter(p));
            return list;
        }
    }
}
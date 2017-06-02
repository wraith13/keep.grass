using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using RuyiJinguBang;

namespace keep.grass
{
    public class AlphaResouceEntry
    {
        public Assembly Assembly;
        public string Path;
        public byte[] Binary;
    }
    public class AlphaResouceProxy
    {
        static AlphaResouceEntry[] Cache = new AlphaResouceEntry[] { };

        static public async Task<byte[]> GetWithoutCache(Assembly Assembly, string Path)
        {
            using(var stream = Assembly.GetManifestResourceStream(Path))
            {
                var result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, (int)stream.Length);
                return result;
            }
        }
        static public byte[] GetFromCache(Assembly Assembly, string Path)
        {
            return Cache
                .Where(i => i.Assembly.GetName().FullName == Assembly.GetName().FullName && i.Path == Path)
                .Select(i => i.Binary)
                .FirstOrDefault();
        }
        static public async Task<byte[]> Get(Assembly Assembly, string Path)
        {
            var result = GetFromCache(Assembly, Path);
            if (null == result)
            {
                try
                {
                    result = await GetWithoutCache(Assembly, Path);
                    if (null != result)
                    {
                        lock (Cache)
                        {
                            Cache = Cache.Concat
                            (
                                new AlphaResouceEntry
                                {
	                                Assembly = Assembly,
	                                Path = Path,
                                    Binary = result,
                                }
                            )
                            .ToArray();
                        }
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine("AlphaImageProxy::Get::catch::err" + err.ToString());
                }
            }
            return result;
        }
    }
}

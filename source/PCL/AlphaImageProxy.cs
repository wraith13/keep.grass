using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace keep.grass
{
	public class AlphaImageEntry
	{
		public string Url;
		public byte[] Binary;
		public DateTime Stamp;
	}
	public class AlphaImageProxy
	{
		static AlphaImageEntry[] Cache = new AlphaImageEntry[] { };

		static public async Task<byte[]> GetWithoutCache(string Url)
		{
			return await AlphaFactory.MakeSureDomain().GetByteArrayFromUrlAsync(Url);
		}
		static public byte[] GetFromCache(string Url)
		{
			return Cache
				.Where(i => i.Url == Url)
				.Select(i => i.Binary)
				.FirstOrDefault();
		}
		static public async Task<byte[]> Get(string Url)
		{
			var Now = DateTime.Now;
			var Expire = Now - TimeSpan.FromMinutes(60);
			var result = Cache
				.Where(i => i.Url == Url && Expire < i.Stamp)
				.Select(i => i.Binary)
				.FirstOrDefault();
			if (null == result)
			{
				try
				{
					result = await GetWithoutCache(Url);
					if (null != result)
					{
						lock (Cache)
						{
							Cache = Cache
								.Where(i => Expire < i.Stamp)
								.Concat
								(
									new AlphaImageEntry
									{
										Url = Url,
										Binary = result,
										Stamp = Now,
									}
								)
								.ToArray();
						}
					}
				}
				catch(Exception err)
				{
					Debug.WriteLine("AlphaImageProxy::Get::catch::err" + err.ToString());
					result = Cache
						.Where(i => i.Url == Url)
						.Select(i => i.Binary)
						.FirstOrDefault();
				}
			}
			return result;
		}
	}
}

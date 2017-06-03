using System;
using System.Threading.Tasks;
using System.Reflection;

namespace keep.grass
{
    public abstract class ImageSource
    {
        public abstract Task<byte[]> Get();
        public static ImageSource FromResource(Assembly Assembly, string Path)
        {
            return new ResourceImageSource(Assembly, Path);
        }
        public static ImageSource FromUri(string Url)
        {
            return new WebImageSource(Url);
        }
    }
    public class ResourceImageSource : ImageSource
    {
        Assembly Assembly;
        string Path;
        public ResourceImageSource(Assembly a_Assembly, string a_Path)
        {
            Assembly = a_Assembly;
            Path = a_Path;
        }
        public override async Task<byte[]> Get()
        {
            return await AlphaResouceProxy.Get(Assembly, Path);
        }
    }
    public class WebImageSource : ImageSource
    {
        string Url;
        public WebImageSource(string a_Url)
        {
            Url = a_Url;
        }
        public override async Task<byte[]> Get()
        {
            return await AlphaImageProxy.Get(Url);
        }
    }
}

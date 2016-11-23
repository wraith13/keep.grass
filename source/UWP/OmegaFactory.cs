using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using keep.grass.Languages;
using Xamarin.Forms;
using System.Diagnostics;

namespace keep.grass.UWP
{
    public class OmegaFactory : AlphaFactory
    {
        public static void MakeSureInit()
        {
            if (null == AlphaFactory.Get())
            {
                AlphaFactory.Init(new OmegaFactory());
            }
        }

        public override AlphaDomain MakeOmegaDomain()
        {
            return new OmegaDomain();
        }
        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
        }
        public override AlphaLanguage MakeOmegaLanguage()
        {
            return new OmegaLanguage();
        }
        public override AlphaActivityIndicatorTextCell MakeOmegaActivityIndicatorTextCell()
        {
            return new OmegaActivityIndicatorTextCell();
        }
        public override AlphaCircleImageCell MakeOmegaCircleImageCell()
        {
            return new OmegaCircleImageCell();
        }
        //  Xam.Plugins.Forms.ImageCircle が原因でまた異常終了するようになったらここのコードを有効にすること
        //public override Image MakeOmegaCircleImage()
        //{
        //    return new Image();
        //}
        public override VoidEntryCell MakeOmegaEntryCell()
        {
            return new OmegaEntryCell();
        }
        public override VoidSwitchCell MakeOmegaSwitchCell()
        {
            return new OmegaSwitchCell();
        }
        public override AlphaCircleGraph MakeOmegaCircleGraph()
        {
            return new OmegaCircleGraph();
        }
        public override async Task<ImageSource> MakeOmegaImageSourceFromUrl(string Url)
        {
            //	本来、override 元のコードで正常に動作しなければならないが、 UWP版を Windows Phone で
            //	動作させた場合に画像が表示されない為、こちらのコードで問題を回避している。
            //	なお、同じ UWP 版でも PC の Windows 10 であれば下のコードで正常に動作する。
            try
            {
                var Binary = await MakeSureDomain().HttpClient.GetByteArrayAsync(Url);
                return ImageSource.FromStream(() => new System.IO.MemoryStream(Binary));
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
                return null;
            }
        }
    }
}

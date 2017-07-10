using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using keep.grass.App;

namespace keep.grass.UWP
{
    public class OmegaAppFactory : AlphaAppFactory
    {
        public static void MakeSureInit()
        {
            if (null == AlphaAppFactory.Get())
            {
                AlphaAppFactory.Init(new OmegaAppFactory());
            }
        }

        public override AlphaApp MakeOmegaApp()
        {
            return new OmegaApp();
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
        public override Type GetOmegaGitHubUserCellType()
        {
            return typeof(OmegaCircleImageCell);
        }
    }
}

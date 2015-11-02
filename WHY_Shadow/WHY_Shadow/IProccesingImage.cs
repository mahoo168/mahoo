using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHY_Shadow
{
    interface IProccesingImage
    {
        String GetShadowName();

        void SetOriginalImage(ShadowPackage srcPacage);

        void Processing();

        ShadowPackage GetProccesedImage();

        void CloseForm();
        void ShowForm();
    }
}

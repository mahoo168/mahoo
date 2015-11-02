using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHY_Shadow
{
    interface ICamera
    {

        ShadowPackage GetImage();

        void OnClose();

    }
}

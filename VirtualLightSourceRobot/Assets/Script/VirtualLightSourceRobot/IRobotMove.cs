using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;

namespace Robot
{
    public interface IRobotMove
    {
        Vector3 UpDate(Vector3 robotPos, List<Vector3> list_pos);

    }
}

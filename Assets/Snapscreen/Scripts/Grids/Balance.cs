using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    [System.Serializable, CreateAssetMenu(fileName = "Balance", menuName = "NorthLab/Snapscreen/Grids/Balance")]
    public class Balance : CompositionGrid
    {

        public override void DrawGUI(int left, int right, int top, int bottom, Color color)
        {
            int centerX = (left + right) / 2;
            int centerY = (top + bottom) / 2;

            DrawLine(new Vector2(centerX, top), new Vector2(centerX, bottom), 3, color);
            DrawLine(new Vector2(left, centerY), new Vector2(right, centerY), 3, color);
        }

    }
}
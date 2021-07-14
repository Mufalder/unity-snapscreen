using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    [System.Serializable, CreateAssetMenu(fileName = "Thirds", menuName = "NorthLab/Snapscreen/Grids/Thirds")]
    public class Thirds : CompositionGrid
    {

        public override void DrawGUI(int left, int right, int top, int bottom, Color color)
        {
            int thirdX = (right - left) / 3;
            int thirdY = (bottom - top) / 3;

            DrawLine(new Vector2(left + thirdX, top), new Vector2(left + thirdX, bottom), 3, color);
            DrawLine(new Vector2(left + thirdX + thirdX, top), new Vector2(left + thirdX + thirdX, bottom), 3, color);

            DrawLine(new Vector2(left, top + thirdY), new Vector2(right, top + thirdY), 3, color);
            DrawLine(new Vector2(left, top + thirdY + thirdY), new Vector2(right, top + thirdY + thirdY), 3, color);
        }

    }
}
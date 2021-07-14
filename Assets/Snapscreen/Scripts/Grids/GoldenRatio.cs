using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    [System.Serializable, CreateAssetMenu(fileName = "GoldenRatio", menuName = "NorthLab/Snapscreen/Grids/Golden Ratio")]
    public class GoldenRatio : CompositionGrid
    {

        public override void DrawGUI(int left, int right, int top, int bottom, Color color)
        {
            int width = right - left;
            int height = bottom - top;
            int x = Mathf.RoundToInt(width * 0.618f);
            int y = Mathf.RoundToInt(height * 0.618f);

            DrawLine(new Vector2(left + x, top), new Vector2(left + x, bottom), 3, color);
            DrawLine(new Vector2(right - x, top), new Vector2(right - x, bottom), 3, color);

            DrawLine(new Vector2(left, top + y), new Vector2(right, top + y), 3, color);
            DrawLine(new Vector2(left, bottom - y), new Vector2(right, bottom - y), 3, color);
        }

    }
}
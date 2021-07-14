using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    [System.Serializable, CreateAssetMenu(fileName = "Grid", menuName = "NorthLab/Snapscreen/Grids/Grid")]
    public class GridTiles : CompositionGrid
    {

        public override void DrawGUI(int left, int right, int top, int bottom, Color color)
        {
            int width = right - left;
            int height = bottom - top;
            int x = width / 4;
            int y = height / 4;

            DrawLine(new Vector2(left + x, top), new Vector2(left + x, bottom), 3, color);
            DrawLine(new Vector2(left + x + x, top), new Vector2(left + x + x, bottom), 3, color);
            DrawLine(new Vector2(left + x + x + x, top), new Vector2(left + x + x + x, bottom), 3, color);

            DrawLine(new Vector2(left, top + y), new Vector2(right, top + y), 3, color);
            DrawLine(new Vector2(left, top + y + y), new Vector2(right, top + y + y), 3, color);
            DrawLine(new Vector2(left, top + y + y + y), new Vector2(right, top + y + y + y), 3, color);
        }

    }

}
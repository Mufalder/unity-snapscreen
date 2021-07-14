using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    [System.Serializable, CreateAssetMenu(fileName = "KubrickPerspective", menuName = "NorthLab/Snapscreen/Grids/Kubrick's Perspective")]
    public class KubrickPerspective : CompositionGrid
    {

        public override void DrawGUI(int left, int right, int top, int bottom, Color color)
        {
            int width = right - left;
            int height = bottom - top;
            int centerX = (left + right) / 2;
            int centerY = (top + bottom) / 2;
            int rect1_x = Mathf.RoundToInt(width * 0.113f);
            int rect1_y = Mathf.RoundToInt(height * 0.113f);
            int rect2_x = Mathf.RoundToInt(width * 0.282f);
            int rect2_y = Mathf.RoundToInt(height * 0.282f);

            DrawLine(new Vector2(left, top), new Vector2(right, bottom), 3, color);
            DrawLine(new Vector2(left, bottom), new Vector2(right, top), 3, color);
            DrawLine(new Vector2(centerX, top), new Vector2(centerX, bottom), 3, color);
            DrawLine(new Vector2(left, centerY), new Vector2(right, centerY), 3, color);

            DrawLine(new Vector2(left + rect1_x, top + rect1_y), new Vector2(right - rect1_x, top + rect1_y), 3, color);
            DrawLine(new Vector2(right - rect1_x, top + rect1_y), new Vector2(right - rect1_x, bottom - rect1_y), 3, color);
            DrawLine(new Vector2(right - rect1_x, bottom - rect1_y), new Vector2(left + rect1_x, bottom - rect1_y), 3, color);
            DrawLine(new Vector2(left + rect1_x, bottom - rect1_y), new Vector2(left + rect1_x, top + rect1_y), 3, color);

            DrawLine(new Vector2(left + rect2_x, top + rect2_y), new Vector2(right - rect2_x, top + rect2_y), 3, color);
            DrawLine(new Vector2(right - rect2_x, top + rect2_y), new Vector2(right - rect2_x, bottom - rect2_y), 3, color);
            DrawLine(new Vector2(right - rect2_x, bottom - rect2_y), new Vector2(left + rect2_x, bottom - rect2_y), 3, color);
            DrawLine(new Vector2(left + rect2_x, bottom - rect2_y), new Vector2(left + rect2_x, top + rect2_y), 3, color);
        }

    }
}

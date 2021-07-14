using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NorthLab
{

    /// <summary>
    /// Base class for composition grid objects
    /// </summary>
    [System.Serializable]
    public abstract class CompositionGrid : ScriptableObject
    {

        /// <summary>
        /// Abstract method for the composition grid drawing.
        /// </summary>
        /// <param name="left">Left coordinate of the screen</param>
        /// <param name="right">Right coordinate of the screen</param>
        /// <param name="top">Top coordinate of the screen</param>
        /// <param name="bottom">Bottom coordinate of the screen</param>
        /// <param name="color">Color of the grid</param>
        public abstract void DrawGUI(int left, int right, int top, int bottom, Color color);

        private static Texture2D lineTex;

        /// <summary>
        /// Draw a line.
        /// </summary>
        /// <param name="pointA">Start position of the line</param>
        /// <param name="pointB">End position of the line</param>
        /// <param name="width">Width of the line</param>
        /// <param name="color">Color of the line</param>
        public static void DrawLine(Vector2 pointA, Vector2 pointB, float width, Color color)
        {
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Matrix4x4 saveMatrix = GUI.matrix;
            Color saveColor = GUI.color;

            GUI.color = color;
            Vector2 delta = pointB - pointA;
            GUIUtility.ScaleAroundPivot(new Vector2(delta.magnitude, width), Vector2.zero);
            GUIUtility.RotateAroundPivot(Vector2.Angle(delta, Vector2.right) * Mathf.Sign(delta.y), Vector2.zero);
            GUI.matrix = Matrix4x4.TRS(pointA, Quaternion.identity, Vector3.one) * GUI.matrix;

            GUI.DrawTexture(new Rect(Vector2.zero, Vector2.one), lineTex);

            GUI.matrix = saveMatrix;
            GUI.color = saveColor;
        }

    }

}
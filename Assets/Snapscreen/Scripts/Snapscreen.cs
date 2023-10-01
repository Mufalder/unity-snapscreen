using System.IO;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NorthLab
{
    [ExecuteInEditMode]
    public class Snapscreen : MonoBehaviour
    {

        public enum CameraMode { MainCamera, SpecificCamera };
        public enum GUIPositions {LeftTop, RightTop, RightBottom, LeftBottom, Explicit };

        [SerializeField, Tooltip("Screenshot file name"), Header("Main")]
        new private string name = "Screenshot";
        [SerializeField, Tooltip("Saves the captured image with alpha channel. This option will clear the background")]
        private bool transparentBackground = false;
        [SerializeField, Tooltip("Use MainCamera to take a screenshot or use another specific camera")]
        private CameraMode cameraMode = CameraMode.MainCamera;
        [SerializeField]
        private Camera specificCamera = null;
        [SerializeField, Tooltip("Path to save a screenshot file")]
        private string path;
        [SerializeField, Header("Input")]
        private bool useInput = true;
        [SerializeField, Tooltip("Button name to take a screenshot from the Input settings")]
        private string button = "Submit";
        [SerializeField, Tooltip("Custom key to take a screenshot")]
        private KeyCode key = KeyCode.None;
        [SerializeField, Tooltip("Take screenshot when pressing PrintScreen key")]
        private bool onPrintScreen = false;
        [SerializeField, Tooltip("Put date stamp on a screenshot file name")]
        private bool dateStamp = true;
        [SerializeField, Tooltip("Show GUI. May cause delay while taking a screenshot"), Header("UI")]
        private bool useGUI = false;
        [SerializeField, Tooltip("GUI position")]
        private GUIPositions guiPosition = GUIPositions.LeftTop;
        [SerializeField, Tooltip("Explicit GUI position")]
        private Vector2 position = Vector2.zero;
        [SerializeField, Header("Crop"), Tooltip("Crop the screenshot?")]
        private bool crop = false;
        [SerializeField, Tooltip("Crop amount from the left")]
        private int left = 0;
        [SerializeField, Tooltip("Crop amount from the right")]
        private int right = 0;
        [SerializeField, Tooltip("Crop amount from the top")]
        private int top = 0;
        [SerializeField, Tooltip("Crop amount from the bottom")]
        private int bottom = 0;
        [SerializeField, Header("Composition grid"), Tooltip("Show composition grid")]
        private bool compositionGrid = false;
        [SerializeField, Tooltip("Select grid to draw")]
        private CompositionGrid grid = null;
        [SerializeField]
        private Color gridColor = Color.white;

        private int windWidth, windHeight;

        public delegate void OnScreenshotTaken(string name, Texture2D screenshot);
        public static OnScreenshotTaken onScreenshotTaken;

        //On component reset
        private void Reset()
        {
            name = "Screenshot";
            transparentBackground = false;
            cameraMode = CameraMode.MainCamera;
            specificCamera = null;
            path = Application.dataPath;
            button = "Submit";
            key = KeyCode.None;
            dateStamp = true;
            useGUI = false;
            crop = false;
            compositionGrid = false;
            grid = null;
            gridColor = Color.white;
        }

        //Validate crop numbers
        private void OnValidate()
        {
            Vector2 resolution = GetMainGameViewSize();
            int width = (int)resolution.x - 10;
            int height = (int)resolution.y - 10;

            if (left < 0)
                left = 0;
            if (left > width / 2)
                left = width / 2;

            if (right < 0)
                right = 0;
            if (right > width / 2)
                right = width / 2;

            if (top < 0)
                top = 0;
            if (top > height / 2)
                top = height / 2;

            if (bottom < 0)
                bottom = 0;
            if (bottom > height / 2)
                bottom = height / 2;
        }

        //Returns game view resolution even if it's not focused
        private Vector2Int GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            Vector2 resVector = (Vector2)Res;
            return new Vector2Int((int)resVector.x, (int)resVector.y);
        }

        private void Update()
        {
#if UNITY_EDITOR
            //Do not execute if game is not playing or use input off
            if (!Application.isPlaying || !useInput)
                return;

            //Take screenshot by pressing button or key
            if ((!string.IsNullOrEmpty(button) && Input.GetButtonUp(button)) || Input.GetKeyUp(key))
            {
                Take();
            }
#endif
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            //Check if printscreen is pressed
            if (onPrintScreen && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.SysReq)
            {
                TakeScreenshot();
            }

            //Load white texture for a further drawing
            Texture2D rect = Resources.Load<Texture2D>("white");

            //Draw crop zone
            if (crop)
            {
                if (rect)
                {
                    GUI.color = new Color(0, 0, 0, 0.5f);
                    GUI.DrawTexture(new Rect(0, 0, left, Screen.height), rect);
                    GUI.DrawTexture(new Rect(Screen.width, 0, -right, Screen.height), rect);
                    GUI.DrawTexture(new Rect(left, 0, Screen.width - left - right, top), rect);
                    GUI.DrawTexture(new Rect(left, Screen.height, Screen.width - left - right, -bottom), rect);
                }
                else
                {
                    Debug.LogWarning("Cannot find the 'Resources/white.png' file.");
                }
            }

            //Draw composition grid
            if (compositionGrid && grid != null)
            {
                if (crop)
                {
                    grid.DrawGUI(left, Screen.width - right, top, Screen.height - bottom, gridColor);
                }
                else
                {
                    grid.DrawGUI(0, Screen.width, 0, Screen.height, gridColor);
                }
            }

            //Do not execute if gui is disabled
            if (!useGUI)
                return;

            //Do not execute if game is not playing
            if (!Application.isPlaying)
                return;

            //Draw window
            windWidth = Screen.width / 5;
            windHeight = windWidth / 2;
            Rect windRect = new Rect(0, 0, windWidth, windHeight);
            switch (guiPosition)
            {
                case GUIPositions.RightTop:
                    windRect = new Rect(Screen.width - windWidth, 0, windWidth, windHeight);
                    break;

                case GUIPositions.RightBottom:
                    windRect = new Rect(Screen.width - windWidth, Screen.height - windHeight, windWidth, windHeight);
                    break;

                case GUIPositions.LeftBottom:
                    windRect = new Rect(0, Screen.height - windHeight, windWidth, windHeight);
                    break;

                case GUIPositions.Explicit:
                    windRect = new Rect(position.x, position.y, windWidth, windHeight);
                    break;

                default:
                    windRect = new Rect(0, 0, windWidth, windHeight);
                    break;
            }
            GUI.Window(0, windRect, Window, "Snapscreen");
#endif
        }

        //Window
        private void Window(int windowID)
        {
            //Draw "Take" button
            int width = windWidth - 60;
            if (GUI.Button(new Rect(30, 41, windWidth - 60, width * 0.25f), "SNAP"))
            {
                TakeScreenshot();
            }
        }

        //Image cropping
        private Texture2D CropImage(Texture2D capture)
        {
            Texture2D cropped = new Texture2D(capture.width - left - right, capture.height - top - bottom);
            cropped.SetPixels(capture.GetPixels(left, top, capture.width - left - right, capture.height - top - bottom));
            return cropped;
        }

        //Save texture2D to the disk
        private void SaveImage(Texture2D capture, string path)
        {
            byte[] bytes = ImageConversion.EncodeToPNG(capture);
            File.WriteAllBytes(path, bytes);
        }

        //Get path of the screenshot file
        private string GetPath()
        {
            string additional;
            if (dateStamp)
                additional = System.DateTime.Now.ToString(" yyyy-MM-dd H-mm-ss.f") + ".png";
            else additional = ".png";
            return path + "/" + name + additional;
        }

        //Screenshot method
        private void Take()
        {
            //Check target camera
            Camera cam = cameraMode == CameraMode.MainCamera ? Camera.main : specificCamera;

            if (cam == null)
            {
                Debug.LogError(cameraMode == CameraMode.MainCamera ? "No MainCamera is found!" : "Specific camera is null");
                return;
            }

            Vector2Int resolution = GetMainGameViewSize();

            //Grabbing the screenshot on to the rendertexture
            RenderTexture oldTargetTexture = cam.targetTexture;
            CameraClearFlags oldFlags = cam.clearFlags;
            RenderTexture oldActive = RenderTexture.active;

            Texture2D tex_transparent;
            if (crop)
            {
                tex_transparent = new Texture2D(resolution.x - left - right, resolution.y - top - bottom, TextureFormat.ARGB32, false);
            }
            else
            {
                tex_transparent = new Texture2D(resolution.x, resolution.y, TextureFormat.ARGB32, false);
            }
            RenderTexture render_texture = RenderTexture.GetTemporary(resolution.x, resolution.y, 24, RenderTextureFormat.ARGB32);

            Rect grab_area = new Rect(0, 0, resolution.x, resolution.y);
            if (crop)
            {
                grab_area.x = left;
                grab_area.y = top;
                grab_area.width -= left + right;
                grab_area.height -= top + bottom;
            }

            RenderTexture.active = render_texture;
            cam.targetTexture = render_texture;

            if (transparentBackground)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Color.clear;
            }

            cam.Render();
            tex_transparent.ReadPixels(grab_area, 0, 0);
            tex_transparent.Apply();

            //Save image
            SaveImage(tex_transparent, GetPath());

            //Clearing and restoring camera parameters
            cam.clearFlags = oldFlags;
            cam.targetTexture = oldTargetTexture;
            RenderTexture.active = oldActive;
            RenderTexture.ReleaseTemporary(render_texture);

            if (Application.isPlaying)
                Destroy(tex_transparent);
            else DestroyImmediate(tex_transparent);
        }

        /// <summary>
        /// Take the screenshot
        /// </summary>
        public void TakeScreenshot()
        {
            Take();
        }

#if UNITY_EDITOR
        #region Menu items
        [MenuItem("Tools/Snapscreen/Add Snapscreen To The Main Camera")]
        private static void AddSnapscreenToMainCamera()
        {
            GameObject go = GameObject.FindWithTag("MainCamera");
            if (go)
            {
                if (go.GetComponent<Snapscreen>())
                {
                    Debug.LogWarning("Main camera already have a Snapscreen component");
                    return;
                }

                go.AddComponent<Snapscreen>();
            }
            else
            {
                Debug.LogWarning("There is no main camera in the scene. Add the 'MainCamera' tag to your camera");
            }
        }
        #endregion
#endif
    }
}
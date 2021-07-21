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

        public enum GUIPositions {LeftTop, RightTop, RightBottom, LeftBottom, Explicit };

        [SerializeField, Tooltip("Screenshot file name"), Header("Main")]
        new private string name = "Screenshot";
        [SerializeField, Tooltip("Path to save a screenshot file")]
        private string path;
        [SerializeField, Tooltip("Resolution scale multiplier. E.g.: 1024x1024 with multiplier of 2 will give 2048x2048 screenshot")]
        private int factor = 1;
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
        [SerializeField, Tooltip("Time before GUI unhides after a screenshot taken. It's Time.deltaTime multiplier. Use bigger values if the GUI still present in the screenshots.")]
        private float unhideTime = 6;
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

        private bool hideGUI;

        public delegate void OnScreenshotTaken(string name, Texture2D screenshot);
        public static OnScreenshotTaken onScreenshotTaken;

        //On component reset
        private void Reset()
        {
            name = "Screenshot";
            path = Application.dataPath;
            factor = 1;
            button = "Submit";
            key = KeyCode.None;
            dateStamp = true;
            useGUI = false;
            unhideTime = 4;
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
        private Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
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
                TakeScreenshot();
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

            //Do not execute if GUI is hidden
            if (hideGUI)
                return;

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
            Rect windRect = new Rect(0, 0, 256, 128);
            switch (guiPosition)
            {
                case GUIPositions.RightTop:
                    windRect = new Rect(Screen.width - 256, 0, 256, 128);
                    break;

                case GUIPositions.RightBottom:
                    windRect = new Rect(Screen.width - 256, Screen.height - 128, 256, 128);
                    break;

                case GUIPositions.LeftBottom:
                    windRect = new Rect(0, Screen.height - 128, 256, 128);
                    break;

                case GUIPositions.Explicit:
                    windRect = new Rect(position.x, position.y, 256, 128);
                    break;

                default:
                    windRect = new Rect(0, 0, 256, 128);
                    break;
            }
            GUI.Window(0, windRect, Window, "Snapscreen");
#endif
        }

        //Window
        private void Window(int windowID)
        {
            //Draw "Take" button
            if (GUI.Button(new Rect(30, 41, 196, 46), "Take"))
            {
                TakeScreenshot();
            }
        }

        //Unhide gui
        private void UnhideGUI()
        {
            hideGUI = false;
        }

        //Waiting for end of frame and then capture a screenshot as texture to crop it further
        private IEnumerator TakeCoroutine(string path)
        {
            yield return new WaitForEndOfFrame();
            Texture2D capture = ScreenCapture.CaptureScreenshotAsTexture(factor);
            if (EditorApplication.isPlaying && crop)
            {
                Texture2D cropped = CropImage(capture);
                SaveImage(cropped, path);
                onScreenshotTaken?.Invoke(path, cropped);
            }
            else
            {
                SaveImage(capture, path);
                onScreenshotTaken?.Invoke(path, capture);
            }

            //if GUI is hidden then unhide it
            if (hideGUI)
            {
                yield return new WaitForSeconds(Time.deltaTime * unhideTime);
                UnhideGUI();
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
            //Convert RGBA32 captured texture to the RBG24 to disable alpha channel
            Texture2D converted = new Texture2D(capture.width, capture.height, TextureFormat.RGB24, false);
            converted.SetPixels(capture.GetPixels());

            byte[] bytes = converted.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }

        private void Take()
        {
#if UNITY_EDITOR
            //If game is not playing then open game window
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.ExecuteMenuItem("Window/General/Game");
            }

            //Combining file path and name
            string additional;
            if (dateStamp)
                additional = System.DateTime.Now.ToString(" yyyy-MM-dd H-mm-ss.f") + ".png";
            else additional = ".png";
            string finalPath = path + "/" + name + additional;

            StartCoroutine(TakeCoroutine(finalPath));
#endif
        }

        public void TakeScreenshot()
        {
            //If Unity in the play mode with GUI enabled then hide the gui and take a screenshot after few frames
            if (Application.isPlaying && (useGUI || crop || compositionGrid))
            {
                hideGUI = true;
                Invoke("Take", Time.deltaTime * 2);
            }
            else //else just take it immediately
            {
                Take();
            }
        }

#if UNITY_EDITOR
        #region Menu items
        [MenuItem("NorthLab/Snapscreen/Add Snapscreen To The Main Camera")]
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

        [MenuItem("NorthLab/Snapscreen/Create Empty Snapscreen Object")]
        private static void CreateEmptySnapscreenObject()
        {
            GameObject go = new GameObject("Snapscreen");
            go.AddComponent<Snapscreen>();
        }
        #endregion
#endif
    }
}
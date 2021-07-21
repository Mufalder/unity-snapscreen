v1.3.2.1

Place "Snapscreen" on empty GameObject or any other GameObject. Select approtiate resolution in Game view window of the Unity. Configure the script according to this description:

Name - file name of your screenshot.
Path - directory to save your screenshot.
Date stamp - place date stamp in the name of your screenshot file.
Factor - resolution multiplier. For example, if you set 1024*1024 resolution with factor 2 you would get 2048*2048 screenshot.

Button - input button name from Input settings to take screenshot.
Key - custom key to take screenshot.
On Print Screen - takes screenshot when Print Screen key is pressed.

Use GUI - use GUI to take screenshots inside of Play mode. WARNING: Snapping with GUI requires delays, not good if you want to take instant screenshot.
Unhide time - time before GUI unhides after a screenshot taken. It's Time.deltaTime multiplier. Use bigger values if the GUI still present in the screenshots.
GUI Position - GUI position on the screen.
Position - explicit GUI position on the screen.

Crop - Crop the screenshot? WARNING: Works only inside the Play mode!
Left, right, top, bottom - crop amounts.

Composition grid - Show composition grid
Grid - select grid to draw
Grid color - color of the composition grid

Take button - use it to take screenshots outside of Play mode.

Enjoy!
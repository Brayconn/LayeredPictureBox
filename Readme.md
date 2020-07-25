# LayeredPictureBox
It's like a PictureBox, but it supports multiple layers of images, and doesn't die when you try to update it in realtime.

## Usage
Before doing anything add the layers you want, either using `AddLayer(Image)`/`AddLayer(Image,Point)`/`AddLayer(Layer)` to add existing images, or `AddLayers(int)` to add dummy layers that you can fill in later.

If you're using the control as an editor with a visible mouse, or just something where images will probably be going "off screen", you'll want to call `LockCanvasSize()` to stop the control from resizing all over the place.
Alternatively, you can lock it yourself by using the `CanvasSizeLocked` property, and the `CurrentCanvasWidth`/`CurrentCanvasHeight` properties.

If you want scrollbars, just put this control inside a panel that has `AutoScroll` set to `true`.

The `CanvasSize` represents how big an area you have set aside to display things. It will always return the "real value" in pixels, not the scaled size according to `CanvasScale`.

Changing the `CanvasScale` will always update the `Width` and `Height` of the control, regardless of other settings. `AutoSize`/`AutoSizeMode` does control all other size changes however.
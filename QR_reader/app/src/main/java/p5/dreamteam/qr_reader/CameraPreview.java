package p5.dreamteam.qr_reader;

import android.content.Context;
import android.hardware.Camera;
import android.util.AttributeSet;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import java.io.IOException;

/**
 * Scans an image. Displays on screen. Sends scanned images to {@link ZBarScannerActivity} to scan with the callback.
 */
class CameraPreview extends SurfaceView implements SurfaceHolder.Callback {
    /**
     * Tag for logging
     */
    private static final String TAG = "CameraPreview";
    /**
     * The active camera instance
     */
    private Camera _camera;
    /**
     * Delivers copies of preview frames as they are displayed
     */
    private Camera.PreviewCallback _previewCallback;
    /**
     * Should flash be enabled when previewing?
     */
    private boolean _flash;

    /**
     * "Real" constructor. Android Studio asked for the others, since the layout editor
     * (and possibly the emulator) needs them.
     * @param context The activity from which the preview is called. Usually {@link ZBarScannerActivity}.
     * @param previewCallback Used to deliver preview frames to ZBar in addition to actually displaying on screen.
     * @param flash Should flash be used? {@link MainActivity#_swcFlash}
     */
    public CameraPreview(Context context, Camera.PreviewCallback previewCallback, boolean flash) {
        super(context);

        this._previewCallback = previewCallback;
        this._flash = flash;

        SurfaceHolder holder = getHolder();
        holder.addCallback(this);
    }

    /**
     * {See {@link CameraPreview#CameraPreview(Context, Camera.PreviewCallback, boolean)}}
     */
    public CameraPreview(Context context) {
        super(context);
    }

    /**
     * {See {@link CameraPreview#CameraPreview(Context, Camera.PreviewCallback, boolean)}}
     */
    public CameraPreview(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    /**
     * {See {@link CameraPreview#CameraPreview(Context, Camera.PreviewCallback, boolean)}}
     */
    public CameraPreview(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public void setCamera(Camera camera) {
        _camera = camera;
        if (_camera != null) {
            requestLayout();
        }
    }

    /**
     * Called by Android immediately on surface creation.
     * Attempt to set camera preview to active SurfacePreview
     * @param holder Contains the Surface on which to place preview
     */
    public void surfaceCreated(SurfaceHolder holder) {
        if (_camera != null) {
            try {
                _camera.setPreviewDisplay(holder);
            } catch (IOException e) {
                Log.d(TAG, e.getMessage());
            }
        }
    }

    /**
     * Called by Android after any structural changes to the surface (including startup)
     * Sets Resolution, autofocus, flash, layout, orientation, preview callback, and starts preview.
     * @param holder The holder whose surface has changed
     * @param format Not relevant
     * @param width Not relevant
     * @param height Not relevant
     */
    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
        if (_camera != null) {
            Camera.Parameters parameters = _camera.getParameters();
            parameters.setPreviewSize(1280, 720);
            parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            if (_flash) {
                parameters.setFlashMode(Camera.Parameters.FLASH_MODE_TORCH);
            }
            requestLayout();

            _camera.setDisplayOrientation(90);
            _camera.setParameters(parameters);
            _camera.setPreviewCallback(_previewCallback);
            _camera.startPreview();
        }
    }

    /**
     * Called by Android before surface is being destroyed.
     * Disables camera, and stops preview
     * @param holder The holder whose surface is destroyed
     */
    public void surfaceDestroyed(SurfaceHolder holder) {
        if (_camera != null) {
            _camera.cancelAutoFocus();
            _camera.stopPreview();
            _camera.release();
            _camera = null;
        }
    }
}
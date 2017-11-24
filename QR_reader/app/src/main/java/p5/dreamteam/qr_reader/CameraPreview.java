package p5.dreamteam.qr_reader;

import android.content.Context;
import android.hardware.Camera;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import java.io.IOException;
import java.util.List;

/**
 * TODO: Add a class header comment!
 */
class CameraPreview extends SurfaceView implements SurfaceHolder.Callback {
    private static final String TAG = "CameraPreview";

    private Camera _camera;
    private Camera.PreviewCallback _previewCallback;
    private boolean _flash;

    public CameraPreview(Context context, Camera.PreviewCallback previewCallback, boolean flash) {
        super(context);

        this._previewCallback = previewCallback;
        this._flash = flash;

        SurfaceHolder holder = getHolder();
        holder.addCallback(this);
    }

    public void setCamera(Camera camera) {
        _camera = camera;
        if (_camera != null) {
            requestLayout();
        }
    }

    public void surfaceCreated(SurfaceHolder holder) {
        if (_camera != null) {
            try {
                _camera.setPreviewDisplay(holder);
            } catch (IOException e) {
                Log.d(TAG, e.getMessage());
            }
        }
    }

    public void surfaceChanged(SurfaceHolder holder, int format, int width, int height) {
        if (_camera != null) {
            Camera.Parameters parameters = _camera.getParameters();
//            List<Camera.Size> previewSizes = parameters.getSupportedPreviewSizes();
//            for (Camera.Size size : previewSizes) {
//                Log.i(TAG, "Height: " + size.height + "     Width: " + size.width);
//            }
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

    public void surfaceDestroyed(SurfaceHolder holder) {
        if (_camera != null) {
            _camera.cancelAutoFocus();
            _camera.stopPreview();
            _camera.release();
            _camera = null;
        }
    }
}
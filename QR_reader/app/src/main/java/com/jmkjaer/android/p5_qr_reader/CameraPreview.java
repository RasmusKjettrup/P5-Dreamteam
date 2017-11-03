package com.jmkjaer.android.p5_qr_reader;

import android.app.Activity;
import android.content.Context;
import android.hardware.Camera;
import android.hardware.Camera.Size;
import android.util.Log;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;

import java.io.IOException;
import java.util.List;

/**
 * TODO: Add a class header comment!
 */
class CameraPreview extends SurfaceView implements SurfaceHolder.Callback {
    private static final String TAG = "CameraPreview";

    private Camera _camera;
    private int _orientation;
    private Camera.PreviewCallback _previewCallback;
    private Camera.AutoFocusCallback _autofocusCallback;

    public CameraPreview(Context context, Camera.PreviewCallback previewCallback, Camera
            .AutoFocusCallback autoFocusCallback) {
        super(context);

        this._previewCallback = previewCallback;
        this._autofocusCallback = autoFocusCallback;

        SurfaceHolder holder = getHolder();
        holder.addCallback(this);
        holder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
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
            List<Camera.Size> previewSizes = parameters.getSupportedPreviewSizes();
            for (Camera.Size size : previewSizes) {
                Log.i(TAG, "Height: " + size.height + "     Width: " + size.width);
            }

            Camera.Size previewSize = previewSizes.get(0); // 1280 x 720 on Huawei Y530
            parameters.setPreviewSize(previewSize.width, previewSize.height);
            parameters.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            requestLayout();

            _camera.setDisplayOrientation(90);
            _camera.setParameters(parameters);
            _camera.setPreviewCallback(_previewCallback);
            _camera.startPreview();
            _camera.autoFocus(_autofocusCallback);
        }
    }

    public void surfaceDestroyed(SurfaceHolder holder) {
        if (_camera != null) {
            _camera.cancelAutoFocus();
            _camera.stopPreview();
            _camera = null;
        }
    }
}
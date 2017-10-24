package com.jmkjaer.android.p5_qr_reader;

import android.content.Context;
import android.hardware.Camera;
import android.util.Log;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

/**
 * TODO: Add a class header comment!
 */
public class CameraPreview extends SurfaceView implements SurfaceHolder.Callback {
    private static final String TAG = "CameraPreview";
    private SurfaceHolder mHolder;
    private Camera mCamera;

    public CameraPreview(Context context, Camera mCamera) {
        super(context);
        this.mCamera = mCamera;
        mCamera.setDisplayOrientation(90);

        mHolder = getHolder();
        mHolder.addCallback(this);
    }


    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        try {
            mCamera.setPreviewDisplay(holder);
        } catch (Exception e) {
            Log.d(TAG, "Error setting camera preview: " + e.getMessage());
        }
    }

    /**
     * Release camera so that other apps can use it
     * @param holder
     */
    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        mCamera.stopPreview();
        mCamera.release();
        mCamera = null;
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int w, int h) {
        if (mHolder.getSurface() == null) {
            return;
        }

        try {
            mCamera.stopPreview();
        } catch (Exception e) {
        }

        try {
            mCamera.setPreviewDisplay(mHolder);
            mCamera.startPreview();
        } catch (Exception e) {
            Log.d(TAG, "Error starting camera preview: " + e.getMessage());
        }
    }
}
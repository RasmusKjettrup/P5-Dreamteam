package com.jmkjaer.android.p5_qr_reader;

import android.hardware.Camera;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.widget.FrameLayout;

public class MainActivity extends AppCompatActivity {
    Camera mCamera;
    CameraPreview mPreview;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mCamera = getCameraInstance();
        Camera.Parameters params = mCamera.getParameters();
        params.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
        mCamera.setParameters(params);
        mPreview = new CameraPreview(this, mCamera);
        FrameLayout preview = (FrameLayout)findViewById(R.id.camera_preview);
        preview.addView(mPreview);



    }

//    private boolean checkCameraHardware() {
//        if (this.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA)) {
//            Toast toast = Toast.makeText(this, "Device has camera!", Toast.LENGTH_LONG);
//            toast.show();
//            return true;
//        } else {
//            Toast toast = Toast.makeText(this, "Device does not have camera!", Toast.LENGTH_LONG);
//            toast.show();
//            return false;
//        }
//    }

    private static Camera getCameraInstance() {
        Camera c = null;
        try {
            c = Camera.open();
        } catch (Exception e) {
            Log.d("","Camera sucks.");
        }
        return c;
    }
}

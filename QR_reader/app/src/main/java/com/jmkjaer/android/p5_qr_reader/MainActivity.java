package com.jmkjaer.android.p5_qr_reader;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.text.TextUtils;
import android.view.View;
import android.widget.Toast;

import net.sourceforge.zbar.Symbol;


public class MainActivity extends AppCompatActivity {
//    Camera mCamera;
//    CameraPreview mPreview;
//
//    @Override
//    protected void onCreate(Bundle savedInstanceState) {
//        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_main);
//
//        mCamera = getCameraInstance();
//        Camera.Parameters params = mCamera.getParameters();
//        params.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
//        mCamera.setParameters(params);
//        mPreview = new CameraPreview(this, mCamera, 90);
//        FrameLayout preview = (FrameLayout)findViewById(R.id.camera_preview);
//        preview.addView(mPreview);
//    }
//
////    private boolean checkCameraHardware() {
////        if (this.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA)) {
////            Toast toast = Toast.makeText(this, "Device has camera!", Toast.LENGTH_LONG);
////            toast.show();
////            return true;
////        } else {
////            Toast toast = Toast.makeText(this, "Device does not have camera!", Toast.LENGTH_LONG);
////            toast.show();
////            return false;
////        }
////    }
//
//    private static Camera getCameraInstance() {
//        Camera c = null;
//        try {
//            c = Camera.open(0); // back camera
//        } catch (Exception e) {
//            Log.d("","Camera sucks.");
//        }
//        return c;
//    }

    private static final int ZBAR_SCANNER_REQUEST = 0;
    private static final int ZBAR_QR_SCANNER_REQUEST = 1;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    public void launchScanner(View v) {
        if (isCameraAvailable()) {
            Intent intent = new Intent(this, ZBarScannerActivity.class);
            startActivityForResult(intent, ZBAR_SCANNER_REQUEST);
        } else {
            Toast.makeText(this, "Rear Facing Camera Unavailable", Toast.LENGTH_SHORT).show();
        }
    }

    public void launchQRScanner(View v) {
        if (isCameraAvailable()) {
            Intent intent = new Intent(this, ZBarScannerActivity.class);
            intent.putExtra(ZBarConstants.SCAN_MODES, new int[]{Symbol.QRCODE});
            startActivityForResult(intent, ZBAR_SCANNER_REQUEST);
        } else {
            Toast.makeText(this, "Rear Facing Camera Unavailable", Toast.LENGTH_SHORT).show();
        }
    }

    public boolean isCameraAvailable() {
        PackageManager pm = getPackageManager();
        return pm.hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case ZBAR_SCANNER_REQUEST:
            case ZBAR_QR_SCANNER_REQUEST:
                if (resultCode == RESULT_OK) {
                    Toast.makeText(this, "Scan Result = " + data.getStringExtra(ZBarConstants.SCAN_RESULT), Toast.LENGTH_SHORT).show();
                } else if(resultCode == RESULT_CANCELED && data != null) {
                    String error = data.getStringExtra(ZBarConstants.ERROR_INFO);
                    if(!TextUtils.isEmpty(error)) {
                        Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
                    }
                }
                break;
        }
    }
}

package p5.dreamteam.qr_reader;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import net.sourceforge.zbar.Symbol;

import java.util.concurrent.ExecutionException;

public class MainActivity extends AppCompatActivity/* implements InputEventListener, DisconnectEventListener*/ {
    private static final int ZBAR_SCANNER_REQUEST = 0;
    private static final int ZBAR_QR_SCANNER_REQUEST = 1;
    private TextView _txtView;
    private final static String TAG = "MainActivity";

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        _txtView = findViewById(R.id.txt_message);
//        _connectionHandler = new ConnectionHandler(this, this);
//        _connectionHandler.clientSetup();
    }

//    public void disconnectEventInvoke(){
//        // Event occurs whenever the client is disconnected from the server
//    }
//
//    public void inputEventInvoked(String string){
//        // Event occurs whenever the client recieves input from the server
//    }

    public void launchScanner(View v) {
        if (isCameraAvailable()) {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) !=
                    PackageManager.PERMISSION_GRANTED) { // If no permission -> ask, else start
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, 0);
            } else {
                Intent intent = new Intent(this, ZBarScannerActivity.class);
                startActivityForResult(intent, ZBAR_SCANNER_REQUEST);
            }
        } else {
            Toast.makeText(this, "Rear Facing Camera Unavailable", Toast.LENGTH_SHORT).show();
        }
    }

    public void launchQRScanner(View v) {
        if (isCameraAvailable()) {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager
                    .PERMISSION_GRANTED) {
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, 0);
            } else {
                Intent intent = new Intent(this, ZBarScannerActivity.class);
                intent.putExtra(ZBarConstants.SCAN_MODES, new int[]{Symbol.QRCODE});
                startActivityForResult(intent, ZBAR_SCANNER_REQUEST);
            }
        } else {
            Toast.makeText(this, "Rear Facing Camera Unavailable", Toast.LENGTH_SHORT).show();
        }
    }

    public boolean isCameraAvailable() {
        return getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case ZBAR_SCANNER_REQUEST:
            case ZBAR_QR_SCANNER_REQUEST:
                if (resultCode == RESULT_OK) {
                    String result = null;
                    GoogleAPIRequest request = new GoogleAPIRequest();
                    try {
                        result = request.execute(data.getStringExtra(ZBarConstants.SCAN_RESULT)).get();
                    } catch (InterruptedException e) {
                        Log.e(TAG, "Connection to server was interrupted.");
                    } catch (ExecutionException e) {
                        Log.e(TAG, "Could not execute due to connection interrupt.");
                    }
                    _txtView.setText(result);
                } else if (resultCode == RESULT_CANCELED && data != null) {
                    String error = data.getStringExtra(ZBarConstants.ERROR_INFO);
                    if (!TextUtils.isEmpty(error)) {
                        Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(this, data.getStringExtra(ZBarConstants.ERROR_INFO), Toast.LENGTH_LONG).show();
                }
                break;
        }
    }
}

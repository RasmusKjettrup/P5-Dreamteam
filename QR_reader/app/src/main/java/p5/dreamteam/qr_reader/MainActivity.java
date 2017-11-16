package p5.dreamteam.qr_reader;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.text.InputType;
import android.text.TextUtils;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.concurrent.ExecutionException;

public class MainActivity extends AppCompatActivity {

    private static final int ZBAR_SCANNER_REQUEST = 0;
    private static final int ZBAR_QR_SCANNER_REQUEST = 1;
    private TextView _txtResponse;
    private EditText _editTextToSend;
    private EditText _editIP;
    private EditText _editPort;

    private final static String TAG = "MainActivity";
    private String serverResponse;
    private String dataToSend;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        _txtResponse = findViewById(R.id.txt_response);
        _editTextToSend = findViewById(R.id.edt_text_to_send);
        _editIP = findViewById(R.id.edit_ip);
        _editPort = findViewById(R.id.edit_port);

        _editIP.setInputType(InputType.TYPE_CLASS_PHONE);
        _editPort.setInputType(InputType.TYPE_CLASS_PHONE);
    }

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

    public boolean isCameraAvailable() {
        return getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case ZBAR_SCANNER_REQUEST:
            case ZBAR_QR_SCANNER_REQUEST:
                if (resultCode == RESULT_OK) {
                    dataToSend = data.getStringExtra(ZBarConstants.SCAN_RESULT);
                    sendDataToServer(dataToSend);
                } else if(resultCode == RESULT_CANCELED && data != null) {
                    String error = data.getStringExtra(ZBarConstants.ERROR_INFO);
                    if(!TextUtils.isEmpty(error)) {
                        Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
                    }
                }
                break;
        }
    }

    public void sendDataToServer(String data) {
        ConnectionTask task = new ConnectionTask(_editIP.getText().toString(), 100, data);
        try {
            serverResponse = task.execute().get();
            _txtResponse.setText(serverResponse);
        } catch (InterruptedException e) {
            e.printStackTrace();
        } catch (ExecutionException e) {
            e.printStackTrace();
        }
    }

    public void updateUserInterface(View view) {
        sendDataToServer(_editTextToSend.getText().toString());
        Toast.makeText(this, serverResponse, Toast.LENGTH_LONG).show();
        _txtResponse.setText(serverResponse);
    }
}

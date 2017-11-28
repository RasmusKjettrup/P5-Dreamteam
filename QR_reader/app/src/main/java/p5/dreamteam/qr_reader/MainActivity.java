package p5.dreamteam.qr_reader;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.text.InputType;
import android.text.TextUtils;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

/**
 * Main class that the user meets on app launch.
 */
public class MainActivity extends AppCompatActivity {
    private static final int ZBAR_SCANNER_REQUEST = 0;
    private EditText _editTextToSend;
    private EditText _editIP;
    private EditText _editPort;
    private CheckBox _chkFlash;
    private CheckBox _chkSendImmediately;

    private final static String TAG = "MainActivity";
    private String _serverResponse;

    @SuppressLint("SetTextI18n")
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        _editTextToSend = findViewById(R.id.edit_textToSend);
        _editIP = findViewById(R.id.edit_ip);
        _editPort = findViewById(R.id.edit_port);
        _chkFlash = findViewById(R.id.chk_flash);
        _chkSendImmediately = findViewById(R.id.chk_sendImmediately);

        _editIP.setInputType(InputType.TYPE_CLASS_PHONE);
        _editPort.setInputType(InputType.TYPE_CLASS_PHONE);
        _editIP.setText("192.168.43.7");
        _editPort.setText("100");
    }

    public void checkCamPermissionAndLaunchScanner(View v) {
        if (isCameraAvailable()) { // Next line: request permissions and wait for user response
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, 0);
        } else {
            Toast.makeText(this, "Rear facing camera unavailable", Toast.LENGTH_SHORT).show();
        }
    }

    private void launchScanner() {
        Intent intent = new Intent(this, ZBarScannerActivity.class);
        intent.putExtra("FLASH", _chkFlash.isChecked());
        startActivityForResult(intent, ZBAR_SCANNER_REQUEST);
    }

    public boolean isCameraAvailable() {
        return getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        switch (requestCode) {
            case ZBAR_SCANNER_REQUEST:
                if (resultCode == RESULT_OK) {
                    String dataToSend = data.getStringExtra(ZBarConstants.SCAN_RESULT);
                    if (_chkSendImmediately.isChecked()) {
                        sendDataToServer(dataToSend);
                    } else {
                        _editTextToSend.setText(dataToSend);
                    }
                } else if(resultCode == RESULT_CANCELED && data != null) {
                    String error = data.getStringExtra(ZBarConstants.ERROR_INFO);
                    if(!TextUtils.isEmpty(error)) {
                        Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
                    }
                }
                break;
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String permissions[],
                                           @NonNull int[] grantResults) {
        if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            launchScanner();
        } else {
            Toast.makeText(this, "Camera permission denied. Cannot scan.", Toast.LENGTH_LONG).show();
        }
    }

    public int sendDataToServer(String data) {
        try {
            ConnectionTask task = new ConnectionTask(_editIP.getText().toString(),
                    Integer.parseInt(_editPort.getText().toString()), data);
            _serverResponse = task.execute().get(2, TimeUnit.SECONDS);
            // Horrible workaround to handle missing exceptions in other thread
            if (_serverResponse == null) {
                makeCentreToast("Server not found");
                return 1;
            } else if (_serverResponse.equals("")) {
                makeCentreToast("Found IP, but server not responding");
                return 2;
            } else if(_serverResponse.endsWith("<EOF>")) {
                _serverResponse.replace("<EOF>", "");
                return 0;
            } else
            {
                makeCentreToast("Message lost in during connection");
                return 3;
            }
        } catch (InterruptedException e) {
            makeCentreToast("Interrupted error");
            return 4;
        } catch (ExecutionException e) {
            makeCentreToast("Execution error");
            return 5;
        } catch (TimeoutException e) {
            makeCentreToast("Server timeout");
            return 6;
        }
    }

    private void makeCentreToast(String message) {
        Toast toast = Toast.makeText(this, message, Toast.LENGTH_LONG);
        toast.setGravity(Gravity.CENTER, 0, 0);
        toast.show();
    }

    public void sendDataButtonClick(View view) {
        if (sendDataToServer(_editTextToSend.getText().toString()) == 0) {
            _editTextToSend.setText("");
        }
    }

    public void requestButtonClick(View view){
        int errorCode = sendDataToServer("@req");
        if (errorCode != 0)
            return;
        Intent intent = new Intent(this, VisualRepresentationActivity.class);
        intent.putExtra("RepresentationData", _serverResponse);
        startActivity(intent);
    }
}

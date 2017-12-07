package p5.dreamteam.qr_reader;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.hardware.Camera;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.Window;
import android.view.WindowManager;

import net.sourceforge.zbar.Config;
import net.sourceforge.zbar.Image;
import net.sourceforge.zbar.ImageScanner;
import net.sourceforge.zbar.Symbol;
import net.sourceforge.zbar.SymbolSet;

/**
 * TODO: Add a class header comment!
 */
public class ZBarScannerActivity extends Activity implements Camera.PreviewCallback, ZBarConstants {

    private static final String TAG = "ZBarScannerActivity";
    private CameraPreview _preview;
    private Camera _camera;
    private ImageScanner _scanner;
    private boolean _flash;

    static {
        System.loadLibrary("iconv");
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if(!isCameraAvailable()) {
            cancelRequest();
            return;
        }

        _flash = getIntent().getBooleanExtra("FLASH", _flash);

        // Hide the window title.
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);

        // Create and configure the ImageScanner;
        setupScanner();

        // Create a RelativeLayout container that will hold a SurfaceView,
        // and set it as the content of our activity.
        _preview = new CameraPreview(this, this, _flash);
        setContentView(_preview);
    }

    public void setupScanner() {
        _scanner = new ImageScanner();

        int[] symbols = getIntent().getIntArrayExtra(SCAN_MODES);
        if (symbols != null) {
            _scanner.setConfig(Symbol.NONE, Config.ENABLE, 0);
            for (int symbol : symbols) {
                _scanner.setConfig(symbol, Config.ENABLE, 1);
            }
        }
    }

    @Override
    protected void onResume() {
        super.onResume();

        _camera = Camera.open(); // rear
        if(_camera == null) {
            cancelRequest();
            return;
        }

        _preview.setCamera(_camera);
    }

    @Override
    protected void onStop() {
        super.onStop();

        if (_camera != null) {
            _preview.setCamera(null);
            _camera.cancelAutoFocus();
            _camera.setPreviewCallback(null);
            _camera.stopPreview();
            _camera.release();
            _camera = null;
        }
    }

    @Override
    protected void onRestart() {
        super.onRestart();
        _camera = Camera.open();
        if (_camera == null) {
            cancelRequest();
            return;
        }

        _preview.setCamera(_camera);
    }

    @Override
    protected void onPause() {
        super.onPause();

        if (_camera != null) {
            _preview.setCamera(null);
            _camera.cancelAutoFocus();
            _camera.setPreviewCallback(null);
            _camera.stopPreview();
            _camera.release();

            _camera = null;
        }
    }

    public boolean isCameraAvailable() {
        PackageManager pm = getPackageManager();
        return pm.hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    public void cancelRequest() {
        Intent dataIntent = new Intent();
        dataIntent.putExtra(ERROR_INFO, "Camera unavailable");
        setResult(Activity.RESULT_CANCELED, dataIntent);
        finish();
    }

    public void onPreviewFrame(byte[] data, Camera camera) {
        Camera.Parameters parameters = camera.getParameters();
        Camera.Size size = parameters.getPreviewSize();

        Image barcode = new Image(size.width, size.height, "Y800");
        barcode.setData(data);

        int result = _scanner.scanImage(barcode);

        if (result != 0) {
            _camera.cancelAutoFocus();
            _camera.setPreviewCallback(null);
            _camera.stopPreview();
            SymbolSet syms = _scanner.getResults();
            for (Symbol sym : syms) {
                String symData = sym.getData();
                if (!TextUtils.isEmpty(symData)) {
                    Intent dataIntent = new Intent();
                    dataIntent.putExtra(SCAN_RESULT, symData);
                    dataIntent.putExtra(SCAN_RESULT_TYPE, sym.getType());
                    setResult(Activity.RESULT_OK, dataIntent);
                    finish();
                    break;
                }
            }
        }
    }
}
package p5.dreamteam.qr_reader;

import android.app.Activity;
import android.content.Intent;
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
 * This is where ZBar is actually implemented. Uses {@link CameraPreview} to scan images, and then decodes them.
 * --
 * @see <a href="https://developer.android.com/guide/components/activities/activity-lifecycle.html">
 *     Android Developers: The Activity Lifecycle</a> for more information about the methods in this class
 * @see <a href="https://github.com/ZBar/ZBar/tree/master/zbar">The ZBar GitHub repository, ZBar folder</a>
 * (actually a clone of a Mercurial repository)
 */
public class ZBarScannerActivity extends Activity implements Camera.PreviewCallback, ZBarConstants {

    private static final String TAG = "ZBarScannerActivity";
    private CameraPreview _preview;
    private Camera _camera;
    private ImageScanner _scanner;
    private boolean _flash;

    static { // Unix tool used to convert from one encoding to another. TODO: No idea if this is required
        System.loadLibrary("iconv");
    }

    /**
     * Called when activity is created. If camera not available, exit activity. Check if user wants flash. Fullscreen
     * layout. Set up scanner. Enter camera preview.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        _flash = getIntent().getBooleanExtra("FLASH", _flash);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setupScanner();

        // Create a RelativeLayout container that will hold a SurfaceView,
        // and set it as the content of our activity.
        _preview = new CameraPreview(this, this, _flash);
        setContentView(_preview);
    }

    /**
     * Sets up the image scanner with required stride (10) and required codes (EAN13).
     * The method specifies the symbology pixel stride for each axis. 0 completely disables scanning in a specific
     * direction. We want to be able to scan both in landscape and portrait mode, however, without letting Android
     * rotate the UI, which means that both need to be > 0, sacrificing a small amount of processing power.
     * TODO: How much?
     * We choose a stride of 10 in each direction, which seems to be a good middle ground for performance and fast
     * scanability.
     * void set_config(zbar_symbol_type_t symbology, zbar_config_t config, int value);
     */
    public void setupScanner() {
        _scanner = new ImageScanner();

        // Pixel stride for each direction for all (0) symbol types
        _scanner.setConfig(0, Config.X_DENSITY, 10);
        _scanner.setConfig(0, Config.Y_DENSITY, 10);

        // Check if we only need to recognise certain codes. Then disable everything and enable the required ones
        int[] symbols = getIntent().getIntArrayExtra(SCAN_MODES);
        if (symbols != null) {
            _scanner.setConfig(Symbol.NONE, Config.ENABLE, 0);
            for (int symbol : symbols) {
                _scanner.setConfig(symbol, Config.ENABLE, 1);
            }
        }
    }

    /**
     * Open camera and preview when activity is resumed from pause (e.g. from a turned off screen, or phone call ends)
     */
    @Override
    protected void onResume() {
        super.onResume();

        _camera = Camera.open(); // Rear
        if(_camera == null) {
            cancelRequest();
            return;
        }

        _preview.setCamera(_camera);
    }

    /**
     * Need to disable camera so that other apps may use it when user leaves activity.
     */
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

    /**
     * Open camera and preview when activity is restarted
     */
    @Override
    protected void onRestart() {
        super.onRestart();
        _camera = Camera.open(); // Rear
        if (_camera == null) {
            cancelRequest();
            return;
        }

        _preview.setCamera(_camera);
    }

    /**
     * Need to disable camera so that other apps may use it when user leaves activity.
     */
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

    /**
     * Exit the activity with error info if camera is unavailable.
     */
    public void cancelRequest() {
        Intent dataIntent = new Intent();
        dataIntent.putExtra(ERROR_INFO, "Camera unavailable");
        setResult(Activity.RESULT_CANCELED, dataIntent);
        finish();
    }

    /**
     * Capture image information from the camera stream and send it to the scanner in Y800 pixel format,
     * image by image. Y800 being a YCbCr image with luminance carrying a byte (8 bits) of information,
     * and Cb and Cr both carrying 0 bits, making the image monochrome.
     * @param data An array of pixel luminosities
     * @param camera The camera from where images need to be captured
     */
    public void onPreviewFrame(byte[] data, Camera camera) {
        Camera.Parameters parameters = camera.getParameters();
        Camera.Size size = parameters.getPreviewSize();

        // Pixel format
        Image barcode = new Image(size.width, size.height, "Y800");
        barcode.setData(data);

        // How many symbols were found?
        int result = _scanner.scanImage(barcode);

        if (result != 0) {
            _camera.cancelAutoFocus();
            _camera.setPreviewCallback(null);
            _camera.stopPreview();
            SymbolSet syms = _scanner.getResults();
            for (Symbol sym : syms) { // Should only be 1 symbol
                String symData = sym.getData(); // Actually decode the barcode to get the data
                if (!TextUtils.isEmpty(symData)) {
                    Intent dataIntent = new Intent();
                    dataIntent.putExtra(SCAN_RESULT, symData);
                    dataIntent.putExtra(SCAN_RESULT_TYPE, sym.getType());
                    setResult(Activity.RESULT_OK, dataIntent);
                    finish(); // We have decoded, terminate activity
                    break;
                }
            }
        }
    }
}
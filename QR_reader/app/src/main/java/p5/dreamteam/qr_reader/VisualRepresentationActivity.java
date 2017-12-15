package p5.dreamteam.qr_reader;

import android.app.Activity;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.ListView;

/**
 * Activity for displaying a route from an array of Strings
 */
public class VisualRepresentationActivity extends Activity {
    /**
     * Sets layout from intent getStringExtra() split by newline
     * @param savedInstanceState Get previous activity state of activity if applicaple (e.g. when rotated)
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_visual_representation);
        ListView _view = findViewById(R.id.list);
        String[] _strings = getIntent().getStringExtra("RepresentationData").split("¤");
        ArrayAdapter<String> _adapter = new ArrayAdapter<>(this, R.layout.list_item, _strings);

        _view.setAdapter(_adapter);
    }

}

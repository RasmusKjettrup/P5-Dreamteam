package p5.dreamteam.qr_reader;

import android.app.Activity;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.ListView;

/**
 * Activity for displaying a route from an array of Strings
 */
public class VisualRepresentationActivity extends Activity {
    ListView _view;
    String[] _strings;
    ArrayAdapter<String> _adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_visual_representation);
        _view = findViewById(R.id.list);
        _strings = getIntent().getStringExtra("RepresentationData").split("\n");
        _adapter = new ArrayAdapter<>(this, R.layout.list_item, _strings);

        _view.setAdapter(_adapter);
    }

}

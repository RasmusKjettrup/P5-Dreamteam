package p5.dreamteam.qr_reader;

import android.app.Activity;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.LinearLayout;
import android.widget.ListView;

import java.util.ArrayList;
import java.util.Arrays;

/**
 * Created by kenne on 24-11-2017.
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
        _adapter = new ArrayAdapter<String>(this, R.layout.list_item, _strings);

        _view.setAdapter(_adapter);
    }

}

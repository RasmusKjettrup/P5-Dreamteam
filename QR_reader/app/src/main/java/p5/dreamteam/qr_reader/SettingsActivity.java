package p5.dreamteam.qr_reader;

import android.content.Context;
import android.content.SharedPreferences;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;


public class SettingsActivity extends AppCompatActivity {
    public String IPAddress;
    public String Port;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        Context context = getApplicationContext();
        IPAddress = context.getSharedPreferences(getString(R.string.ipAddress), Context.MODE_PRIVATE).toString();
        Port = context.getSharedPreferences(getString(R.string.port), Context.MODE_PRIVATE).toString();
    }

    public void changeIPAdress(View v){
        String address = ((EditText)findViewById(R.id.address)).getText().toString();
        String port = ((EditText)findViewById(R.id.port)).getText().toString();

        setIPAndPort(address, port);

    }

    public void setIPAndPort(String ipAddres, String port){
        Context context = getApplicationContext();
        SharedPreferences sharedPreferences = context.getSharedPreferences(getString(R.string.ipAddress), Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPreferences.edit();
        editor.putString(getString(R.string.ipAddress),ipAddres);
        editor.commit();

        sharedPreferences = context.getSharedPreferences(getString(R.string.port), Context.MODE_PRIVATE);
        editor = sharedPreferences.edit();
        editor.putString(getString(R.string.ipAddress),ipAddres);
        editor.commit();
    }
}

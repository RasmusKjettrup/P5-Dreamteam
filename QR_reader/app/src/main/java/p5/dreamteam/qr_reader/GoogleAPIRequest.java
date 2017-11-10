package p5.dreamteam.qr_reader;

import android.os.AsyncTask;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;

import javax.net.ssl.HttpsURLConnection;

/**
 * TODO: Add a class header comment!
 */
public class GoogleAPIRequest extends AsyncTask<String, Integer, String> {
    private static String TAG = "GoogleAPIRequest";

    public String test(String isbn) {
        try {
            URL url = new URL("https://www.googleapis.com/books/v1/volumes?q=isbn:" + isbn);
            HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();
            InputStream stream = connection.getInputStream();
            BufferedReader reader = new BufferedReader(new InputStreamReader(stream));
            StringBuilder builder = new StringBuilder();
            String line;
            while ((line = reader.readLine()) != null) {
                builder.append(line);
            }
            JSONObject json = new JSONObject(builder.toString());

            connection.disconnect();
            stream.close();
            builder.delete(0, builder.length());

            return json.getJSONArray("items").getJSONObject(0).getJSONObject("volumeInfo").getString("title");

        } catch (JSONException e) {
            throw new RuntimeException(e);
        } catch (IOException e) {
            Log.e(TAG, "Failed or interrupted I/O");
        }
        return null;
    }

    @Override
    protected String doInBackground(String... strings) {
        return test(strings[0]);
    }

    @Override
    protected void onPostExecute(String res) {
        super.onPostExecute(res);
    }
}

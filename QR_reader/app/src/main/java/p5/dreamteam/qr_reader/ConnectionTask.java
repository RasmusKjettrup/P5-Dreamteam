package p5.dreamteam.qr_reader;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.Socket;

/**
 * Task that asynchronously creates a connection to a server, and terminates on server response.
 */
public class ConnectionTask extends AsyncTask<Void, Void, String> {
    private static final String TAG = "ConnectionTask";
    private String _ip;
    private int _port;
    private String _data;
    private Socket _socket;

    ConnectionTask(String ip, int port, String data) {
        this._ip = ip;
        this._port = port;
        this._data = data;
    }

    @Override
    protected String doInBackground(Void... voids) {
        String response;

        try {
            sendDataToServer(_data);
            response = receiveDataFromServer();
            _socket.close();
        } catch (IOException e) {
            response = "Server not found";
            Log.e(TAG, "Server not found - doInBackground()");
        }

        return response;
    }

    @Override
    protected void onPostExecute(String s) {
        super.onPostExecute(s);
    }

    private String receiveDataFromServer() throws IOException {
        StringBuilder response = new StringBuilder();

        BufferedReader reader = new BufferedReader(new InputStreamReader(_socket.getInputStream()));

        for (String line = reader.readLine(); line != null; line = reader.readLine()) {
            response.append(line);
        }

        return response.substring(0, response.length() - 5); // Remove <EOF> from displayed response
    }

    private void sendDataToServer(String data) throws IOException {
        _socket = new Socket(_ip, _port);
        PrintWriter writer = new PrintWriter(new OutputStreamWriter(_socket.getOutputStream()));
        writer.print(data + "<EOF>"); // Server expects <EOF>, since we can send more lines in one message
        writer.flush();
    }
}

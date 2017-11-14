package p5.dreamteam.qr_reader;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

/**
 * TODO: Add a class header comment!
 */
public class ConnectionTask extends AsyncTask<Void, Void, String> {
    private static final String TAG = "ConnectionTask";
    private String ip;
    private int port;
    private String data;
    private Socket socket;
    private PrintWriter writer;
    private BufferedReader reader;

    public ConnectionTask(String ip, int port, String data) {
        this.ip = ip;
        this.port = port;
        this.data = data;
    }

    @Override
    protected String doInBackground(Void... voids) {
        sendDataToServer(data);
        return receiveDataFromServer();
    }

    private String receiveDataFromServer() {
        return "hello";
    }

    @Override
    protected void onPostExecute(String s) {
        super.onPostExecute(s);
    }

    private void sendDataToServer(String data) {
        try {
            socket = new Socket(ip, port);
            writer = new PrintWriter(socket.getOutputStream(), true);
            reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));

            writer.print(data);
            writer.flush();
        } catch (IOException e) {
            Log.d(TAG, "IO not found");
        }
    }
}

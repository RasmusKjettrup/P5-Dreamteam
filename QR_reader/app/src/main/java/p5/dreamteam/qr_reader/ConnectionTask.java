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
 * TODO: Add a class header comment!
 */
public class ConnectionTask extends AsyncTask<Void, Void, String> {
    private static final String TAG = "ConnectionTask";
    private String ip;
    private int port;
    private String data;
    private Socket socket;

    ConnectionTask(String ip, int port, String data) {
        this.ip = ip;
        this.port = port;
        this.data = data;
    }

    @Override
    protected String doInBackground(Void... voids) {
        sendDataToServer(data);
        String response = receiveDataFromServer();
        try {
            socket.close();
        } catch (IOException e) {
            Log.e(TAG, e.getMessage());
        }
        return response;
    }

    @Override
    protected void onPostExecute(String s) {
        super.onPostExecute(s);
    }

    private String receiveDataFromServer() {
        StringBuilder response = new StringBuilder();
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));

            for (String line = reader.readLine(); line != null; line = reader.readLine()) {
                response.append(line);
            }
        } catch (IOException e) {
            Log.e(TAG, e.getMessage());
        }

        return response.substring(0, response.length() - 5); // Remove <EOF> from displayed response
    }

    private void sendDataToServer(String data) {
        try {
            socket = new Socket(ip, port);
            PrintWriter writer = new PrintWriter(new OutputStreamWriter(socket.getOutputStream()));
            writer.print(data + "<EOF>"); // Server expects <EOF>, since we can send more lines in one message
            writer.flush();
        } catch (IOException e) {
            Log.e(TAG, e.getMessage());
        }
    }
}

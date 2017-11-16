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
        Log.d(TAG, "Sending data to server...");
        sendDataToServer(data);
        Log.d(TAG, "Data sent.");
        Log.d(TAG, "Receiving data...");
        String response = receiveDataFromServer();
        Log.d(TAG, "Response received. Closing socket...");
        try {
            socket.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        Log.d(TAG, "Socket closed, and response sent to onPostExecute");
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
            Log.d(TAG, "Reader initialised.");
            for (String line = reader.readLine(); line != null; line = reader.readLine()) {
                response.append(line);
            }
            Log.d(TAG, "Response read.");
        } catch (IOException e) {
            e.printStackTrace();
        }
        Log.d(TAG, "Returning response...");
        return response.toString();
    }

    private void sendDataToServer(String data) {
        try {
            socket = new Socket(ip, port);
            PrintWriter writer = new PrintWriter(new OutputStreamWriter(socket.getOutputStream()));
            writer.print(data);
            writer.flush();
        } catch (IOException e) {
            Log.e(TAG, e.getMessage());
        }
    }
}

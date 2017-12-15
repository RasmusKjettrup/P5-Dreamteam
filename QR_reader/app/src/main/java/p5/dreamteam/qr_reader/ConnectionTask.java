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
 * Task that "asynchronously" creates a connection to a server, and terminates on server response.
 * "Asynchronously" meaning that it still waits for a server response before terminating.
 * No parameters (Void) sent to the task.
 * No progress update needed (Void).
 * Results in a String (server response) on termination.
 */
public class ConnectionTask extends AsyncTask<Void, Void, String> {
    /**
     * Tag for logging
     */
    private static final String TAG = "ConnectionTask";
    /**
     * The ip to connect to
     */
    private String _ip;
    /**
     * Port to connect through
     */
    private int _port;
    /**
     * Data to send to server. Either scanned or custom message
     */
    private String _data;
    /**
     * The socket to connect through using {@link ConnectionTask#_ip} and {@link ConnectionTask#_port}
     */
    private Socket _socket;

    /**
     * Constructor. Connects using socket.
     * @param ip IP to connect to.
     * @param port Using specified port. Server listens on 100 by default.
     * @param data Data to send to server.
     */
    ConnectionTask(String ip, int port, String data) {
        this._ip = ip;
        this._port = port;
        this._data = data;
    }

    /**
     * Send {@link #_data} to server, and wait for server response.
     * Timeout is handled in MainActivity.
     * Makes asynchronous less smart, but oh well...
     * @return the server response.
     */
    @Override
    protected String doInBackground(Void... voids) {
        String response;

        try {
            sendDataToServer(_data);
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }

        response = receiveDataFromServer();

        try {
            _socket.close();
        } catch (IOException e) {
            Log.e(TAG, "Cannot close socket.");
        }

        return response;
    }

    /**
     * Retrieves data from the server by reading lines from response and stripping EOF
     * @return the response from the server.
     */
    private String receiveDataFromServer() {
        StringBuilder response = new StringBuilder();

        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(_socket.getInputStream()));
            for (String line = reader.readLine(); line != null; line = reader.readLine()) {
                response.append(line);
            }
        } catch (IOException e) {
            return null;
        }

        if (response.toString().endsWith("<EOF>")) {
            return response.substring(0, response.length());
        } else {
            return response.toString();
        }
    }

    /**
     * Creates a new {@link Socket} to communicate with server on local network on specified IP and port.
     * Socket is closed in {@link #receiveDataFromServer()}.
     * @param data String to be sent to server. Ends with EOF string.
     * @throws IOException if something goes wrong with the socket.
     */
    private void sendDataToServer(String data) throws IOException {
        _socket = new Socket(_ip, _port);
        PrintWriter writer = new PrintWriter(new OutputStreamWriter(_socket.getOutputStream()));
        writer.print(data + "<EOF>"); // Server expects <EOF>, since we can send more lines in one message
        writer.flush();
    }
}

package p5.dreamteam.qr_reader;

import android.os.AsyncTask;
import android.os.Handler;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;

import p5.dreamteam.qr_reader.events.DisconnectionEvent;
import p5.dreamteam.qr_reader.events.DisconnectEventListener;
import p5.dreamteam.qr_reader.events.InputEventListener;
import p5.dreamteam.qr_reader.events.RecievedInputEvent;


/**
 * Created by kenne on 31-10-2017.
 */

public class ConnectionHandler extends AsyncTask<Void, Integer, String>{
    private String _serverIP;
    private int _serverPort;
    private String _endSignal = "bye";

    private PrintWriter _outputStream;
    private String _pendingOutput = "";
    private BufferedReader _inputReader;
    private Socket _clientSocket;
    private Handler handler = new Handler();
    private boolean _wasConnected;
    private boolean _currentlyConnecting;

    private RecievedInputEvent _inputEvent = new RecievedInputEvent();
    private DisconnectionEvent _disconnectionEvent = new DisconnectionEvent();


    public ConnectionHandler(InputEventListener inputEventListener, DisconnectEventListener disconnectEventListener){
        this ("192.168.43.2",100, inputEventListener, disconnectEventListener);
    }
    public ConnectionHandler(String serverIP, int serverPort, InputEventListener inputEventListener, DisconnectEventListener disconnectEventListener){
        _serverIP = serverIP;
        _serverPort = serverPort;
        _inputEvent.addListener(inputEventListener);
        _disconnectionEvent.addListener(disconnectEventListener);
    }

    @Override
    protected String doInBackground(Void... voids) {
        _currentlyConnecting = true;
        _wasConnected = clientSetup();
        _currentlyConnecting = false;
        handler.postDelayed(runnable, 1000);

        return "Connection Finished:";
    }

    public String getConnectionStatus(){ // Todo: Mostly used for debugge, consider removing
        if (_wasConnected){
            return "Client has been succesfully connected";
        }else if(_currentlyConnecting){
            return "Connecting";
        }
        return "Client is currently not connected to a server";
    }

    public void sendDataToServer(String data){
        _pendingOutput = data;
    }

    private boolean clientSetup(){
        try{
            _clientSocket = new Socket(_serverIP, _serverPort);
            _outputStream = new PrintWriter(_clientSocket.getOutputStream(),true);
            _outputStream.flush();

            _inputReader = new BufferedReader(new InputStreamReader(_clientSocket.getInputStream()));
        }
        catch (IOException ioe){
            ioe.printStackTrace();
            return false;
        }
        return true;
    }

    private void sendData(String data){
        try {
            _outputStream.print(data);
            _outputStream.flush();
            if (data.equals(_endSignal))
                _disconnectionEvent.invoke();
        } catch (Exception ioException) {
            ioException.printStackTrace();
        }
    }

    private void listenToServer(){
        String message = "";
        do {
            try {
                if (!_inputReader.ready()) {
                    if (message != null) {
                        _inputEvent.invoke(message);
                        message = "";
                    }
                }
                int num = _inputReader.read();
                message += Character.toString((char) num);
            } catch (Exception classNot) {
                classNot.printStackTrace();
            }
        } while (!message.equals(""));
    }

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            listenToServer();

            /*if (!_wasConnected || !_currentlyConnecting){
                _currentlyConnecting = true;
                clientSetup();
                _currentlyConnecting = false;
            }*/
        }
    };
}

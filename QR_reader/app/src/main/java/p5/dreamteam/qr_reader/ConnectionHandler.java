package com.jmkjaer.android.p5_qr_reader;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import com.jmkjaer.android.p5_qr_reader.events.DisconnectionEvent;
import com.jmkjaer.android.p5_qr_reader.events.DisconnectEventListener;
import com.jmkjaer.android.p5_qr_reader.events.InputEventListener;
import com.jmkjaer.android.p5_qr_reader.events.RecievedInputEvent;


/**
 * Created by kenne on 31-10-2017.
 */

public class ConnectionHandler {
    private String _serverIP = "192.168.43.197";
    private int _serverPort = 100;
    private String _endSignal = "bye";

    private PrintWriter _outputStream;
    private BufferedReader _inputReader;
    private Socket _clientSocket;

    private RecievedInputEvent _inputEvent = new RecievedInputEvent();
    private DisconnectionEvent _disconnectionEvent = new DisconnectionEvent();

    public ConnectionHandler(InputEventListener inputEventListener, DisconnectEventListener disconnectEventListener){
        _inputEvent.addListener(inputEventListener);
        _disconnectionEvent.addListener(disconnectEventListener);
    }
    public ConnectionHandler(String serverIP, int serverPort, InputEventListener inputEventListener, DisconnectEventListener disconnectEventListener){
        _serverIP = serverIP;
        _serverPort = serverPort;
        _inputEvent.addListener(inputEventListener);
        _disconnectionEvent.addListener(disconnectEventListener);
    }

    public void clientSetup(){
        try{
            _clientSocket = new Socket(_serverIP, _serverPort);
            _outputStream = new PrintWriter(_clientSocket.getOutputStream(),true);
            // outputStream.flush();
            _inputReader = new BufferedReader(new InputStreamReader(_clientSocket.getInputStream()));
        }
        catch (IOException ioe){
            // Todo: Implement exception handling
        }
    }

    public void sendDataToServer(String data){
        try {
            _outputStream.print(data);
            _outputStream.flush();
            if (data.equals(_endSignal))
                _disconnectionEvent.invoke();
        } catch (Exception ioException) {
            ioException.printStackTrace();
        }
    }

    public void listenToServer(){
        String message = "";
        do {// Todo: Fix busy-waiting
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
                // Todo: Implement exception handling
            }

        } while (!message.equals(""));
    }
}

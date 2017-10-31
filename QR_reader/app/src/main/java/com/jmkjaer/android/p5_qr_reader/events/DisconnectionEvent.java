package com.jmkjaer.android.p5_qr_reader.events;

import java.util.ArrayList;
import java.util.List;



public class DisconnectionEvent {
    private List<DisconnectEventListener> listeners = new ArrayList<DisconnectEventListener>();

    public void addListener(DisconnectEventListener listener) {
        listeners.add(listener);
    }

    public void invoke(){
        for (DisconnectEventListener listener :listeners) {
            listener.disconnectEventInvoke();
        }
    }
}

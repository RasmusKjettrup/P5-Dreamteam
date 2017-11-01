package com.jmkjaer.android.p5_qr_reader.events;

import com.jmkjaer.android.p5_qr_reader.events.InputEventListener;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by kenne on 31-10-2017.
 */

public class RecievedInputEvent{
    private List<InputEventListener> listeners = new ArrayList<InputEventListener>();

    public void addListener(InputEventListener listener) {
        listeners.add(listener);
    }

    public void invoke(String string){
        for (InputEventListener listener :listeners) {
            listener.inputEventInvoked(string);
        }
    }
}

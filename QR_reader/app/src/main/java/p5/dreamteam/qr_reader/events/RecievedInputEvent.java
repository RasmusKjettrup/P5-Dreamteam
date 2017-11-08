package p5.dreamteam.qr_reader.events;

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

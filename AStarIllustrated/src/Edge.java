import processing.core.PApplet;

/**
 * An edge connecting two nodes.
 */
public class Edge {
    private PApplet p;

    private Node start, end;

    Edge(PApplet p, Node start, Node end) {
        this.p = p;
        this.start = start;
        this.end = end;
    }

    public Node getStart() {
        return start;
    }

    public void setStart(Node start) {
        this.start = start;
    }

    public Node getEnd() {
        return end;
    }

    public void setEnd(Node end) {
        this.end = end;
    }
}

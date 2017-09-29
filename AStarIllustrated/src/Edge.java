import processing.core.PApplet;

/**
 * An edge connecting two nodes.
 */
public class Edge implements Comparable<Edge> {
    private PApplet p;

    private Node start, end;

    public int getWeight() {
        return weight;
    }

    public void setWeight(int weight) {
        this.weight = weight;
    }

    private int weight;

    Edge(PApplet p, Node start, Node end, int weight) {
        this.p = p;
        this.start = start;
        this.end = end;
        this.weight = weight;
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

    @Override
    public int compareTo(Edge edge) {
        return (this.weight - edge.weight);
    }
}
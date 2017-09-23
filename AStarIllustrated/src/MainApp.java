import processing.core.PApplet;
import processing.core.PFont;

public class MainApp extends PApplet{
    PFont f;
    private Graph graph;
    private GraphDisplay graphDisplay;

    public static void main(String[] args) {
        PApplet.main("MainApp", args);
    }

    /**
     * Is called once after settings().
     */
    @Override
    public void setup() {
        f = createFont("Arial", 22, true);
        graph = new Graph(this);

        background(255, 204, 0);

        graph.getNodes().add(new Node(this, "A"));
        graph.getNodes().add(new Node(this, "B"));
        graph.getNodes().add(new Node(this, "C"));
        graph.getNodes().add(new Node(this, "D"));
        graph.getNodes().add(new Node(this, "E"));

        graph.getEdges().add(new Edge(this, graph.findNodeByName("A"), graph.findNodeByName("B")));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("B"), graph.findNodeByName("A")));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("B"), graph.findNodeByName("C")));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("C"), graph.findNodeByName("D")));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("D"), graph.findNodeByName("A")));

        graph.setStartNode(graph.findNodeByName("A"));
        graph.setEndNode(graph.findNodeByName("E"));

        graphDisplay = new GraphDisplay(this, graph);
    }

    /**
     * Is called repeatedly after setup().
     */
    @Override
    public void draw() {
        background(255, 204, 0);
//        graphDisplay = new GraphDisplay(this, graph);
        graphDisplay.display();
    }

    /**
     * Is called once in the beginning.
     */
    @Override
    public void settings() {
        size(700, 700);
    }
}
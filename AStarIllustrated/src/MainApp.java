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
        graph.getNodes().add(new Node(this, "F"));

        graph.getEdges().add(new Edge(this, graph.findNodeByName("A"), graph.findNodeByName("D"), 3));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("A"), graph.findNodeByName("F"), 2));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("F"), graph.findNodeByName("B"), 1));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("A"), graph.findNodeByName("C"), 1));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("C"), graph.findNodeByName("E"), 5));
        graph.getEdges().add(new Edge(this, graph.findNodeByName("C"), graph.findNodeByName("B"), 8));

//        graph.setStartNode(graph.findNodeByName("A"));
//        graph.setEndNode(graph.findNodeByName("E"));

        graphDisplay = new GraphDisplay(this, graph);

        Christofides christofides = new Christofides(this, graph);
        christofides.CreateMinimumSpanningTree(graph);
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
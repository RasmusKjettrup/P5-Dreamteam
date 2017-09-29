import processing.core.PApplet;
import java.util.ArrayList;

public class Christofides {
    private PApplet p;
    private Graph graph, H, T;
    private ArrayList<Node> O;
    private ArrayList<Edge> M;

    public Christofides(PApplet p, Graph graph) {
        this.p = p;
        this.graph = graph;
    }

    public Graph CalculateShortestPath() {
        T = CreateMinimumSpanningTree(graph);
        O = CalculateVerticesWithOddDegree(T);
        M = PerfectMatching(O);
        H = CreateMultigraph(M, T);
        H = FormEulerCircuit(H);
        H = CreateHamiltonCircuit(H);

        return H;
    }

    private Graph CreateMinimumSpanningTree(Graph graph) {
        return null;
    }

    private ArrayList<Node> CalculateVerticesWithOddDegree(Graph t) {
        return null;
    }

    private ArrayList<Edge> PerfectMatching(ArrayList<Node> o) {
        return null;
    }

    private Graph CreateMultigraph(ArrayList<Edge> m, Graph t) {
        return null;
    }

    private Graph FormEulerCircuit(Graph h) {
        return null;
    }

    private Graph CreateHamiltonCircuit(Graph h) {
        return null;
    }

    public Graph getGraph() {
        return graph;
    }

    public void setGraph(Graph graph) {
        this.graph = graph;
    }

    public Graph getH() {
        return H;
    }

    public void setH(Graph h) {
        H = h;
    }

    public Graph getT() {
        return T;
    }

    public void setT(Graph t) {
        T = t;
    }

    public ArrayList<Node> getO() {
        return O;
    }

    public void setO(ArrayList<Node> o) {
        O = o;
    }

    public ArrayList<Edge> getM() {
        return M;
    }

    public void setM(ArrayList<Edge> m) {
        M = m;
    }
}

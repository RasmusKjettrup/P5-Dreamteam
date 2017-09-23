import processing.core.PApplet;

import java.util.HashSet;
import java.util.Iterator;

/**
 * The graph which includes nodes and edges.
 * Is iterable on Node.
 */
public class Graph implements Iterable<Node> {
    private PApplet p;

    private HashSet<Edge> edges;
    private HashSet<Node> nodes;
    private Node startNode, endNode;

    Graph(PApplet p) {
        this.p = p;

        edges = new HashSet<>();
        nodes = new HashSet<>();
    }

    Node findNodeByName(String name) {
        for (Node node : nodes) {
            if (node.getName().equals(name)) {
                return node;
            }
        }
        return null;
    }

    HashSet<Edge> getEdges() {
        return edges;
    }

    void setEdges(HashSet<Edge> edges) {
        this.edges = edges;
    }

    HashSet<Node> getNodes() {
        return nodes;
    }

    void setNodes(HashSet<Node> nodes) {
        this.nodes = nodes;
    }

    Node getStartNode() {
        return startNode;
    }

    void setStartNode(Node startNode) {
        this.startNode = startNode;
    }

    Node getEndNode() {
        return endNode;
    }

    void setEndNode(Node endNode) {
        this.endNode = endNode;
    }

    @Override
    public Iterator<Node> iterator() {
        return nodes.iterator();
    }
}

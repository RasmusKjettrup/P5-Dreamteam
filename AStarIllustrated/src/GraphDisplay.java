import processing.core.PApplet;
import processing.core.PConstants;

import java.util.HashMap;

/**
 * GraphDisplay takes the responsibility of drawing the edges and nodes,
 * so that the Graph, Node, and Edge classes don't have to include functionality
 * for that.
 */
class GraphDisplay {
    private PApplet p;
    private Graph graph;
    private HashMap<Node, NodeDisplay> nodeMap;
    private HashMap<Edge, EdgeDisplay> edgeMap;
    private static final float nodeDiameter = 50;
    private static final float nodeRadius = nodeDiameter/2;

    GraphDisplay(PApplet p, Graph graph) {
        this.p = p;
        this.graph = graph;
        this.nodeMap = new HashMap<>();
        this.edgeMap = new HashMap<>();

        for (Node node : graph.getNodes()) {
            nodeMap.put(node, new NodeDisplay(node.getName()));
        }
        for (Edge edge : graph.getEdges()) {
            edgeMap.put(edge, new EdgeDisplay(nodeMap.get(edge.getStart()), nodeMap.get(edge.getEnd())));
        }
    }

    /**
     * Method for the actual display of GraphDisplay's information about the graph.
     */
    void display() {
        // Nodes
        for (NodeDisplay nodeDisplay : nodeMap.values()) {
            Coordinate center = nodeDisplay.center;
            p.fill(255);
            p.ellipse(center.getX(), center.getY(), nodeDiameter, nodeDiameter);
            p.textAlign(PConstants.CENTER);
            p.fill(0);
            p.text(nodeDisplay.name, center.getX(), center.getY() - 10);
        }

        // Edges
        for (EdgeDisplay edgeDisplay : edgeMap.values()) {
            p.line(edgeDisplay.start.getX(), edgeDisplay.start.getY(),
                    edgeDisplay.end.getX(), edgeDisplay.end.getY());
            p.fill(0);
            p.ellipse(edgeDisplay.end.getX(), edgeDisplay.end.getY(), 10, 10);
        }
    }

    /**
     * Calculates the coordinates on the node circle where the line EdgeDisplay must end
     * instead of it going all the way to the center of the NodeDisplay.
     * @param start The start NodeDisplay.
     * @param end The end NodeDisplay.
     * @return The point on the circle where EdgeDisplay ends.
     */
    private Coordinate calcCoordOnEndNodeDisplay(NodeDisplay start, NodeDisplay end) {
        float x = end.center.getX() + nodeRadius * PApplet.cos(calcAngle(start.center, end.center));
        float y = end.center.getY() + nodeRadius * PApplet.sin(calcAngle(start.center, end.center));
        return new Coordinate(x, y);
    }

    /**
     * Calculates the angle or slope of the line between two points (used with NodeDisplay's centers)
     * @param start Start coordinate.
     * @param end End coordinate.
     * @return The slope of the line between the two points.
     */
    private float calcAngle(Coordinate start, Coordinate end) {
        float dx = end.getX() - start.getX();
        float dy = end.getY() - start.getY();
        float theta = PApplet.atan2(dy, dx);
        theta += PApplet.PI;
        return theta;
    }

    /**
     * Imitates a Node and makes it able to be displayed.
     */
    private class NodeDisplay {
        Coordinate center;
        String name;

        NodeDisplay(String name) {
            this.center = new Coordinate(p.random(p.width/20, p.width - p.width/20), p.random(p.height/20, p.height - p.height/20)); // So that circles cannot be placed on end of canvas
            this.name = name;
        }
    }

    /**
     * Imitates an Edge and makes it able to be displayed.
     */
    private class EdgeDisplay {
        Coordinate start, end;

        EdgeDisplay(NodeDisplay start, NodeDisplay end) {
            this.start = start.center;
            this.end = calcCoordOnEndNodeDisplay(start, end);
        }
    }
}
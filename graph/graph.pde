Graph graph;
PFont f;
float Diameter = 50;
float Radius = Diameter/2;

class Node {
  boolean Visited;
  String Name;
  ArrayList<Node> Neighbors;
  float CenterX, CenterY;
  
  Node(String name) {
    Name = name;
    Visited = false;
    Neighbors = new ArrayList<Node>();
    CenterX = random(width);
    CenterY = random(height);
  }
  
  void display() {
    fill(255);
    ellipse(CenterX, CenterY, Diameter, Diameter);
    fill(0);
    text(Name, CenterX, CenterY);
  }
}

class Edge {
  Node Start, End;
  float slope, angle, theta, dy, dx;
  
  Edge(Node start, Node end) {
    Start = start;
    End = end;
  }
  
  void display() {
    stroke(0);
    fill(255);
    line(Start.CenterX, Start.CenterY, calcCoordXOnEndCircle(), calcCoordYOnEndCircle());
  }

  float calcCoordXOnEndCircle() {
    return End.CenterX + Radius * cos(calcAngle());
  }
  
  float calcCoordYOnEndCircle() {
    return End.CenterY + Radius * sin(calcAngle());
  }
  
  float calcAngle() {
    dy = End.CenterY - Start.CenterY;
    dx = End.CenterX - Start.CenterX;
    theta = atan2(dy, dx);
    theta += PI;
    return theta;
  }
}

class Graph {
  ArrayList<Edge> Edges;
  ArrayList<Node> Nodes;
  Node StartNode, EndNode;
  
  Graph() {
    Edges = new ArrayList();
    Nodes = new ArrayList();
    StartNode = null;
    EndNode = null;
  }
  
  Node findNodeByName(String name) {
    for (int i = 0; i < Nodes.size(); i++) {
      if (Nodes.get(i).Name.equals(name)) {
        return Nodes.get(i);
      }
    }
    return null;
  }
  
  void display() {
    stroke(0);
    fill(255);
    for (int i = 0; i < Nodes.size(); i++) {
      Nodes.get(i).display();
    }
    for (int i = 0; i < Edges.size(); i++) {
      Edges.get(i).display();
    }
  }
}

void setup() { //<>//
  size(700, 700); //<>//
  background(255, 204, 0);
  f = createFont("Arial", 22, true);
  
  graph = new Graph();
  
  graph.Nodes.add(new Node("A"));
  graph.Nodes.add(new Node("B"));
  graph.Nodes.add(new Node("C"));
  graph.Nodes.add(new Node("D"));
  graph.Nodes.add(new Node("E"));
  
  graph.Edges.add(new Edge(graph.findNodeByName("A"), graph.findNodeByName("B")));
  graph.Edges.add(new Edge(graph.findNodeByName("B"), graph.findNodeByName("A")));
  graph.Edges.add(new Edge(graph.findNodeByName("B"), graph.findNodeByName("C")));
  graph.Edges.add(new Edge(graph.findNodeByName("B"), graph.findNodeByName("D")));
  graph.Edges.add(new Edge(graph.findNodeByName("C"), graph.findNodeByName("A")));
  graph.Edges.add(new Edge(graph.findNodeByName("D"), graph.findNodeByName("A")));
  graph.Edges.add(new Edge(graph.findNodeByName("D"), graph.findNodeByName("E")));
  graph.Edges.add(new Edge(graph.findNodeByName("E"), graph.findNodeByName("B")));

  graph.StartNode = graph.findNodeByName("A");
  graph.EndNode = graph.findNodeByName("E");
  
  graph.display();
}

void draw() {
}
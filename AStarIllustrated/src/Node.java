import processing.core.PApplet;

class Node {
    private PApplet p;

    private String name;

    Node(PApplet p, String name) {
        this.p = p;
        this.name = name;
    }

    String getName() {
        return name;
    }
}
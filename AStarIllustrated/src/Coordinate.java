/**
 * A point on the canvas.
 */
class Coordinate {
    private final float x;
    private final float y;

    Coordinate(float x, float y) {
        this.x = x;
        this.y = y;
    }

    float getX() {
        return x;
    }

    float getY() {
        return y;
    }
}

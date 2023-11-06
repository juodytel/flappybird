using flapbird;

namespace flapbird;

public class Bird
{
    private int xPosition;
    private int yPosition;

    public Bird(int rows, int columns)
    {
        xPosition = rows / 2;
        yPosition = (columns - 1) / 2;
    }

    public int XPosition => xPosition;

    public int YPosition => yPosition;


    public void MoveUp()
    {
        yPosition--;
    }

    public void MoveDown()
    {
        yPosition++;
    }
}
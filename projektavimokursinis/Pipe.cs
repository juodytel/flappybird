using System;

namespace flapbird;

public class Pipe
{
    private int xPosition;
    public const int GapHeight = 4;
    private int gapTopY;

    public Pipe(int columns, int rows)
    {
        xPosition = columns - 1;
        gapTopY = GenerateRandomPosition(rows);
    }
    
    public int GapTopY => gapTopY;
    
    public int XPosition => xPosition;
    

    private int GenerateRandomPosition(int rows)
    {
        Random random = new Random();
        return random.Next(2, rows - 2 - GapHeight);
    }

    public void ShiftLeft()
    {
        xPosition--;
    }
    
}
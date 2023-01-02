using System.Collections.Generic;
using UnityEngine;

public class DirectionHelper
{
    public static List<Direction> GetDirectionOfMovement(Vector2 vector)
    {
        List<Direction> toReturn = new List<Direction>();
        if (vector.x > 0)
        {
            toReturn.Add(Direction.RIGHT);
        }
        else if (vector.x < 0)
        {
            toReturn.Add(Direction.LEFT);
        }

        if (vector.y > 0)
        {
            toReturn.Add(Direction.UP);
        }
        else if (vector.y < 0)
        {
            toReturn.Add(Direction.DOWN);
        }

        return toReturn;
    }

}

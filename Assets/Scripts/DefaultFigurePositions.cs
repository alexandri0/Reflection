using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DefaultFigurePositions
{
    public static Vector3Int Lazer = new Vector3Int(0, -4, 0);
    public static Vector3Int Pillar = new Vector3Int(0, -6, 0);
    public static Vector3Int[] Prizm = new Vector3Int[2]
    {
        new Vector3Int(0,-5,0),
        new Vector3Int(-1,-5,0)
    };
    public static Vector3Int[] Mirror = new Vector3Int[4]
    {
        new Vector3Int(1,-4,0),
        new Vector3Int(2,-4,0),
        new Vector3Int(-1,-4,0),
        new Vector3Int(-2,-4,0)
    };

    public static Vector3Int[] Get(FigureType type, int player)
    {
        switch (type)
        {
            case FigureType.Lazer:
                if (player == 1)
                    return new Vector3Int[1] { Lazer };
                else
                    return Mul_Y_ToMinusOne(new Vector3Int[1] { Lazer });
            case FigureType.Pillar:
                if (player == 1)
                    return new Vector3Int[1] { Pillar };
                else
                    return Mul_Y_ToMinusOne(new Vector3Int[1] { Pillar });
            case FigureType.Prizm:
                if (player == 1)
                    return Prizm;
                else
                    return Mul_Y_ToMinusOne(Prizm);
            case FigureType.Mirror:
                if (player == 1)
                    return Mirror;
                else
                    return Mul_Y_ToMinusOne(Mirror);
            default: return new Vector3Int[1] { Vector3Int.zero };
        };
    }

    private static Vector3Int[] Mul_Y_ToMinusOne(Vector3Int[] input) // mirror position by Y
    {
        Vector3Int[] output = new Vector3Int[input.Length];
        for (int x = 0; x < input.Length; x++)
        {
            output[x] = input[x];
            output[x].y *= -1;
        }
            
        return output;
    }
}

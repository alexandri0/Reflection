using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position;
    public Cell()
    {
        position = Vector3Int.zero;
    }
    public Cell(Vector3Int position)
    {
        this.position = position;
    }

    public void Move(int direction)
    {
        switch (direction)
        {
            case 0: Right(); break;
            case 1: TopRight(); break;
            case 2: TopLeft(); break;
            case 3: Left(); break;
            case 4: DownLeft(); break;
            case 5: DownRight(); break;
        }
    }
    public void SetPosition(Vector3Int pos)
    {
        position = pos;
    }


    void Right()
    {
        position += Vector3Int.right;
    }
    void TopRight()
    {
        if (position.y % 2 == 0)
            position += Vector3Int.up;
        else
            position += Vector3Int.up + Vector3Int.right;
    }
    void TopLeft()
    {
        if (position.y % 2 == 0)
            position += Vector3Int.up + Vector3Int.left;
        else
            position += Vector3Int.up ;
    }
    void Left()
    {
        position += Vector3Int.left;
    }
    void DownLeft()
    {
        if (position.y % 2 == 0)
            position += Vector3Int.down + Vector3Int.left;
        else
            position += Vector3Int.down;
    }
    void DownRight()
    {
        if (position.y % 2 == 0)
            position += Vector3Int.down;
        else
            position += Vector3Int.down + Vector3Int.right;
    }  
}

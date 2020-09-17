using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FigureType
{
    Pillar = 0,
    Lazer,
    Prizm,
    Mirror
}
public class Figure
{
    public int player; //1 2
    public FigureType type;
    public int direction;
    public Vector3Int position;
    public Figure(FigureType t, int number, int player)
    {
        type = t;
        direction = 0;
        this.player = player;
        position = DefaultFigurePositions.Get(type, player)[number];
    }

    public int Reflect(int input_direction) // return 10 means no reflection
    {
        int local_input = new HexDirection(input_direction) - 3;
        switch (type)
        {
            case FigureType.Pillar: return 10;
            case FigureType.Lazer: return 10;
            case FigureType.Mirror:               
                int left = new HexDirection(direction) + 1;
                int right= new HexDirection(direction) - 1;
                if (local_input == left)
                    return right;
                else if (local_input == right)
                    return left;
                else return 10;
            case FigureType.Prizm:
                if(direction % 2 == 0)
                    return local_input % 2 == 0 ? new HexDirection(local_input) + 1: new HexDirection(local_input) - 1;
                else
                    return local_input % 2 == 0 ? new HexDirection(local_input) - 1 : new HexDirection(local_input) + 1;
        }
        return 10;
    }

    internal class HexDirection
    {
        int Value { get; set; }

        public HexDirection(int initval)
        {
            Value = initval;
        }
        public static int operator +(HexDirection d1, int x) // moving around the ring, 01234501234501 ...
        {
            int outd = d1.Value + x;
            while (outd > 5)
                outd -= 6;
            return outd;
        }

        public static int operator -(HexDirection d1, int x) // same, but another side
        {
            int outd = d1.Value - x;
            while (outd < 0)
                outd += 6;
            return outd;
        }
    }
}

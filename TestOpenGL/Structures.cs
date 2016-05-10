﻿using System;
using System.Collections.Generic;

using TestOpenGL.Logic;
using TestOpenGL.VisualObjects;

namespace TestOpenGL
{
    /// <summary>
    /// Структура для передачи нескольких блоков.
    /// </summary>
    struct VisualObjectStructure<T>
    {
        private Queue<T> objects;
        private Queue<Coord> coords;
        public VisualObjectStructure(Queue<T> objects, Queue<Coord> coords)
        {
            this.coords = coords;
            this.objects = objects;
        }
        public void Push(T O, Coord C)
        {
            if (objects == null)
                objects = new Queue<T>();
            if (coords == null)
                coords = new Queue<Coord>();

            objects.Enqueue(O);
            coords.Enqueue(C);
        }
        public T PopObject()
        {
            return objects.Dequeue();
        }
        public Coord PopCoord()
        {
            return coords.Dequeue();
        }
        public int Count
        {
            get { return objects.Count; }
        }
    }
    


    /// <summary>
    /// Структура для передачи координат.
    /// </summary>
    struct Coord
    {
        private int x;
        private int y;
        private int z;

        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }

        public int Z
        {
            get { return z; }
        }

        public Coord(int x, int y)
            : this(x, y, 0)
        { }
        public Coord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            if (!Analytics.CorrectCoordinate(x, y, z))
                throw new Exception("Неверненькие координатки пришли.");
        }
    }

    /// <summary>
    /// Перечисление частей тела, которые может занимать одетый предмет.
    /// </summary>
    enum Part { Head, Body, Leg, LHand, RHand };

    enum Feature { Power, Stamina, Agility };

    public delegate void EventDelegate();

    enum TypeVisualObject { Block, Being, Decal};

    enum Direction { Left, Up, Right, Down};
}

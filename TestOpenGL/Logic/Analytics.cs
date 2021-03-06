﻿using System;
using System.Collections.Generic;

using TestOpenGL.PhisicalObjects;
using TestOpenGL.Renders;

namespace TestOpenGL.Logic
{
    static class Analytics
    {
        //TODO: почему я смотрю на это, и мне хочется плакать?..

        
        public static Func<B, R> Partial<A, B, R>(this Func<A, B, R> f, A a)
        {
            return b => f(a, b);
        }
        public static Func<T7, T8, T9, T10, R> WepCon<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>
            (this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> f, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return (t7, t8, t9, t10) => f(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }
        public static Action RemoverDecal<A>(this Action<A> f, A a)
        {
            return () => f(a);
        }
        

        /// <summary>
        /// Проверка переданных координат на корректность.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool CorrectCoordinate(int x, int y)
        {
            if (x >= 0 && x < GlobalData.WorldData.Level.LengthX)
                if (y >= 0 && y < GlobalData.WorldData.Level.LengthY)
                    return true;
            return false;
        }

        /*static public Func<int, int, int> add = (x, y) => x + y;
        
        static public Func<int, Func<int, int>> curriedAdd = add.Curry();
        static public Func<int, int> inc = curriedAdd(1);
        static public Func<A, Func<B, R>> Curry<A, B, R>(this Func<A, B, R> f)
        {
            return a => b => f(a, b);
        }*/

        /// <summary>
        /// Поиск пути в ширину.
        /// </summary>
        /// <param name="start"> Стартовые координаты.</param>
        /// <param name="end"> Координаты точки, к которой ищем путь.</param>
        /// <returns></returns>
        static public Stack<Coord> BFS(Coord start, Coord end)
        {
            //Stopwatch SW = new Stopwatch();
            //SW.Start();
            Stack<Coord> SC = new Stack<Coord>();
            int[] dx = { 1, 0, -1, 0 }, dy = { 0, 1, 0, -1 };
            int[,] grid = new int[GlobalData.WorldData.Level.LengthX, GlobalData.WorldData.Level.LengthY];

            for (int i = 0; i < GlobalData.WorldData.Level.LengthX; i++)
                for (int j = 0; j < GlobalData.WorldData.Level.LengthY; j++)
                    grid[i, j] = -1;

            int d = 0;
            bool stope;
            grid[start.X, start.Y] = 0;

            do
            {
                stope = true;
                for (int xd = 0; xd < GlobalData.WorldData.Level.LengthX; xd++)
                {
                    for (int yd = 0; yd < GlobalData.WorldData.Level.LengthY; yd++)
                    {
                        if (grid[xd, yd] == d)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if(CorrectCoordinate(xd + dx[k], yd + dy[k]))//(yd + dy[k] >= 0 && yd + dy[k] < GlobalData.WorldData.Level.LengthY && xd + dx[k] >= 0 && xd + dx[k] < GlobalData.WorldData.Level.LengthX)
                                {
                                    if (GlobalData.WorldData.Level.IsPassable(0, new Coord(xd + dx[k], yd + dy[k])) == true || (xd + dx[k] == end.X && yd + dy[k] == end.Y))
                                    {
                                        if (grid[xd + dx[k], yd + dy[k]] == -1)
                                        {
                                            stope = false;
                                            grid[xd + dx[k], yd + dy[k]] = d + 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                d++;
            } while (stope == false && grid[end.X, end.Y] == -1);

            if (grid[end.X, end.Y] == -1)
                return SC;

            int xdop = end.X;
            int ydop = end.Y;
            while (d > 0)
            {
                SC.Push(new Coord(xdop, ydop));
                d--;
                for (int k = 0; k < 4; k++)
                {
                    if ((ydop + dy[k] >= 0 && ydop + dy[k] < GlobalData.WorldData.Level.LengthY && xdop + dx[k] >= 0 && xdop + dx[k] < GlobalData.WorldData.Level.LengthX))//new Coord(xdop + dx[k], ydop + dy[k]);//
                        if (grid[xdop + dx[k], ydop + dy[k]] == d)
                        {
                            xdop = xdop + dx[k];
                            ydop = ydop + dy[k];
                            break;
                        }
                }
            }
            //SW.Stop();
            //System.Windows.Forms.MessageBox.Show(SW.ElapsedMilliseconds.ToString());

            return SC;
        }

        static public Direction DirectionToGrid(Coord start, Coord end)
        {
            if (start.X <= end.X && start.Y < end.Y)
                return Direction.Up;
            if (start.X > end.X && start.Y <= end.Y)
                return Direction.Left;
            if (start.X >= end.X && start.Y > end.Y)
                return Direction.Down;
            if (start.X < end.X && start.Y >= end.Y)
                return Direction.Right;

            return Direction.None;
        }

        /// <summary>
        /// Возвращает очередь координат, из которых строится линия от ячейки start до ячейки end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static public Queue<Coord> CoordsLine(Coord start, Coord end)
        {
            Queue<Coord> coords = new Queue<Coord>(); ;
            int dx, dy;
            int lengthX, lengthY, length;

            dx = end.X - start.X >= 0 ? 1 : -1;
            dy = end.Y - start.Y >= 0 ? 1 : -1;

            lengthX = Math.Abs(end.X - start.X);
            lengthY = Math.Abs(end.Y - start.Y);

            length = lengthX > lengthY ? lengthX : lengthY;

            if (length == 0)
            {
                coords.Enqueue(start);
                return coords;
            }



            int X, Y, d;

            X = start.X;
            Y = start.Y;
            d = -lengthX;

            length++;
            while (length > 0)
            {
                length--;
                coords.Enqueue(new Coord(X, Y));
                if (lengthY <= lengthX)
                {
                    X = X + dx;
                    d = d + 2 * lengthY;
                    if (d > 0)
                    {
                        d = d - 2 * lengthX;
                        Y = Y + dy;
                    }
                }
                else
                {
                    Y = Y + dy;
                    d = d + 2 * lengthX;
                    if (d > 0)
                    {
                        d = d - 2 * lengthY;
                        X = X + dx;
                    }
                }
            }

            return coords;
        }

        static public double GetGlobalDepth(Layer layer, int partLayer, ModifyDepth modifyDepth)
        {
            double resultDepth = 0;
            for (int i = 0; i < (int)layer; i++)
                resultDepth += GlobalData.WorldData.Level.GetDepthLayer((Layer)i);

            double delta = 0.4;

            switch (modifyDepth)
            {
                case ModifyDepth.None:
                    resultDepth += partLayer;
                    break;

                case ModifyDepth.UnderLayer:
                    resultDepth -= delta;
                    break;

                case ModifyDepth.ToLayer:
                    resultDepth += GlobalData.WorldData.Level.GetDepthLayer(layer) - 1 + delta;
                    break;

                case ModifyDepth.UnderPartLayer:
                    resultDepth += partLayer - delta;
                    break;

                case ModifyDepth.ToPartLayer:
                    resultDepth += partLayer + delta;
                    break;
            }

            return resultDepth;
        }

        static public bool IsInCamera(Coord C, Camera camera)
        {
            if (C.X < camera.MinX)
                return false;
            if (C.X > camera.MaxX)
                return false;
            if (C.Y < camera.MinY)
                return false;
            if (C.Y > camera.MaxY)
                return false;
            return true;
        }

        static public bool IsInArea(Coord C1, Coord C2, Coord C)
        {
            bool inX = false, inY = false;
            if (C1.X >= C2.X)
            { 
                if (C.X <= C1.X && C.X >= C2.X)
                    inX = true;
            }
            else
            {
                if (C.X <= C2.X && C.X >= C1.X)
                    inX = true;
            }

            if (C1.Y >= C2.Y)
            {
                if (C.Y <= C1.Y && C.Y >= C2.Y)
                    inY = true;
            }
            else
            {
                if (C.Y <= C2.Y && C.Y >= C1.Y)
                    inY = true;
            }

            return inX && inY;
        }
        static public List<Being> GetBeingInArea(Coord C1, Coord C2)
        {
            List<Being> LB = GlobalData.WorldData.Level.GetMap<Being>().GetAllObject();
            List<Being> answerLB = new List<Being>();

            foreach (Being being in LB)
                if (IsInArea(C1, C2, being.Coord))
                    answerLB.Add(being);

            return answerLB;
        }
        static public List<Being> GetAllEnemies(Being being)
        {
            Coord C1, C2;
            int[] mass = { 
                             being.Coord.X - being.RangeOfVisibility, 
                             being.Coord.Y - being.RangeOfVisibility,
                             being.Coord.X + being.RangeOfVisibility, 
                             being.Coord.Y + being.RangeOfVisibility
                         };

            mass[0] = mass[0] < 0 ? 0 : mass[0];
            mass[1] = mass[1] < 0 ? 0 : mass[1];
            mass[2] = mass[2] >= GlobalData.WorldData.Level.LengthX ? GlobalData.WorldData.Level.LengthX - 1 : mass[2];
            mass[3] = mass[3] >= GlobalData.WorldData.Level.LengthY ? GlobalData.WorldData.Level.LengthY - 1 : mass[3];

            C1 = new Coord(mass[0], mass[1]);
            C2 = new Coord(mass[2], mass[3]);

            List<Being> LB = GetBeingInArea(C1, C2);
            for (int i = LB.Count - 1; i >= 0; i--)
            {
                if (LB[i].Alliance == being.Alliance || LB[i].Alliance < 0)
                    LB.RemoveAt(i);
            }

            return LB;
        }

        static public int Distance(Coord C1, Coord C2)
        {
            return Math.Abs(C1.X - C2.X) + Math.Abs(C1.Y - C2.Y);
        }
    }
}

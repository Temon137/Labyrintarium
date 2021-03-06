﻿using System;

using TestOpenGL.OutInfo;


namespace TestOpenGL.Renders
{
    class Camera
    {
        // Размеры отображаемой части поля
        int width, height;
        // Текущее смещение камеры относительно блока (0, 0)
        int shiftX, shiftY;
        // Позиция прицела
        VoidEventDelegate changePositionEvent;
        VoidEventDelegate changeSizeEvent;
        
        PhisicalObject looking;
        //-------------


        public Camera(int width, int height)
        {
            this.width = width;
            this.height = height;

            Look();
        }

        public int Height
        { 
            get { return height; }
            set
            {
                height = value > GlobalData.WorldData.Level.LengthX || value > GlobalData.WorldData.Level.LengthY ? Math.Min(GlobalData.WorldData.Level.LengthX, GlobalData.WorldData.Level.LengthY) : (value < 1 ? 1 : value);
                Look();
                changeSizeEvent?.Invoke();
            } 
        }
        public int Width
        { 
            get { return width; }
            set
            {
                width = value > GlobalData.WorldData.Level.LengthX || value > GlobalData.WorldData.Level.LengthY ? Math.Min(GlobalData.WorldData.Level.LengthX, GlobalData.WorldData.Level.LengthY) : (value < 1 ? 1 : value); ;
                Look();
                changeSizeEvent?.Invoke();
            } 
        }

        public int MinX
        { get { return shiftX; } }
        public int MaxX
        { get { return shiftX + Width - 1; } }
        public int MinY
        { get { return shiftY; } }
        public int MaxY
        { get { return shiftY + Height - 1; } }

        public event VoidEventDelegate ChangePositionEvent
        {
            add { changePositionEvent += value; }
            remove { changePositionEvent -= value; }
        }
        public event VoidEventDelegate ChangeSizeEvent
        {
            add
            {
                changeSizeEvent += value;
                changeSizeEvent?.Invoke();
            }
            remove { changeSizeEvent -= value; }
        }
        //=============


        public void SetLookingVO(PhisicalObject b)
        {
            looking = b;
            Look();
            if (b != null)
            {
                b.ChangeCoordEvent += new VoidEventDelegate(Look);
            }
        }

        public void Look()
        {
            if (looking != null)
            {
                shiftX = looking.Coord.X - width / 2;
                shiftX = shiftX < 0 ? 0 : shiftX;
                shiftX = shiftX > GlobalData.WorldData.Level.LengthX - width ? GlobalData.WorldData.Level.LengthX - width : shiftX;
                shiftY = looking.Coord.Y - height / 2;
                shiftY = shiftY < 0 ? 0 : shiftY;
                shiftY = shiftY > GlobalData.WorldData.Level.LengthY - height ? GlobalData.WorldData.Level.LengthY - height : shiftY; //TODO: ВОТ ЗДУСЬ ЖИВУТ ИНОГДА ЧЁРТОВЫ НЕВЕРНЕНЬКИЕ КООРДИНАТКИ!!! Вроде исправлено (в заметках про 150+ кадров)
            }
            else
            {
                shiftX = 0;
                shiftY = 0;
            }

            changePositionEvent?.Invoke();
            return;
        }
    }
}

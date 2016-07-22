﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Linq;

using Tao.OpenGl;

using TestOpenGL.VisualObjects;
using TestOpenGL.Logic;

namespace TestOpenGL.Renders
{
    class Painter
    {
        int maxFPS;
        int pauseMillisecond;

        List<GraphicsObject> listGraphicsObject;

        Thread RenderThread;
        ManualResetEvent isNextRender = new ManualResetEvent(false);

        List<Func<List<Cell>>> shadersList;

        //TODO: вот как-то она тут не в тему, но куда её убрать?..
        Camera camera;

        public event IntEventDelegate EventFPSUpdate;
        //-------------


        public Painter(Camera camera)
        {
            maxFPS = 60;
            pauseMillisecond = 0;

            listGraphicsObject = new List<GraphicsObject>();

            shadersList = new List<Func<List<Cell>>>();

            this.camera = camera;


            SettingVisibleAreaSize();

            RenderThread = new Thread(Render);
            RenderThread.Start();
            StartRender();
        }

        public Tao.Platform.Windows.SimpleOpenGlControl GlControl
        {
            get { return Program.mainForm.GlControl; }
        }

        public Camera Camera
        {
            get { return camera; }
            set 
            {
                if (value != null)
                {
                    StopRender();
                    camera = value;
                    StartRender();
                }
            }
        }

        public int MaxFPS
        {
            get { return maxFPS; }
            set { maxFPS = value > 0 && value < 1000 ? value : 60; }
        }

        public List<Func<List<Cell>>> ShadersList
        { get { return shadersList; } }
        //=============

        public void AddGraphicsObject(GraphicsObject graphicsObject)
        {
            listGraphicsObject.Add(graphicsObject);
        }
        public void RemoveGraphicsObject(GraphicsObject graphicsObject)
        {
            listGraphicsObject.Remove(graphicsObject);
        }

        void FPSUpdate(int newValue)
        {
            EventFPSUpdate?.Invoke(newValue);
        }
        void ProcessingFPS(int renderElapsedTime) //TODO: херовое название, придумать получше.
        {
            FPSUpdate((int)(1000 / ((pauseMillisecond + renderElapsedTime) == 0 ? 1 : (pauseMillisecond + renderElapsedTime))));

            pauseMillisecond = (int)(1000 / MaxFPS - renderElapsedTime);
            pauseMillisecond = pauseMillisecond > 0 ? pauseMillisecond : 0;
        }

        public void StartRender()
        {
            isNextRender.Set();
        }
        public void StopRender()
        {
            isNextRender.Reset();
            Thread.Sleep(1);
        }

        void Render()
        {
            Stopwatch sw = new Stopwatch();

            while (true)
            {
                isNextRender.WaitOne();
                StopRender();

                VoidEventDelegate del = delegate
                {
                    sw.Start();

                    DrawFrame(listGraphicsObject); 

                    sw.Stop();
                    ProcessingFPS((int)sw.ElapsedMilliseconds);
                    sw.Reset();

                    StartRender();
                };
                Program.mainForm.Invoke(del);


                Thread.Sleep(pauseMillisecond);
            }
        }

        void DrawFrame(List<GraphicsObject> listGraphicsObject)
        {
            List<GraphicsObject> lGO = new List<GraphicsObject>();
            List<Cell> lC = new List<Cell>();

            lGO.AddRange(listGraphicsObject); //TODO: Проверить, может и без этого норм.

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            
            
            /*
            // Фоны
            //zShift = 0;
            foreach (Background b in Program.L.GetMap<Background>().GetAllVO())
                if (Analytics.IsInCamera(b.C, this.Camera))
                    listRO.AddRange(b.GetRenderObjects());//(new RenderObject(b, (int)TypeVisualObject.Background * (Program.L.LengthZ - 1)));
            // Конец фонов

            // Блоки
            //zShift += Program.L.LengthZ;
            foreach (Block b in Program.L.GetMap<Block>().GetAllVO())
                if (Analytics.IsInCamera(b.C, this.Camera))
                    //listRO.Add(new RenderObject(b, (int)TypeVisualObject.Block * (Program.L.LengthZ - 1)));
                    listRO.AddRange(b.GetRenderObjects());
            // Конец блоков

            // Сущности
            //zShift += Program.L.LengthZ;
            foreach (Being b in Program.L.GetMap<Being>().GetAllVO())
                if (b.IsSpawned)
                    if (Analytics.IsInCamera(b.C, this.Camera))
                        //listRO.Add(new RenderObject(b, (int)TypeVisualObject.Being * (Program.L.LengthZ - 1)));
                        listRO.AddRange(b.GetRenderObjects());
            // Конец сущностей

            // Декали
            //zShift += Program.L.LengthZ;
            foreach (Decal d in Program.L.GetMap<Decal>().GetAllVO())
                if (Analytics.IsInCamera(d.C, this.Camera))
                    //listRO.Add(new RenderObject(d, (int)TypeVisualObject.Decal * (Program.L.LengthZ - 1)));
                    listRO.AddRange(d.GetRenderObjects());
            // Конец декалей
            */

            foreach(GraphicsObject go in lGO)
            {
                lC.AddRange(go.GetCells());
            }

            foreach (Func<List<Cell>> func in shadersList)
                lC.AddRange(func());



            var sort = from cell in lC
                   orderby cell.GlobalDepth
                   select cell;

            foreach (Cell cell in sort)
                DrawCell(cell);

            

            // Прицел
            //DrawCell(camera.Sight.AimDecal.GraphicsObject.GetCells()[0]);

            Program.mainForm.GlControl.SwapBuffers();
        }

        void DrawCell(Cell cell)
        {
            int size = 1;
            // включаем режим текстурирования
            Gl.glEnable(Gl.GL_TEXTURE_2D);

            // включаем режим текстурирования
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, cell.Texture.textureId);

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glTexCoord2d(0.0, 0.0);
            Gl.glVertex2d((double)cell.C.X, (double)cell.C.Y);

            Gl.glTexCoord2d(0.0, 0.0 + size);
            Gl.glVertex2d((double)cell.C.X, (double)cell.C.Y + size);

            Gl.glTexCoord2d(0.0 + size, 0.0 + size);
            Gl.glVertex2d((double)cell.C.X + size, (double)cell.C.Y + size);

            Gl.glTexCoord2d(0.0 + size, 0.0);
            Gl.glVertex2d((double)cell.C.X + size, (double)cell.C.Y);

            Gl.glEnd();

            // отключаем режим текстурирования
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        public void SettingVisibleAreaSize()
        {
            Gl.glViewport(0, 0, this.GlControl.Width, this.GlControl.Height);
            // устанавливаем проекционную матрицу 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очищаем ее 
            Gl.glLoadIdentity();

            // теперь необходимо корректно настроить 2D ортогональную проекцию 
            // в зависимости от того, какая сторона больше 
            // мы немного варьируем то, как будут сконфигурированы настройки проекции 
            if (this.GlControl.Width <= this.GlControl.Height)
                Glu.gluOrtho2D(0.0, camera.Width, 0.0, camera.Height * (float)this.GlControl.Height / (float)this.GlControl.Width);
            else
                Glu.gluOrtho2D(0.0, camera.Width * (float)this.GlControl.Width / (float)this.GlControl.Height, 0.0, camera.Height);

            // переходим к объектно-видовой матрице 
            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Camera.Look();
        }

        public void ClearShadersList()
        {
            shadersList.Clear();
        }

        /*public void DrawColor(Coord C, double colorA, double colorB, double colorC)
        {
            Gl.glColor3d(colorA, colorB, colorC);

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glVertex2d((double)C.X, (double)C.Y);
            Gl.glVertex2d((double)C.X, (double)C.Y);
            Gl.glVertex2d((double)C.X, (double)C.Y);
            Gl.glVertex2d((double)C.X, (double)C.Y);

            Gl.glEnd();

            Gl.glColor3f(0, 222, 0);
            Gl.glRasterPos2f(0.4f + C.X, 0.3f + C.Y);
            //Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_TIMES_ROMAN_10, '0');
        }*/
    }
}

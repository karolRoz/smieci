﻿using GLFW; //Przestrzeń nazw biblioteki GLFW.NET. Zawiera ona bindingi do biblioteki GLFW zapewniającej możliwość tworzenia aplikacji wykorzystujacych OpenGL
using GlmSharp; //Przestrzeń nazw biblioteki GlmSharp. GlmSharp to port biblioteki GLM - OpenGL Mathematics implementującej podstawowe operacje matematyczne wykorzystywane w grafice 3D.

using Shaders; //Przestrzeń nazw pomocniczej biblioteki do wczytywania programów cieniującch
using Models;

using System;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;

namespace PMLabs
{


    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC: IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {

        static ShaderProgram sp; //Wskaźnik na obiekt reprezentujący program cieniujący
        static float speed_y; //Prędkość obrotu wokół osi Y [rad/s]
        static float speed_x; //Prędkość obrotu wokół osi X [rad/s]
		static int tex0; //Uchwyt –deklaracja globalna/pole klasy
		static int tex1; //Uchwyt –deklaracja globalna/pole klasy
        static KeyCallback kc = KeyProcessor;

        //Obsługa klawiatury - zmiana prędkości obrotu wokół poszczególnych osi w zależności od wciśniętych klawiszy
        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods) { 
            if (state==InputState.Press)
            {
                if (key == Keys.Left) speed_y = -3.14f;
                if (key == Keys.Right) speed_y =  3.14f;
                if (key == Keys.Up) speed_x = -3.14f;
                if (key == Keys.Down) speed_x = 3.14f;
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;
                if (key == Keys.Up) speed_x = 0;
                if (key == Keys.Down) speed_x = 0;
            }
        }

        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            sp = new ShaderProgram("v_shader.glsl", "f_shader.glsl"); //Wczytaj przykładowy program cieniujący
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
            GL.Enable(EnableCap.DepthTest);            
			tex0=ReadTexture("C:\\Users\\Student\\Downloads\\temat-4\\Helpers\\metal.png");
			tex1=ReadTexture("C:\\Users\\Student\\Downloads\\temat-4\\Helpers\\bricks.png");
        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {
           GL.DeleteTexture(tex0);
           GL.DeleteTexture(tex1);
        }

        //Wczytuje plik z teksturą i kopiuje teksturę do pamięciu karty graficznej. Zwraca uchwyt na wczytaną teksturę
        public static int ReadTexture(string filename)
        {
            var tex = GL.GenTexture(); //Wygeneruj nowy uchwyt
            GL.ActiveTexture(TextureUnit.Texture0); //Aktywuj zerową jednostkę teksturującą...
            GL.BindTexture(TextureTarget.Texture2D, tex);//... i powiąż ją z nowym uchwytem

            Bitmap bitmap = new Bitmap(filename); //Wczytaj plik z obrazkiem 
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits( //Utwórz bufor w pamięci z obrazkiem
              new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
              System.Drawing.Imaging.ImageLockMode.ReadOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, //Kopiuj obrazek do pamięci karty graficznej
              data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
             
            bitmap.UnlockBits(data); //Zwolnij bufor w pamięci komputera
            bitmap.Dispose();//Zwolnij pamięć zajętą przez obrazek

            //Ustaw teksturowanie z uzyciem bilinear filtering
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);


             
            //Zwróć uchwyt na wczytaną teksturę
            return tex;
        }


        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window,float angle_x,float angle_y)
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);

            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1, 1, 50); //Wylicz macierz rzutowania
            mat4 V = mat4.LookAt(new vec3(0, 0, -3), new vec3(0, 0, 0), new vec3(0, 1, 0)); //Wylicz macierz widoku

            sp.Use();//Aktywuj program cieniujący
            GL.UniformMatrix4(sp.U("P"), 1, false, P.Values1D); //Wyślij do zmiennej jednorodnej P programu cieniującego wartość zmiennej P zadeklarowanej powyżej
            GL.UniformMatrix4(sp.U("V"), 1, false, V.Values1D); //Wyślij do zmiennej jednorodnej V programu cieniującego wartość zmiennej V zadeklarowanej powyżej

            mat4 M = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x,new vec3 (1,0,0)); //Macierz modelu to iloczyun macierzy obrotu wokół osi Y i X.
            GL.UniformMatrix4(sp.U("M"), 1, false, M.Values1D); //Wyślij do zmiennej jednorodnej M programu cieniującego wartość zmiennej M zadeklarowanej powyżej

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, tex0);
			GL.Uniform1(sp.U("textureMap0"),0); //drawScene

            GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, tex1);
			GL.Uniform1(sp.U("textureMap1"),1); //drawScene

            GL.EnableVertexAttribArray(sp.A("vertex")); //Aktywuj przesyłanie danych do atrybutu vertex
            GL.EnableVertexAttribArray(sp.A("normal")); //Aktywuj przesyłanie danych do atrybutu normal
            GL.EnableVertexAttribArray(sp.A("texCoord")); //Aktywuj przesyłanie danych do atrybutu texCoord            
            GL.EnableVertexAttribArray(sp.A("color")); //Aktywuj przesyłanie danych do atrybutu color

            GL.VertexAttribPointer(sp.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.vertices); //Dane do atrybutu vertex pobieraj z tablicy MyCube.vertices
            GL.VertexAttribPointer(sp.A("normal"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.vertexNormals); //Dane do atrybutu vertex pobieraj z tablicy MyCube.vertexNormals
            GL.VertexAttribPointer(sp.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, MyTeapot.texCoords); //Dane do atrybutu vertex pobieraj z tablicy MyCube.texCoords
            GL.VertexAttribPointer(sp.A("color"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.colors); //Dane do atrybutu vertex pobieraj z tablicy MyCube.colors

            GL.DrawArrays(PrimitiveType.Triangles, 0, MyTeapot.vertexCount); //Rysuj model

            GL.DisableVertexAttribArray(sp.A("vertex")); //Wyłącz przesyłanie danych do atrybutu vertex 
            GL.DisableVertexAttribArray(sp.A("normal")); //Wyłącz przesyłanie danych do atrybutu normal 
            GL.DisableVertexAttribArray(sp.A("texCoord")); //Wyłącz przesyłanie danych do atrybutu texCoord
            GL.DisableVertexAttribArray(sp.A("color")); //Wyłącz przesyłanie danych do atrybutu color
			
			mat4 M2 = mat4.Translate(new vec3(0,1,0)) * mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x,new vec3 (1,0,0)); //Macierz modelu to iloczyun macierzy obrotu wokół osi Y i X.
            GL.UniformMatrix4(sp.U("M"), 1, false, M2.Values1D); //Wyślij do zmiennej jednorodnej M programu cieniującego wartość zmiennej M zadeklarowanej powyżej

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, tex0);
			GL.Uniform1(sp.U("textureMap0"),1); //drawScene

            GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, tex1);
			GL.Uniform1(sp.U("textureMap1"),1); //drawScene

            GL.EnableVertexAttribArray(sp.A("vertex")); //Aktywuj przesyłanie danych do atrybutu vertex
            GL.EnableVertexAttribArray(sp.A("normal")); //Aktywuj przesyłanie danych do atrybutu normal
            GL.EnableVertexAttribArray(sp.A("texCoord")); //Aktywuj przesyłanie danych do atrybutu texCoord            
            GL.EnableVertexAttribArray(sp.A("color")); //Aktywuj przesyłanie danych do atrybutu color

            GL.VertexAttribPointer(sp.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.vertices); //Dane do atrybutu vertex pobieraj z tablicy MyCube.vertices
            GL.VertexAttribPointer(sp.A("normal"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.vertexNormals); //Dane do atrybutu vertex pobieraj z tablicy MyCube.vertexNormals
            GL.VertexAttribPointer(sp.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, MyTeapot.texCoords); //Dane do atrybutu vertex pobieraj z tablicy MyCube.texCoords
            GL.VertexAttribPointer(sp.A("color"), 4, VertexAttribPointerType.Float, false, 0, MyTeapot.colors); //Dane do atrybutu vertex pobieraj z tablicy MyCube.colors

            GL.DrawArrays(PrimitiveType.Triangles, 0, MyTeapot.vertexCount); //Rysuj model

            GL.DisableVertexAttribArray(sp.A("vertex")); //Wyłącz przesyłanie danych do atrybutu vertex 
            GL.DisableVertexAttribArray(sp.A("normal")); //Wyłącz przesyłanie danych do atrybutu normal 
            GL.DisableVertexAttribArray(sp.A("texCoord")); //Wyłącz przesyłanie danych do atrybutu texCoord
            GL.DisableVertexAttribArray(sp.A("color")); //Wyłącz przesyłanie danych do atrybutu color

            //Skopiuj ukryty bufor do bufora widocznego            
            Glfw.SwapBuffers(window);
        }



        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenGL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenGL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenGL - tutaj będą realizowane polecenia OpenGL
            Glfw.SwapInterval(1); //Skopiowanie tylnego bufora na przedni ma się rozpocząć po zakończeniu aktualnego odświerzania ekranu

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window); //Wykonaj metodę inicjującą Twoje zasoby 

            float angle_x = 0; //Aktualny kąt obrotu wokół osi X
            float angle_y = 0; //Aktualny kąt obrotu wokół osi Y

            Glfw.Time = 0; //Wyzeruj licznik czasu

            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                angle_x += speed_x * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi X zgodnie z prędkością obrotu
                angle_y += speed_y * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi Y zgodnie z prędkością obrotu
                Glfw.Time = 0; //Wyzeruj licznik czasu
                DrawScene(window,angle_x,angle_y); //Wykonaj metodę odświeżającą zawartość okna

                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby

            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }
                    

    }
}
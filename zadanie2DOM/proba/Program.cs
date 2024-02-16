using GLFW; //Przestrzeń nazw biblioteki GLFW.NET. Zawiera ona bindingi do biblioteki GLFW zapewniającej możliwość tworzenia aplikacji wykorzystujacych OpenGL
using GlmSharp; //Przestrzeń nazw biblioteki GlmSharp. GlmSharp to port biblioteki GLM - OpenGL Mathematics implementującej podstawowe operacje matematyczne wykorzystywane w grafice 3D.

using Shaders; //Przestrzeń nazw pomocniczej biblioteki do wczytywania programów cieniującch

using System;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;

using Models;

namespace PMLabs
{
    

    
    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static int tex; //Uchwyt – deklaracja globalna/pole klas


        static float speed_y; //Prędkość obrotu wokół osi Y [rad/s]
        static float speed_x; //Prędkość obrotu wokół osi X [rad/s]

        static KeyCallback kc = KeyProcessor;

        public static int ReadTexture(string filename)
        {
            var tex = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            Bitmap bitmap = new Bitmap(filename);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
            new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width,
            data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            bitmap.Dispose();
            GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            return tex;
        }
        //Obsługa klawiatury - zmiana prędkości obrotu wokół poszczególnych osi w zależności od wciśniętych klawiszy
        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press)
            {
                if (key == Keys.Left) speed_y = -3.14f;
                if (key == Keys.Right) speed_y = 3.14f;
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;
            }
        }

        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            DemoShaders.InitShaders("Shaders/");
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
            GL.Enable(EnableCap.DepthTest);
            tex = ReadTexture("bricks.png");
        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {
            GL.DeleteTexture(tex);
        }

        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window, float angle_x, float angle_y)
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1, 1, 50); //Wylicz macierz rzutowania
            mat4 V = mat4.LookAt(new vec3(0, 0, -5), new vec3(0, 0, 0), new vec3(0, 1, 0)); //Wylicz macierz widoku
            mat4 M = mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0)); //Macierz modelu to iloczyun macierzy obrotu wokół osi Y i X.

            DemoShaders.spTextured.Use();
            GL.UniformMatrix4(DemoShaders.spTextured.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spTextured.U("V"), 1, false, V.Values1D);

            // Rysowanie prostopadłościanu (Dom)
            GL.UniformMatrix4(DemoShaders.spTextured.U("M"), 1, false, M.Values1D);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, Dom.vertices);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, Dom.texCoords);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.Uniform1(DemoShaders.spTextured.U("tex"), 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Dom.vertexCount);
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));

            // Rysowanie prostopadłościanu (Dom)
            GL.UniformMatrix4(DemoShaders.spTextured.U("M"), 1, false, M.Values1D);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, Dom.vertices);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, Dom.texCoords);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.Uniform1(DemoShaders.spTextured.U("tex"), 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Dom.vertexCount);
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));

            // Rysowanie stożka (Dach)
            mat4 M_Dach = mat4.Translate(new vec3(0,1.0f, 0)) * mat4.Rotate(angle_y, new vec3(0, 1, 0)) * mat4.Rotate(angle_x, new vec3(1, 0, 0));
            GL.UniformMatrix4(DemoShaders.spTextured.U("M"), 1, false, M_Dach.Values1D);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, Dach.vertices);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, Dach.texCoords);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Dach.vertexCount);
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));

            // Rysowanie prostopadłościanu (Dom)
            mat4 M_Komin = mat4.Identity;
            M_Komin = M_Dach * mat4.Translate(new vec3(0.25f, 0, 0));
            GL.UniformMatrix4(DemoShaders.spTextured.U("M"), 1, false, M_Komin.Values1D);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("vertex"), 4, VertexAttribPointerType.Float, false, 0, Komin.vertices);
            GL.EnableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));
            GL.VertexAttribPointer(DemoShaders.spTextured.A("texCoord"), 2, VertexAttribPointerType.Float, false, 0, Komin.texCoords);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.Uniform1(DemoShaders.spTextured.U("tex"), 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Dom.vertexCount);
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("vertex"));
            GL.DisableVertexAttribArray(DemoShaders.spTextured.A("texCoord"));
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
                DrawScene(window, angle_x, angle_y); //Wykonaj metodę odświeżającą zawartość okna

                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby

            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}
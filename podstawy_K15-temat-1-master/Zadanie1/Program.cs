using GLFW; //Przestrzeń nazw biblioteki GLFW.NET. Zawiera ona bindingi do biblioteki GLFW zapewniającej możliwość tworzenia aplikacji wykorzystujacych OpenGL
using GlmSharp; //Przestrzeń nazw biblioteki GlmSharp. GlmSharp to port biblioteki GLM - OpenGL Mathematics implementującej podstawowe operacje matematyczne wykorzystywane w grafice 3D.

using Shaders; //Przestrzeń nazw pomocniczej biblioteki do wczytywania programów cieniującch
using Models; //Przestrzeń nazw pomocniczej biblioteki rysującej kilka przykładowych modeli

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

        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            DemoShaders.InitShaders("Shaders\\"); //Wczytaj przykładowe programy cieniujące
        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {
            
        }

        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window,float angle)
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);

            // Tu rysujemy
            mat4 V = mat4.LookAt(
                new vec3(0.0f, 0.0f, -5.0f),
                new vec3(0.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1.0f, 1.0f, 50.0f);

            DemoShaders.spConstant.Use();//Aktywacja programu cieniującego
            GL.UniformMatrix4(DemoShaders.spConstant.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("V"), 1, false, V.Values1D);

            mat4 Mt1 = mat4.Identity;
            Mt1 = Mt1 * mat4.Translate(new vec3(1.0f, 0, 0));
            Mt1 = Mt1 * mat4.Rotate(0, new vec3(0.0f, 1.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt1.Values1D);
            Torus.torus.drawWire();


            //Skopiuj ukryty bufor do bufora widocznego            
            Glfw.SwapBuffers(window);
        }
        static float speed = 3.14f / 2; // [radiany/s]

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(1000, 1000, "OpenGL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenGL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenGL - tutaj będą realizowane polecenia OpenGL
            Glfw.SwapInterval(1); //Skopiowanie tylnego bufora na przedni ma się rozpocząć po zakończeniu aktualnego odświerzania ekranu

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window); //Wykonaj metodę inicjującą Twoje zasoby 

            float angle = 0;
            Glfw.Time = 0;

            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {

                angle += speed * (float)Glfw.Time;
                Glfw.Time = 0;

                DrawScene(window, angle); //Wykonaj metodę odświeżającą zawartość okna
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby

            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }
                    

    }
}
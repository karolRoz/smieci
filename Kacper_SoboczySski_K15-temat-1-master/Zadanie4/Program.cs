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
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }
    class Program
    {

        static float speed_y;
        static float speed_x;

        public static float far = 0;
        public static float position_x = 0;
        public static float cam_x = 0;

        static KeyCallback kc = KeyProcessor;

        static Sphere slonce=new Sphere(109.17f, 36f, 36f); //- prawdziwe wymiary slonca
        static Sphere ziemia=new Sphere(1.0f, 36f, 16f);
        static Sphere merkury = new Sphere(0.38f, 36f, 16f);
        static Sphere wenus = new Sphere(0.95f, 36f, 16f);
        static Sphere mars = new Sphere(0.54f, 36f, 16f);
        static Sphere jowisz = new Sphere(11.26f, 36f, 16f);
        static Sphere saturn = new Sphere(9.45f, 36f, 16f);
        static Sphere uran = new Sphere(4.47f, 36f, 16f);
        static Sphere neptun = new Sphere(3.89f, 36f, 16f);

        //Księżyce
        static Sphere ksiezyc_Ziemia=new Sphere(0.27f, 36f, 36f);
        static Sphere ksiezyc_Mars_Fobos = new Sphere(0.0017f, 36f, 36f);
        static Sphere ksiezyc_Mars_Deimos = new Sphere(0.0009f, 36f, 36f);


        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Repeat)
            {
                if (key == Keys.Right) position_x += 100f;
                if (key == Keys.K) position_x += 10f;
                if (key == Keys.D) position_x += 1f;
                if (key == Keys.Left) position_x -= 100f;
                if (key == Keys.H) position_x -= 10f;
                if (key == Keys.A) position_x -= 1f;


                if (key == Keys.Up) far += 1000f;
                if (key == Keys.U) far += 100f;
                if (key == Keys.W) far += 1f;
                if (key == Keys.Down) far -= 1000f;
                if (key == Keys.J) far -= 100f;
                if (key == Keys.S) far -= 1f;
            }
            if (state == InputState.Press)
            {
                if (key == Keys.Enter)
                {
                   speedSunMerkury = (6.29f / 10) / 0.24f; // zmienne szybkości dla orbit wokół słońca
                   speedSunWenus = (6.29f / 10) / 0.61f;
                   speedSunZiemia = 6.29f / 10;
                   speedSunMars = (6.29f / 10) / 1.88f;
                   speedSunJowisz = (6.29f / 10) / 11.86f;
                   speedSunSaturn = (6.29f / 10) / 29.44f;
                   speedSunUran = (6.29f / 10) / 84.07f;
                   speedSunNeptun = (6.29f / 10) / 164.87f;
                }
                if (key == Keys.P)
                {
                   speedSunMerkury = 0f; // zmienne szybkości dla orbit wokół słońca
                   speedSunWenus = 0f;
                   speedSunZiemia = 0f;
                   speedSunMars = 0f;
                   speedSunJowisz = 0f;
                   speedSunSaturn = 0f;
                   speedSunUran = 0f;
                   speedSunNeptun = 0f;
                }
            }
               
        }
        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            DemoShaders.InitShaders("Shaders\\"); //Wczytaj przykładowe programy cieniujące
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
            GL.Enable(EnableCap.DepthTest);
        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {

        }

        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window, float angle, 
            float angleMerkury, float angleWenus, float angleZiemia, float angleMars, float angleJowisz, float angleSaturn,float angleUran, float angleNeptun,
            float angleSunMerkury, float angleSunWenus, float angleSunZiemia, float angleSunMars, float angleSunJowisz, float angleSunSaturn, float angleSunUran, float angleSunNeptun,
            float angleKsiezycZiemiaObieg, float angleKsiezycMarsFobosObieg, float angleKsiezycMarsDeimosObieg,
            float position_x, float far )
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Tu rysujemy
            mat4 P = mat4.Perspective(glm.Radians(50f), 1.0f, 1.0f, 120000f);
            mat4 V = mat4.LookAt(
                new vec3(0.0f + position_x, 0.0f, -100000f+far),
                new vec3(0.0f + position_x, 0.0f, 1.0f),
                new vec3(0.0f, -1.0f, 0.0f));
            
            
            DemoShaders.spConstant.Use();//Aktywacja programu cieniującego
            GL.UniformMatrix4(DemoShaders.spConstant.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("V"), 1, false, V.Values1D);
            // Słońce
            mat4 Mt1 = mat4.Identity;
            Mt1 = Mt1 * mat4.Rotate(angle, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt1.Values1D);
            slonce.drawWire();

                    // słońce widma pozwalające innym planetom przemieszczać się w innej prędkości wokół głównego słońca
                    mat4 Mt1_2 = mat4.Identity;
                    Mt1_2 = Mt1_2 * mat4.Rotate(angleSunMerkury, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_3 = mat4.Identity;
                    Mt1_3 = Mt1_3 * mat4.Rotate(angleSunWenus, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_4 = mat4.Identity;
                    Mt1_4 = Mt1_4 * mat4.Rotate(angleSunZiemia, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_5 = mat4.Identity;
                    Mt1_5 = Mt1_5 * mat4.Rotate(angleSunMars, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_6 = mat4.Identity;
                    Mt1_6 = Mt1_6 * mat4.Rotate(angleSunJowisz, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_7 = mat4.Identity;
                    Mt1_7 = Mt1_7 * mat4.Rotate(angleSunSaturn, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_8 = mat4.Identity;
                    Mt1_8 = Mt1_8 * mat4.Rotate(angleSunUran, new vec3(0.0f, 0.0f, 1.0f));

                    mat4 Mt1_9 = mat4.Identity;
                    Mt1_9 = Mt1_9 * mat4.Rotate(angleSunNeptun, new vec3(0.0f, 0.0f, 1.0f));


            //Merkury
            mat4 Mt2 = Mt1_2;
            Mt2 = Mt2 * mat4.Translate(new vec3(109.17f + 5790.9f, 0, 0)); // 10 000km - 1f
            Mt2 = Mt2 * mat4.Rotate(angleMerkury, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt2.Values1D);
            merkury.drawWire();

            //Wenus
            mat4 Mt3 = Mt1_3;
            Mt3 = Mt3 * mat4.Translate(new vec3(109.17f + 1082.0f, 0, 0));
            Mt3 = Mt3 * mat4.Rotate(angleWenus, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt3.Values1D);
            wenus.drawWire();

            //Ziemia
            mat4 Mt4 = Mt1_4;
            Mt4 = Mt4 * mat4.Translate(new vec3(109.17f + 1495.9f, 0, 0));
            Mt4 = Mt4 * mat4.Rotate(angleZiemia, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt4.Values1D);
            ziemia.drawWire();

                //Widmo ziemia aby księżyc mógł krążyć wokół niej w odpowiednim czasie
                mat4 Mt4_1 = Mt1_4;
                Mt4_1 = Mt4_1 * mat4.Translate(new vec3(109.17f + 1495.9f, 0, 0));
                Mt4_1 = Mt4_1 * mat4.Rotate(angleKsiezycZiemiaObieg, new vec3(0.0f, 0.0f, 1.0f));

            //Mars
            mat4 Mt5 = Mt1_5;
            Mt5 = Mt5 * mat4.Translate(new vec3(109.17f + 2279.3f, 0, 0));
            Mt5 = Mt5 * mat4.Rotate(angleMars, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt5.Values1D);
            mars.drawWire();
                
                // Widmo Marsy aby księżyce mogły krążyć wokół niej w odpowiednim czasie
                mat4 Mt5_1 = Mt1_5;                                                             // fobos
                Mt5_1 = Mt5_1 * mat4.Translate(new vec3(109.17f + 2279.3f, 0, 0));
                Mt5_1 = Mt5_1 * mat4.Rotate(angleKsiezycMarsFobosObieg, new vec3(0.0f, 0.0f, 1.0f));


                mat4 Mt5_2 = Mt1_5;                                                             //deimos
                Mt5_2 = Mt5_2 * mat4.Translate(new vec3(109.17f + 2279.3f, 0, 0));
                Mt5_2 = Mt5_2 * mat4.Rotate(angleKsiezycMarsDeimosObieg, new vec3(0.0f, 0.0f, 1.0f));

            //Jowisz
            mat4 Mt6 = Mt1_6;
            Mt6 = Mt6 * mat4.Translate(new vec3(109.17f + 7784.1f, 0, 0));
            Mt6 = Mt6 * mat4.Rotate(angleJowisz, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt6.Values1D);
            jowisz.drawWire();

            //Saturn
            mat4 Mt7 = Mt1_7;
            Mt7 = Mt7 * mat4.Translate(new vec3(109.17f + 14267.2f, 0, 0));
            Mt7 = Mt7 * mat4.Rotate(angleSaturn, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt7.Values1D);
            saturn.drawWire();

            //Uran
            mat4 Mt8 = Mt1_8;
            Mt8 = Mt8 * mat4.Translate(new vec3(109.17f + 28709.7f, 0, 0));
            Mt8 = Mt8 * mat4.Rotate(angleUran, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt8.Values1D);
            uran.drawWire();

            //Neptun
            mat4 Mt9 = Mt1_9;
            Mt9 = Mt9 * mat4.Translate(new vec3(109.17f+44982.5f, 0, 0));
            Mt9 = Mt9 * mat4.Rotate(angleNeptun, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mt9.Values1D);
            neptun.drawWire();




            // księżyc Ziemi
            mat4 Kt1 = Mt4_1;
            Kt1 = Kt1 * mat4.Translate(new vec3(1f + 38.44f, 0, 0));
            Kt1 = Kt1 * mat4.Rotate(angleKsiezycZiemiaObieg, new vec3(0.0f, 0.0f, 1.0f)); 
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Kt1.Values1D);
            ksiezyc_Ziemia.drawWire();

            // Księżyce Marsa
            mat4 Kt2 = Mt5_1;                                                                         // Fobos
            Kt2 = Kt2 * mat4.Translate(new vec3(0.54f + 0.93f, 0, 0));
            Kt2 = Kt2 * mat4.Rotate(angle, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Kt2.Values1D);
            ksiezyc_Mars_Fobos.drawWire();

            mat4 Kt3 = Mt5_2;                                                                         // Deimos
            Kt3 = Kt3 * mat4.Translate(new vec3(0.54f + 2.34f, 0, 0));
            Kt3 = Kt3 * mat4.Rotate(angleKsiezycMarsDeimosObieg, new vec3(0.0f, 0.0f, 1.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Kt3.Values1D);
            ksiezyc_Mars_Deimos.drawWire();





            //Skopiuj ukryty bufor do bufora widocznego            
            Glfw.SwapBuffers(window);
        }
        
        static float speedSunMerkury = (6.29f / 10) / 0.24f; // zmienne szybkości dla orbit wokół słońca
        static float speedSunWenus = (6.29f / 10) / 0.61f;
        static float speedSunZiemia = 6.29f / 10;
        static float speedSunMars = (6.29f / 10) / 1.88f;
        static float speedSunJowisz = (6.29f / 10) / 11.86f;
        static float speedSunSaturn = (6.29f / 10) / 29.44f;
        static float speedSunUran = (6.29f / 10) / 84.07f;
        static float speedSunNeptun = (6.29f / 10) / 164.87f;

        static float speedKsiezycZiemiaObieg = (6.29f / 10) / 0.07f; // fajnie że akurat księżyce obracają się wokół własnej osi w tym samym czasie co obiegają swoje planety
        static float speedKsiezycMarsFobosObieg = (6.29f / 10) / 0.0008f;
        static float speedKsiezycMarsDeimosObieg = (6.29f / 10) / 0.003f;


        static float speed = 229.33f / 25.44f; // [radiany/s]
        static float speedMerkury = 29.33f / 56.80f; // zmienne szybkości dla obrotu wokół własnej osi dla planet
        static float speedWenus = 229.33f / 243.72f;
        static float speedZiemia = 229.33f;
        static float speedMars = 229.33f / 1.02f;
        static float speedJowisz = 229.33f / 0.41f;
        static float speedSaturn = 229.33f / 0.44f;
        static float speedUran = 229.33f / 0.72f;
        static float speedNeptun = 229.33f / 0.67f;

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(1000, 800, "OpenGL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenGL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenGL - tutaj będą realizowane polecenia OpenGL
            Glfw.SwapInterval(1); //Skopiowanie tylnego bufora na przedni ma się rozpocząć po zakończeniu aktualnego odświerzania ekranu


            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window); //Wykonaj metodę inicjującą Twoje zasoby 
         
            float angleSunMerkury = 0; // deklaracja wszystkich zmiennych dla obrotu wokół słońca
            float angleSunWenus = 0;
            float angleSunZiemia = 0;
            float angleSunMars = 0;
            float angleSunJowisz = 0;
            float angleSunSaturn = 0;
            float angleSunUran = 0;
            float angleSunNeptun = 0;

            float angleKsiezycZiemiaObieg = 0; //deklaracja wszystkich zmienny dla obrotu księżyców
            float angleKsiezycMarsFobosObieg = 0;
            float angleKsiezycMarsDeimosObieg = 0;

            float angle = 0;            // deklaracja wszystkich zmiennych dla obrotu planet
            float angleMerkury = 0;
            float angleWenus = 0;
            float angleZiemia = 0;
            float angleMars = 0;
            float angleJowisz = 0;
            float angleSaturn = 0;
            float angleUran = 0;
            float angleNeptun = 0;
            Glfw.Time = 0;
            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                angle += speed * (float)Glfw.Time;
                angleSunMerkury += speedSunMerkury * (float)Glfw.Time;
                angleSunWenus += speedSunWenus * (float)Glfw.Time;
                angleSunZiemia += speedSunZiemia * (float)Glfw.Time;
                angleSunMars += speedSunMars * (float)Glfw.Time;
                angleSunJowisz += speedSunJowisz * (float)Glfw.Time;
                angleSunSaturn += speedSunSaturn * (float)Glfw.Time;
                angleSunUran += speedSunUran * (float)Glfw.Time;
                angleSunNeptun += speedSunNeptun * (float)Glfw.Time;
                angleKsiezycZiemiaObieg += speedKsiezycZiemiaObieg * (float)Glfw.Time;
                angleKsiezycMarsFobosObieg += speedKsiezycMarsFobosObieg * (float)Glfw.Time;
                angleKsiezycMarsDeimosObieg += speedKsiezycMarsDeimosObieg * (float)Glfw.Time;

                angleMerkury += speedMerkury * (float)Glfw.Time;
                angleWenus += speedWenus * (float)Glfw.Time;
                angleZiemia += speedZiemia * (float)Glfw.Time;
                angleMars += speedMars * (float)Glfw.Time;
                angleJowisz += speedJowisz * (float)Glfw.Time;
                angleSaturn += speedSaturn * (float)Glfw.Time;
                angleUran += speedUran * (float)Glfw.Time;
                angleNeptun += speedNeptun * (float)Glfw.Time;
                Glfw.Time = 0;

                DrawScene(window, angle, 
                    angleMerkury, angleWenus, angleZiemia, angleMars, angleJowisz, angleSaturn, angleUran, angleNeptun,
                    angleSunMerkury, angleSunWenus, angleSunZiemia, angleSunMars, angleSunJowisz,angleSunSaturn,angleSunUran,angleSunNeptun, 
                    angleKsiezycZiemiaObieg, angleKsiezycMarsFobosObieg, angleKsiezycMarsDeimosObieg,
                    position_x, far); //Wykonaj metodę odświeżającą zawartość okna
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby

            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}
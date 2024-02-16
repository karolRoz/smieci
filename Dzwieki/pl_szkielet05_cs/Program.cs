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
    public class BC: IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
      

        static KeyCallback kc = KeyProcessor;

        static float angle;
        static float speed = 3.14f / 2;
        

        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods) { 
            if (state==InputState.Press)
            {
                if (key==Keys.A)
                {

                }
            }
            if (state == InputState.Release)
            {
                if (key == Keys.A)
                {

                }
            }
        }

        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            DemoShaders.InitShaders("Pomoce/");
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {
            
        }

        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window)
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);

            

            //Skopiuj ukryty bufor do bufora widocznego            
            Glfw.SwapBuffers(window);
        }

        public static byte[] LoadWave(string filename, out int channels, out int bits, out int rate)
        {
            Stream sm = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (BinaryReader reader = new BinaryReader(sm))
            {
                // Nagłówek RIFF
                string chunkID = new string(reader.ReadChars(4));
                if (chunkID != "RIFF") throw new NotSupportedException("To nie jest nawet plik RIFF");

                int chunkSize = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE") throw new NotSupportedException("To nie jest plik WAVE");

                // Format danych
                string subchunk1ID = new string(reader.ReadChars(4));
                if (subchunk1ID != "fmt ") throw new NotSupportedException("Nieznany format WAVE");

                int subchunk1IDSize = reader.ReadInt32();
                int audioFormat = reader.ReadInt16();
                int numChannels = reader.ReadInt16();
                int sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                int blockAlign = reader.ReadInt16();
                int bitsPerSample = reader.ReadInt16();

                // Dane
                string subchunk2ID = new string(reader.ReadChars(4));
                if (subchunk2ID != "data") throw new NotSupportedException("Nieznany format danych WAVE");

                int subchunk2Size = reader.ReadInt32();

                byte[] data = reader.ReadBytes(subchunk2Size); // PCM 

                // Zwrócenie wyników
                channels = numChannels;
                bits = bitsPerSample;
                rate = sampleRate;

                return data;
            }
        }

        public static void InitSound()
        {

        }

        public static void FreeSound()
        {

        }

        public static void SoundEvents()
        {

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
            InitSound();

            angle = 0;
            Glfw.Time = 0; //Wyzeruj licznik czasu

            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {

                angle += speed * (float)Glfw.Time;
                Glfw.Time = 0; //Wyzeruj licznik czasu
                DrawScene(window); //Wykonaj metodę odświeżającą zawartość okna
                SoundEvents();
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby
            FreeSound();
            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }
                    

    }
}
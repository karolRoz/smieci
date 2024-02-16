using GLFW; //Przestrzeń nazw biblioteki GLFW.NET. Zawiera ona bindingi do biblioteki GLFW zapewniającej możliwość tworzenia aplikacji wykorzystujacych OpenAL

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using OpenTK.Audio.OpenAL;

using HelpersNS;

namespace PMLabs
{


    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenAL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {


        static KeyCallback kc = KeyProcessor;

        static ALDevice device; //pole klasy
        static ALContext context; //pole klasy
        static int buf; //pole klasy
        static int source; //pole klasy
        static double A = short.MaxValue; //Amplituda
        static int fp = 44100; //Częstotliwość próbowania 44KHz
        static double op = 1.0 / fp; //Okres próbkowania: czas pomiędzy próbkami
        static int lp = 10 * fp; //Liczba próbek na 10 sekund dźwięku
        static short[] data = new short[lp];//Alokuj tablicę
        static bool graj = true;


        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press && key == Keys.Space)
            {
                int sourcestate;
                AL.GetSource(source, ALGetSourcei.SourceState, out sourcestate);
                if ((ALSourceState)sourcestate == ALSourceState.Playing)
                {
                    AL.SourcePause(source);
                    Console.WriteLine("Paused");
                }
                if ((ALSourceState)sourcestate == ALSourceState.Paused)
                {
                    AL.SourcePlay(source);
                    Console.WriteLine("Playing");
                }
            }
        }
        //Obliczanie wartości próbkowanej funkcji
        public static short signal(double t, double f, double A)
        {
            return (short)(A * Math.Sin(2.0 * Math.PI * f * t));
        }
        public static void InitSound(double hz)
        {
            device = ALC.OpenDevice(null);//Utworzenie urządzenia (w InitSound)

            //Utworzenie kontekstu (w InitSound)
            context = ALC.CreateContext(device, new ALContextAttributes());
            //Uaktywnienie kontekstu (w InitSound)
            ALC.MakeContextCurrent(context);

            buf = AL.GenBuffer(); //gdziekolwiek, np. w InitSound

            int channels;
            int bits;
            int sampleFreq;
            double f = hz; //Częstotliwość fali sinusoidalnej

            for (int x = 0; x < lp; x++)
            {
                data[x] = signal(op * x, f, A); //generuj kolejne próbki
            }
            AL.BufferData<short>(buf, ALFormat.Mono16, data, fp);
            

            source = AL.GenSource(); //gdziekolwiek, np. w InitSound

            AL.BindBufferToSource(source, buf); //gdziekolwiek, np. w InitSound

            AL.SourcePlay(source);

        }

        public static void FreeSound()
        {
            //w metodzie FreeSound
            if (context != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(context);
            }
            context = ALContext.Null;

            //w metodzie FreeSound
            if (device != ALDevice.Null)
            {
                ALC.CloseDevice(device);
            }
            device = ALDevice.Null;

            AL.DeleteBuffer(buf); //we FreeSound
            AL.DeleteSource(source); //we FreeSound
        }

        public static void SoundEvents()
        {
            AL.Source(source, ALSource3f.Position, 0.0f, 0.0f, 0.0f); //x,y,z
            AL.Source(source, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f); //x,y,z
            AL.Source(source, ALSourcef.Gain, 0.2f);//wsp. Gain, dom. 1 (bez zmian)
            AL.Source(source, ALSourcef.Pitch, 1f);//wsp. Pitch, dom. 1 (bez zmian)
            AL.Listener(ALListener3f.Position, 0.0f, 0.0f, 0.0f); //x,y,z

        }
        public static void utwor(double czas)
        {

            // --------1
            if (czas >= 0.2 && czas < 0.22 && graj==true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if(czas >=0.35 && czas <= 0.55 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 0.55 && czas < 0.57 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 0.7 && czas <= 0.9 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 0.90 && czas < 0.92 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // ---------2
            if (czas >= 1.1 && czas <= 1.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.30 && czas < 1.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.45 && czas <= 1.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.65 && czas < 1.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.8 && czas <= 2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2 && czas < 2.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // ----------3
            if (czas >= 2.15 && czas <= 2.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.3 && czas < 2.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 2.35 && czas <= 2.45 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.45 && czas < 2.47 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 2.5 && czas <= 2.6 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.6 && czas < 2.62 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // kolejna trójka 



            //-----------1

            if (czas >= 3.1 && czas <= 3.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.3 && czas < 3.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.45 && czas <= 3.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.65 && czas < 3.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.75 && czas <= 3.95 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.95 && czas < 3.97 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            // ----------2

            if (czas >= 4.1 && czas <= 4.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.3 && czas < 4.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.45 && czas <= 4.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.65 && czas < 4.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.8 && czas <= 5 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5 && czas < 5.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // ----------3

            if (czas >= 5.15 && czas <= 5.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.3 && czas < 5.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.35 && czas <= 5.45 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.45 && czas < 5.47 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.5 && czas <= 5.6 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.6 && czas < 5.62 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }



            // połowa utworu za nami - teraz druga połowa


            /* Tu jest kolejna druga połowa ale trzeba się zabrać za inny utwór
            if (czas >= 6.1 && czas <= 6.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 6.3 && czas < 6.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 6.45 && czas <= 6.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 6.65 && czas < 6.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 6.8 && czas <= 7 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 7 && czas < 7.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // ---------2
            if (czas >= 7.15 && czas <= 7.35 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.30 && czas < 1.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.45 && czas <= 1.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.65 && czas < 1.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.8 && czas <= 2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2 && czas < 2.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // ----------3
            if (czas >= 2.15 && czas <= 2.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.3 && czas < 2.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 2.35 && czas <= 2.45 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.45 && czas < 2.47 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 2.5 && czas <= 2.6 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.6 && czas < 2.62 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // kolejna trójka 



            //-----------1

            if (czas >= 3.1 && czas <= 3.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.3 && czas < 3.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.45 && czas <= 3.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.65 && czas < 3.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.75 && czas <= 3.95 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.95 && czas < 3.97 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            // ----------2

            if (czas >= 4.1 && czas <= 4.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.3 && czas < 4.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.45 && czas <= 4.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.65 && czas < 4.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.8 && czas <= 5 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5 && czas < 5.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // ----------3

            if (czas >= 5.15 && czas <= 5.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.3 && czas < 5.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.35 && czas <= 5.45 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.45 && czas < 5.47 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.5 && czas <= 5.6 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.6 && czas < 5.62 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }*/







        }

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenAL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenAL - tutaj będą realizowane polecenia OpenAL
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury

            InitSound(698.5);

            double startTime = Glfw.Time;
            double currentTime;
            double elapsedTime;

            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                currentTime = Glfw.Time;
                elapsedTime = currentTime - startTime;
                
                //double xd = Glfw.Time = 1.5;

                SoundEvents();
                utwor(elapsedTime);

                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeSound();
            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}
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
        static ALDevice device2; //pole klasy
        static ALContext context2; //pole klasy
        static int buf; //pole klasy
        static int buf2;
        static int source; //pole klasy
        static int source2;
        static double A = short.MaxValue; //Amplituda
        static int fp = 44100; //Częstotliwość próbowania 44KHz
        static double op = 1.0 / fp; //Okres próbkowania: czas pomiędzy próbkami
        static int lp = 10 * fp; //Liczba próbek na 10 sekund dźwięku
        static short[] data = new short[lp];//Alokuj tablicę
        static bool graj = true;
        static bool graj2 = true;
        static double pauza;
        static float poz_x=0;


        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            if (state == InputState.Press && key == Keys.Left)
            {
                poz_x += 1f;
            }
            if (state == InputState.Press && key == Keys.Right)
            {
                poz_x -= 1f;
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
        public static void InitSound2(double hz2)
        {

            buf2 = AL.GenBuffer(); //gdziekolwiek, np. w InitSound

            int channels;
            int bits;
            int sampleFreq;
            double f2 = hz2; //Częstotliwość fali sinusoidalnej

            for (int x = 0; x < lp; x++)
            {
                data[x] = signal(op * x, f2, A); //generuj kolejne próbki
            }
            AL.BufferData<short>(buf2, ALFormat.Mono16, data, fp);


            source2 = AL.GenSource(); //gdziekolwiek, np. w InitSound

            AL.BindBufferToSource(source2, buf2); //gdziekolwiek, np. w InitSound

            AL.SourcePlay(source2);
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

            //w metodzie FreeSound
            if (context2 != ALContext.Null)
            {
                ALC.MakeContextCurrent(ALContext.Null);
                ALC.DestroyContext(context2);
            }
            context2 = ALContext.Null;

            //w metodzie FreeSound
            if (device2 != ALDevice.Null)
            {
                ALC.CloseDevice(device2);
            }
            device2 = ALDevice.Null;

            AL.DeleteBuffer(buf2); //we FreeSound
            AL.DeleteSource(source2); //we FreeSound
        }

        public static void SoundEvents()
        {
            AL.Source(source, ALSourcef.Gain, 0.2f);//wsp. Gain, dom. 1 (bez zmian)
            AL.Source(source, ALSourcef.Pitch, 1f);//wsp. Pitch, dom. 1 (bez zmian)
            AL.Source(source2, ALSourcef.Gain, 0.05f);//wsp. Gain, dom. 1 (bez zmian)
            AL.Source(source2, ALSourcef.Pitch, 0.5f);//wsp. Pitch, dom. 1 (bez zmian)
            AL.Listener(ALListener3f.Position, poz_x, 0.0f, 0.0f); //x,y,z

        }
        public static void utwor(double czas)
        {
            // początkowy rytm
            //          1 powtórzenie

            if (czas >= 0.5 && czas < 0.52 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 0.6 && czas <= 1 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1 && czas < 1.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.1 && czas <= 1.2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.2 && czas < 1.22 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.25 && czas <= 1.4 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.4 && czas < 1.42 && graj == true)
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
            if (czas >= 1.7 && czas <= 1.9 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 1.9 && czas < 1.92 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 1.95 && czas <= 2.4 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 2.4 && czas < 2.42 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            //          2 powtórzenie

            if (czas >= 2.45 && czas <= 2.95 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3 && czas < 3.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.1 && czas <= 3.5 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.5 && czas < 3.52 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.6 && czas <= 3.7 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.7 && czas < 3.72 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.75 && czas <= 3.9 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 3.9 && czas < 3.92 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 3.95 && czas <= 4.1 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.1 && czas < 4.12 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.15 && czas <= 4.35 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.35 && czas < 4.37 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 4.4 && czas <= 4.85 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 4.85 && czas < 4.87 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // Nowy rytm

            // 1 powtórzenie

            if (czas >= 4.9 && czas <= 5 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5 && czas < 5.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.05 && czas <= 5.25 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.25 && czas < 5.27 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.3 && czas <= 5.5 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 5.5 && czas < 5.52 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 5.55 && czas <= 6 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 6 && czas < 6.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // 2 powtórzenie
            if (czas >= 6.05 && czas <= 6.15 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 6.15 && czas < 6.17 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 6.2 && czas <= 6.4 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 6.4 && czas < 6.42 && graj == true)
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
            if (czas >= 6.7 && czas <= 7.15 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 7.15 && czas < 7.17 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // Długa nuta
            if (czas >= 7.2 && czas <= 9.2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 9.2 && czas < 9.22 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // Kolejny rytm 4 szybkie
            if (czas >= 9.3 && czas <= 9.45 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 9.45 && czas < 9.47 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 9.48 && czas <= 9.63 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 9.63 && czas < 9.65 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 9.66 && czas <= 9.81 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 9.81 && czas < 9.83 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 9.84 && czas <= 9.99 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 9.99 && czas < 10.01 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // Długa nuta
            if (czas >= 10.02 && czas <= 11.52 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 11.52 && czas < 11.54 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            
            //Nowy rytm
            // 1 powtórzenie
            
            //  4 szybkie (15)
            if (czas >= 11.6 && czas <= 11.75 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 11.75 && czas < 11.77 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 11.78 && czas <= 11.93 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 11.93 && czas < 11.95 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 11.96 && czas <= 12.11 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 12.11 && czas < 12.13 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 12.14 && czas <= 12.29 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 12.29 && czas < 12.31 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

 
            if (czas >= 12.35 && czas <= 12.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 12.65 && czas < 12.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 12.7 && czas <= 12.85 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 12.85 && czas < 12.87 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 12.9 && czas <= 13.2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 13.2 && czas < 13.22 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 13.25 && czas <= 13.4 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 13.4 && czas < 13.42 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // średnia nuta
            if (czas >= 13.45 && czas <= 14.1 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 14.1 && czas < 14.12 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // 2 powótrzenie
            //  4 szybkie (15)
            if (czas >= 14.15 && czas <= 14.3 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 14.3 && czas < 14.32 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 14.33 && czas <= 14.48 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 14.48 && czas < 14.5 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 14.51 && czas <= 14.66 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 14.66 && czas < 14.68 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 14.69 && czas <= 14.84 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 14.84 && czas < 14.86 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            if (czas >= 14.9 && czas <= 15.2 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 15.2 && czas < 15.22 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 15.25 && czas <= 15.4 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 15.4 && czas < 15.42 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 15.45 && czas <= 15.75 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 15.75 && czas < 15.77 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 15.8 && czas <= 15.95 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 15.95 && czas < 15.97 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }

            // średnia nuta
            if (czas >= 16 && czas <= 16.65 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 16.65 && czas < 16.67 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // koniec
            if (czas >= 16.7 && czas <= 17 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 17 && czas < 17.02 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }
            if (czas >= 17.05 && czas <= 17.35 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 17.35 && czas < 17.37 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }


            // długa nuta
            if (czas >= 17.4 && czas <= 18.9 && graj == false)
            {
                AL.SourcePlay(source);
                graj = true;
            }
            if (czas >= 18.9 && czas < 18.92 && graj == true)
            {
                AL.SourceStop(source);
                graj = false;
            }








        }
        public static void nutydodatkowe(double czas)
        {

            // nuta wspomagająca ( ta dodatkowa  w tle )
            if (czas >= 2.3 && czas <= 2.32 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 2.45 && czas <= 4.7 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 4.7 && czas <= 4.72 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 4.9 && czas <= 7.2 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 7.2 && czas <= 7.22 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 7.3 && czas <= 9.8 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 9.8 && czas <= 9.82 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 10.02 && czas <= 12.2 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 12.2 && czas <= 12.22 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 12.35 && czas <= 14.8 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 14.8 && czas <= 14.82 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 14.9 && czas <= 17.3 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 17.3 && czas <= 17.32 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }
            if (czas >= 17.4 && czas <= 18.9 && graj2 == false)
            {
                AL.SourcePlay(source2);
                graj2 = true;
            }
            if (czas >= 18.9 && czas <= 18.92 && graj2 == true)
            {
                AL.SourceStop(source2);
                graj2 = false;
            }


        }

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenAL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenAL - tutaj będą realizowane polecenia OpenAL
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury

            InitSound(698.5);
            InitSound2(261.6);
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
                nutydodatkowe(elapsedTime);

                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeSound();
            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}
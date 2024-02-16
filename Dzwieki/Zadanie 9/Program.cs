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


        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
        }

        public static void InitSound()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device, new ALContextAttributes());
            ALC.MakeContextCurrent(context);

            buf = AL.GenBuffer(); //gdziekolwiek, np. w InitSound

            AL.SourcePlay(source);
        }

        public static void FreeSound()
        {
            AL.DeleteSource(source); //we FreeSound
            AL.DeleteBuffer(buf); //we FreeSound

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
        }

        public static void SoundEvents()
        {

        }

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenAL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenAL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenAL - tutaj będą realizowane polecenia OpenAL
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury

            InitSound();


            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                SoundEvents();
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }


            FreeSound();
            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }


    }
}
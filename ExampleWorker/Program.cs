using MmoWorker;

namespace ExampleWorker
{
    class Program
    {
        private static MmoClient? _mmoClient;

        static void Main(string[] args)
        {
            _mmoClient = new MmoClient();
            _mmoClient.Connect("127.0.0.1", 1338);

            bool loop = true;
            while (loop)
            {
                try
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.S)
                    {
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        loop = false;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

            _mmoClient.Stop();
        }
    }
}
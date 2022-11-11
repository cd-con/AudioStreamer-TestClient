using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("AudioStreamer тестовый клиент\n<!>Клиент работает в режиме сырых комманд<!>");
        Console.WriteLine("Используем стандартные параметры подключения (без шифрования)");
        IPEndPoint ipe = new(IPAddress.Loopback, 8080);
        using (Socket socket = new Socket(new IPEndPoint(IPAddress.Loopback, 8080).AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Connect(ipe);
            // Набор тестов

            // Тест 1. Пинг-понг
            Console.WriteLine("Тест 1. Пинг-понг");
            DateTime dt = DateTime.Now;
            Send(socket, "ping");
            Console.WriteLine($"Исполнение запроса заняло >> {DateTime.Now - dt}");


            // Тест 2. Получить данные по ID и собрать их в один файл.
            Console.WriteLine("Тест2. Получить данные по ID и собрать их в один файл.");
            Send(socket, "get_song_by_id 0 hq");

            using (FileStream f = File.Open("recived_file.wav", FileMode.OpenOrCreate))
            {
                while (true)
                {
                    (byte[] inputBytes, int bufferLength) = ReadRaw(socket);
                    string friendlyMessage = Encoding.Unicode.GetString(inputBytes, 0, bufferLength);
                    Console.Write("Message ");
                    Console.WriteLine(friendlyMessage);
                    if (friendlyMessage == "transmission_end")
                    {
                        break;
                    }
                    else
                    {
                        foreach (byte bufferElement in inputBytes)
                        {
                            f.WriteByte(bufferElement);
                        }
                    }
                }
            }
        }
    }

    private static string Read(Socket socket)
    {
        int bytes;
        StringBuilder message = new();
        byte[] data = new byte[256];
        bytes = socket.Receive(data);
        message.Append(Encoding.Unicode.GetString(data, 0, bytes));
        return message.ToString();
    }

    private static (byte[],int) ReadRaw(Socket socket)
    {
        byte[] data = new byte[256];
        int len = socket.Receive(data);
        return (data,len);
    }

    private static void Send(Socket handler, string message)
    {
        StringBuilder builder = new StringBuilder();
        int bytes = 0;
        byte[] data = Encoding.Unicode.GetBytes(message);
        handler.Send(data);
    }
}
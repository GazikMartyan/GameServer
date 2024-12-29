using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static TcpListener server = null!;
    private const int Port = 5000; // Порт для подключения клиентов

    static async Task Main(string[] args)
    {
        Console.WriteLine("Запуск сервера...");
        server = new TcpListener(IPAddress.Any, Port);
        server.Start();
        Console.WriteLine($"Сервер запущен на порту {Port}");

        while (true)
        {
            Console.WriteLine("Ожидание подключения клиентов...");
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Клиент подключен!");

            // Обработка клиента в отдельной задаче
            _ = HandleClientAsync(client);
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        var stream = client.GetStream();
        var buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Клиент отключился

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Сообщение от клиента: {message}");

                // Отправка ответа клиенту
                string response = $"Сервер получил: {message}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                break;
            }
        }

        Console.WriteLine("Клиент отключился.");
        client.Close();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutoNewFileManager
{
    public class FileServer
    {
        public static event Action<string> OnMessage;
        private static string directoryPath = @"D:\CompartirArchivos"; // Ruta de la carpeta compartida
        private static int port = 61116;  // Puerto en el que el servidor escucha


        public static async Task StartServerAsync(string FolderToUpload)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            OnMessage?.Invoke($"Servidor TCP escuchando en el puerto {port}");

            while (true)
            {
                // Espera una conexión de forma asíncrona
                TcpClient client = await listener.AcceptTcpClientAsync();
                var clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                string clientIp = clientEndPoint?.Address.ToString();
                OnMessage?.Invoke($"Se conectó el cliente {clientEndPoint?.Address.ToString()}");

                // Maneja la conexión del cliente en un hilo separado
                await HandleClient(client, FolderToUpload);
            }
        }

        private static async Task HandleClient(TcpClient client, string Folder)
        {
            OnMessage?.Invoke("HandleClient ha sido llamado.");
            using (client)
            {
                NetworkStream stream = client.GetStream();
                string[] files = Directory.GetFiles(Folder);

                // 1. Calcular el peso total de los archivos
                long totalSize = files.Sum(filePath => new FileInfo(filePath).Length);

                // 2. Enviar el tamaño total al cliente
                byte[] totalSizeBytes = BitConverter.GetBytes(totalSize);
                await stream.WriteAsync(totalSizeBytes, 0, totalSizeBytes.Length);

                // 3. Enviar cada archivo al cliente
                foreach (string filePath in files)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    OnMessage?.Invoke($"Enviando archivo: {fileInfo.Name}");

                    // 1. Enviar el nombre del archivo
                    byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileInfo.Name);
                    byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                    await stream.WriteAsync(fileNameLengthBytes, 0, fileNameLengthBytes.Length);
                    await stream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

                    // 2. Enviar el contenido del archivo por fragmentos
                    byte[] fileLengthBytes = BitConverter.GetBytes(fileInfo.Length);
                    await stream.WriteAsync(fileLengthBytes, 0, fileLengthBytes.Length);

                    // Leer el archivo en fragmentos y enviarlo
                    byte[] buffer = new byte[4096]; // Tamaño del buffer
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        int bytesRead;
                        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await stream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                }

                OnMessage?.Invoke("Todos los archivos han sido enviados.");
                string endMessage = "FIN";
                byte[] endMessageBytes = Encoding.UTF8.GetBytes(endMessage);
                await stream.WriteAsync(BitConverter.GetBytes(endMessageBytes.Length), 0, sizeof(int));
                await stream.WriteAsync(endMessageBytes, 0, endMessageBytes.Length);
            }
        }
    }
}

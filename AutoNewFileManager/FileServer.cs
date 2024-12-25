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
        private static int port = 8080;  // Puerto en el que el servidor escucha


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

        private static async Task HandleClient(TcpClient client, string folder)
        {
            OnMessage?.Invoke("HandleClient ha sido llamado.");
            using (client)
            {
                NetworkStream stream = client.GetStream();

                // Obtener todas las entradas de directorio recursivamente
                IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(folder, "*", SearchOption.AllDirectories);

                // Enviar el número total de entradas (archivos + carpetas)
                int totalEntries = entries.Count();
                byte[] totalEntriesBytes = BitConverter.GetBytes(totalEntries);
                await stream.WriteAsync(totalEntriesBytes, 0, totalEntriesBytes.Length);

                foreach (string entry in entries)
                {
                    FileInfo fileInfo = new FileInfo(entry);
                    bool isDirectory = Directory.Exists(entry);

                    // Enviar si es una carpeta o un archivo
                    byte[] isDirectoryBytes = BitConverter.GetBytes(isDirectory);
                    await stream.WriteAsync(isDirectoryBytes, 0, isDirectoryBytes.Length);

                    // Enviar el nombre relativo (desde la raíz de la carpeta original)
                    string relativePath = GetRelativePath(folder, entry);
                    byte[] relativePathBytes = Encoding.UTF8.GetBytes(relativePath);
                    byte[] relativePathLengthBytes = BitConverter.GetBytes(relativePathBytes.Length);
                    await stream.WriteAsync(relativePathLengthBytes, 0, relativePathLengthBytes.Length);
                    await stream.WriteAsync(relativePathBytes, 0, relativePathBytes.Length);

                    if (!isDirectory) // Si es un archivo, enviar el contenido
                    {
                        byte[] fileLengthBytes = BitConverter.GetBytes(fileInfo.Length);
                        await stream.WriteAsync(fileLengthBytes, 0, fileLengthBytes.Length);

                        byte[] buffer = new byte[4096];
                        using (FileStream fileStream = new FileStream(entry, FileMode.Open, FileAccess.Read))
                        {
                            int bytesRead;
                            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await stream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                OnMessage?.Invoke("Todos los archivos y carpetas han sido enviados.");
            }
        }

        public static string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

    }
}

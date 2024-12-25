using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoNewFileManagerClient
{
    public class FileClient
    {
        private static int port = 8080;
        public static event Action<string> OnMessage;
        public static event Action<bool> AlternateBlocking;
        public static event Action<long> OnProgressChanged; // Nuevo evento para informar del progreso
        public static event Action<long> OnTotalSizeReceived; // Evento para enviar el tamaño total de los archivos

        public static async Task Connect(string ip, string selectedFolder)
        {
            await Task.Run(async () =>
            {
                OnMessage?.Invoke("Intentando conectar...");
                try
                {
                    using (TcpClient client = new TcpClient(ip, port))
                    using (NetworkStream stream = client.GetStream())
                    {

                        // Leer el tamaño total de todos los archivos a enviar
                        byte[] totalSizeBytes = new byte[sizeof(long)];
                        await stream.ReadAsync(totalSizeBytes, 0, totalSizeBytes.Length);
                        long totalSize = BitConverter.ToInt64(totalSizeBytes, 0);

                        // Emitir el tamaño total al formulario
                        OnTotalSizeReceived?.Invoke(totalSize);

                        long totalBytesReceived = 0;

                        while (true)
                        {
                            AlternateBlocking?.Invoke(true);

                            // Leer si es carpeta o archivo
                            byte[] isDirectoryBytes = new byte[sizeof(bool)];
                            int bytesRead = await stream.ReadAsync(isDirectoryBytes, 0, isDirectoryBytes.Length);
                            if (bytesRead == 0) break;
                            bool isDirectory = BitConverter.ToBoolean(isDirectoryBytes, 0);

                            // Leer la longitud del nombre relativo
                            byte[] relativePathLengthBytes = new byte[sizeof(int)];
                            await stream.ReadAsync(relativePathLengthBytes, 0, relativePathLengthBytes.Length);
                            int relativePathLength = BitConverter.ToInt32(relativePathLengthBytes, 0);

                            // Leer el nombre relativo
                            byte[] relativePathBytes = new byte[relativePathLength];
                            await stream.ReadAsync(relativePathBytes, 0, relativePathLength);
                            string relativePath = Encoding.UTF8.GetString(relativePathBytes);

                            // Si es carpeta, crearla
                            string fullPath = Path.Combine(selectedFolder, relativePath);
                            if (isDirectory)
                            {
                                Directory.CreateDirectory(fullPath);
                                OnMessage?.Invoke($"Carpeta creada: {relativePath}");
                            }
                            else // Si es archivo, leer su contenido
                            {
                                // Leer el tamaño del archivo
                                byte[] fileLengthBytes = new byte[sizeof(long)];
                                await stream.ReadAsync(fileLengthBytes, 0, fileLengthBytes.Length);
                                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                                // Leer el contenido del archivo en fragmentos
                                byte[] buffer = new byte[65536];
                                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                                {
                                    long totalBytesRead = 0;
                                    while (totalBytesRead < fileLength)
                                    {
                                        int toRead = (int)Math.Min(buffer.Length, fileLength - totalBytesRead);
                                        int fragmentBytesRead = await stream.ReadAsync(buffer, 0, toRead);
                                        if (fragmentBytesRead == 0) break;

                                        await fileStream.WriteAsync(buffer, 0, fragmentBytesRead);
                                        totalBytesRead += fragmentBytesRead;
                                    }
                                }

                                OnMessage?.Invoke($"Archivo descargado: {relativePath}");
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    OnMessage?.Invoke($"Hubo un error en la conexión. Vuelva a intentar.");
                }


            });
        }
    }
}

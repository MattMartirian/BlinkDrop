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
        private static int port = 61116;
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
                            // 1. Leer la longitud del nombre del archivo
                            byte[] fileNameLengthBytes = new byte[sizeof(int)];
                            int bytesRead = await stream.ReadAsync(fileNameLengthBytes, 0, fileNameLengthBytes.Length);
                            if (bytesRead == 0) break; // No hay más archivos

                            int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

                            // 2. Leer el nombre del archivo
                            byte[] fileNameBytes = new byte[fileNameLength];
                            await stream.ReadAsync(fileNameBytes, 0, fileNameLength);
                            string fileName = Encoding.UTF8.GetString(fileNameBytes);


                            OnMessage?.Invoke($"Descargando {fileName}...");

                            // Si el nombre del archivo es "FIN", terminamos
                            if (fileName == "FIN")
                            {
                                OnMessage?.Invoke($"Transferencia completa! Conexión cerrada.");
                                AlternateBlocking?.Invoke(false);
                                break;
                            }

                            // 3. Leer la longitud del archivo
                            byte[] fileLengthBytes = new byte[sizeof(long)];
                            await stream.ReadAsync(fileLengthBytes, 0, fileLengthBytes.Length);
                            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                            // 4. Leer el contenido del archivo por fragmentos
                            string fullPath = Path.Combine(selectedFolder, fileName);
                            byte[] buffer = new byte[65536]; // Tamaño del buffer

                            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, useAsync: true))
                            {
                                long totalBytesRead = 0;
                                while (totalBytesRead < fileLength)
                                {
                                    int toRead = (int)Math.Min(buffer.Length, fileLength - totalBytesRead);
                                    int fragmentBytesRead = await stream.ReadAsync(buffer, 0, toRead);
                                    if (fragmentBytesRead == 0) break;

                                    await fileStream.WriteAsync(buffer, 0, fragmentBytesRead);
                                    totalBytesRead += fragmentBytesRead;
                                    totalBytesReceived += fragmentBytesRead;

                                    // Emitir el progreso
                                    OnProgressChanged?.Invoke(totalBytesReceived);
                                }
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

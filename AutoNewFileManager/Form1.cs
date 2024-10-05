using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoNewFileManager
{
    public partial class Form1 : Form
    {
        private string selectedFolder;
        private bool serverAbierto = false;

        public Form1()
        {
            InitializeComponent();
            FileServer.OnMessage += ShowMessage;
            CenterToScreen();
            textBox1.Text = new System.Net.WebClient().DownloadString("https://api.ipify.org").Trim();
        }

        private void ShowMessage(string message)
        {
            listBox1.Items.Add(message);
        }

        private void carpetaATransmitirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFolder = folderDialog.SelectedPath;
                    ShowMessage("Carpeta a transmitir seleccionada: " + "'"+selectedFolder+"'");
                }
            }
        }

        private async void abrirServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(selectedFolder) && !serverAbierto)
            {
                await FileServer.StartServerAsync(selectedFolder);
                serverAbierto = true; // Asegúrate de marcar el servidor como abierto.
            }
            else
            {
                ShowMessage("El servidor no se ha iniciado porque el directorio no es válido");
            }
        }
    }
}

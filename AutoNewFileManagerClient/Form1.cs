using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace AutoNewFileManagerClient
{
    public partial class Form1 : Form
    {

        string selectedFolder = "";
        public Form1()
        {
            InitializeComponent();
            FileClient.OnMessage += changeLabel;
            FileClient.AlternateBlocking += AlternateStatus;
            FileClient.OnTotalSizeReceived += OnTotalSizeReceived;
            FileClient.OnProgressChanged += OnProgressChanged;
            CenterToScreen();
            jsonManager.LoadFromJsonAndAddToMenu(carpetasToolStripMenuItem, clickAnyItem);
            label1.Text = "";

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (IsValidString(textBox1.Text))
            {
                await FileClient.Connect(textBox1.Text, selectedFolder);
            }
        }

        private void AlternateStatus(bool option)
        {
            if (button1.InvokeRequired)
            {
                if (option == true)
                {
                    textBox1.Invoke(new Action(() => textBox1.Enabled = false));
                    button1.Invoke(new Action(() => button1.Enabled = false));
                    button2.Invoke(new Action(() => button2.Enabled = false));
                }
                else
                {
                    textBox1.Invoke(new Action(() => textBox1.Enabled = true));
                    button1.Invoke(new Action(() => button1.Enabled = true));
                    button2.Invoke(new Action(() => button2.Enabled = true));
                }
            }
            else
            {
                if (option == true)
                {
                    textBox1.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                }
                else
                {
                    textBox1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }

        private void OnTotalSizeReceived(long totalSize)
        {
            // Utiliza Invoke para actualizar la UI desde un hilo secundario
            Invoke(new Action(() =>
            {
                // Configurar el máximo de la barra de progreso según el tamaño total
                progressBar1.Maximum = (int)totalSize;
                progressBar1.Value = 0;  // Reiniciar la barra de progreso
            }));
        }

        // Método para manejar el evento que actualiza el progreso
        private void OnProgressChanged(long totalBytesReceived)
        {
            // Utiliza Invoke para actualizar la UI desde un hilo secundario
            Invoke(new Action(() =>
            {
                // Actualiza la barra de progreso con los bytes recibidos
                progressBar1.Value = (int)totalBytesReceived;
            }));
        }


        private void clickAnyItem(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                FormatText(menuItem.Tag.ToString()); 
            }
        }

        private void changeLabel(string t)
        {
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(() => label1.Text = t));
            }
            else
            {
                label1.Text = t;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.ValidateNames = false;  // Permitir seleccionar nombres no válidos (carpetas)
                openFileDialog.CheckFileExists = false;  // No validar si existe un archivo
                openFileDialog.CheckPathExists = true;   // Validar que la ruta de la carpeta existe
                openFileDialog.FileName = "Seleccionar carpeta";  // Nombre ficticio para simular selección de carpeta

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Retornar solo la ruta de la carpeta seleccionada
                    FormatText(System.IO.Path.GetDirectoryName(openFileDialog.FileName));

                    DialogResult newF = MessageBox.Show("¿Desea guardar la carpeta elegida como una carpeta por defecto? Podrá elegirla rapidamente desde el menú \"Carpetas\"", "Nueva carpeta predefinida", MessageBoxButtons.YesNo);

                    if (newF == DialogResult.Yes)
                    {
                        string name = Interaction.InputBox("Escriba el nombre para la selección de la nueva carpeta predefinida: ", "Asignando alias para nueva carpeta", "Nueva Carpeta");
                        jsonManager.SaveToJson(name, selectedFolder);
                        jsonManager.LoadFromJsonAndAddToMenu(carpetasToolStripMenuItem, clickAnyItem);
                    }
                }
            }
        }

        private void FormatText(string t)
        {
            selectedFolder = t;
            textBox2.Text = selectedFolder + "       ";
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.ScrollToCaret();  // Asegura que el desplazamiento ocurra
        }

        private bool IsValidString(string str)
        {
            // Verificar si es null
            if (str == null)
            {
                changeLabel("Debe ingresar una IP.");
                return false;
            }

            // Verificar si es vacío o solo espacios
            if (string.IsNullOrWhiteSpace(str))
            {
                changeLabel("Debe ingresar una IP.");
                return false;
            }

            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            // Verificar si NO contiene letras
            if (!Regex.IsMatch(str, pattern))
            { 
                changeLabel("La IP ingresada no representa una dirección válida.");
                return false; // Si hay algún carácter que no es letra, es inválido
            }

            if(string.IsNullOrWhiteSpace(textBox2.Text) || textBox2.Text == null)
            {
                changeLabel("Debe haber una carpeta valida para guardar los datos.");
                return false; 
            }

            return true; // El string es válido si pasa todas las verificaciones
        }
    }
}

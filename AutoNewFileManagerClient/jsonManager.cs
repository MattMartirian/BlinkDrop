using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoNewFileManagerClient
{
    internal class jsonManager
    {
        private static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoFileManagerConfig");
        private static string jsonFilePath = Path.Combine(appDataPath, "defaultFolders.json");

        public static void CheckFiles()
        {
            if (!Directory.Exists(appDataPath))
            {
                CreateFolder();
            }

            if (!File.Exists(jsonFilePath))
            {
                CreateJsonFile();
            }
        }

        public static void CreateFolder()
        {
            Directory.CreateDirectory(appDataPath);
        }

        public static void CreateJsonFile()
        {
            var emptyDictionary = new Dictionary<string, string>();
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(emptyDictionary, Formatting.Indented));

        }

        public static void SaveToJson(string key, string value)
        {
            try
            {
                CheckFiles();
                // Cargar el contenido actual del archivo JSON
                var existingData = LoadJsonData();

                // Añadir el nuevo elemento
                existingData[key] = value; // Sobrescribir si la clave ya existe

                // Guardar el diccionario actualizado en el archivo JSON
                string jsonString = JsonConvert.SerializeObject(existingData, Formatting.Indented);
                File.WriteAllText(jsonFilePath, jsonString);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar los datos en el archivo JSON: {ex.Message}");
            }
        }

        // 4. Cargar el archivo JSON y añadir ToolStripMenuItems
        public static void LoadFromJsonAndAddToMenu(ToolStripMenuItem parentMenuItem, EventHandler clickHandler)
        {
            try
            {
                CheckFiles();
                // Cargar el diccionario desde el archivo JSON
                var folderDictionary = LoadJsonData();
                parentMenuItem.DropDownItems.Clear();
                // Agregar cada clave como un ToolStripMenuItem hijo del parentMenuItem
                foreach (var entry in folderDictionary)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(entry.Key);
                    menuItem.Tag = entry.Value;  // Guardar la ruta en la propiedad Tag
                    menuItem.ForeColor = Color.Blue;
                    menuItem.Click += clickHandler;

                    // Añadir el nuevo ToolStripMenuItem al menú padre
                    parentMenuItem.DropDownItems.Add(menuItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos desde el archivo JSON: {ex.Message}");
            }
        }

        // Función auxiliar para cargar los datos del archivo JSON
        private static Dictionary<string, string> LoadJsonData()
        {
            try
            {
                // Leer el archivo JSON y deserializarlo en un diccionario
                string jsonString = File.ReadAllText(jsonFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo JSON: {ex.Message}");
                return new Dictionary<string, string>(); // Devolver un diccionario vacío si hay un error
            }
        }
    }
}

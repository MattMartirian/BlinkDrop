namespace AutoNewFileManager
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.carpetaATransmitirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 27);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox1.Size = new System.Drawing.Size(441, 173);
            this.listBox1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configuraciónToolStripMenuItem,
            this.abrirServerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(459, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // configuraciónToolStripMenuItem
            // 
            this.configuraciónToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.carpetaATransmitirToolStripMenuItem});
            this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.configuraciónToolStripMenuItem.Text = "Configuración";
            // 
            // carpetaATransmitirToolStripMenuItem
            // 
            this.carpetaATransmitirToolStripMenuItem.Name = "carpetaATransmitirToolStripMenuItem";
            this.carpetaATransmitirToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.carpetaATransmitirToolStripMenuItem.Text = "Carpeta a transmitir";
            this.carpetaATransmitirToolStripMenuItem.Click += new System.EventHandler(this.carpetaATransmitirToolStripMenuItem_Click);
            // 
            // abrirServerToolStripMenuItem
            // 
            this.abrirServerToolStripMenuItem.Name = "abrirServerToolStripMenuItem";
            this.abrirServerToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.abrirServerToolStripMenuItem.Text = "Abrir server";
            this.abrirServerToolStripMenuItem.Click += new System.EventHandler(this.abrirServerToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 228);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(243, 40);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 295);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configuraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem carpetaATransmitirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirServerToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
    }
}


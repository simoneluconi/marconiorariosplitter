namespace Marconi_Orario_Splitter
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelezionaFile = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnSelezionaFile
            // 
            this.btnSelezionaFile.Location = new System.Drawing.Point(12, 12);
            this.btnSelezionaFile.Name = "btnSelezionaFile";
            this.btnSelezionaFile.Size = new System.Drawing.Size(577, 57);
            this.btnSelezionaFile.TabIndex = 0;
            this.btnSelezionaFile.Text = "Seleziona File";
            this.btnSelezionaFile.UseVisualStyleBackColor = true;
            this.btnSelezionaFile.Click += new System.EventHandler(this.btnSelezionaFile_Click);
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(12, 92);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(577, 199);
            this.lstLog.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 306);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnSelezionaFile);
            this.Name = "Form1";
            this.Text = "Marconi Orario Splitter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelezionaFile;
        private System.Windows.Forms.ListBox lstLog;
    }
}


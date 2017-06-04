namespace Test
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
            this.suggestionTextBox1 = new SuggestionTextBox.SuggestionTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // suggestionTextBox1
            // 
            this.suggestionTextBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.suggestionTextBox1.IsSuggestionBoxCaseSensistive = true;
            this.suggestionTextBox1.Location = new System.Drawing.Point(12, 12);
            this.suggestionTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.suggestionTextBox1.Name = "suggestionTextBox1";
            this.suggestionTextBox1.Size = new System.Drawing.Size(453, 22);
            this.suggestionTextBox1.SuggestionBoxBackColor = System.Drawing.Color.White;
            this.suggestionTextBox1.SuggestionBoxForeColor = System.Drawing.Color.Black;
            this.suggestionTextBox1.SuggestionBoxHeight = 200;
            this.suggestionTextBox1.SuggestionBoxSelectionColor = System.Drawing.Color.Blue;
            this.suggestionTextBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(336, 238);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 46);
            this.button1.TabIndex = 3;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 295);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.suggestionTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "SuggestionBox Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SuggestionTextBox.SuggestionTextBox suggestionTextBox1;
        private System.Windows.Forms.Button button1;
    }
}


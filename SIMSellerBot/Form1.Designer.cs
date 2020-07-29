namespace SIMSellerTelegramBot
{
    partial class BotForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartBotButton = new System.Windows.Forms.Button();
            this.StopBotButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartBotButton
            // 
            this.StartBotButton.Location = new System.Drawing.Point(13, 13);
            this.StartBotButton.Name = "StartBotButton";
            this.StartBotButton.Size = new System.Drawing.Size(90, 32);
            this.StartBotButton.TabIndex = 0;
            this.StartBotButton.Text = "Start Bot";
            this.StartBotButton.UseVisualStyleBackColor = true;
            this.StartBotButton.Click += new System.EventHandler(this.StartBotButton_Click);
            // 
            // StopBotButton
            // 
            this.StopBotButton.Location = new System.Drawing.Point(12, 51);
            this.StopBotButton.Name = "StopBotButton";
            this.StopBotButton.Size = new System.Drawing.Size(90, 32);
            this.StopBotButton.TabIndex = 1;
            this.StopBotButton.Text = "Stop Bot";
            this.StopBotButton.UseVisualStyleBackColor = true;
            this.StopBotButton.Click += new System.EventHandler(this.StopBotButton_Click);
            // 
            // BotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StopBotButton);
            this.Controls.Add(this.StartBotButton);
            this.Name = "BotForm";
            this.Text = "Burtimax Bot Form";
            this.Load += new System.EventHandler(this.BotForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartBotButton;
        private System.Windows.Forms.Button StopBotButton;
    }
}


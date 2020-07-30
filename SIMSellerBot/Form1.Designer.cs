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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StartBotButton
            // 
            this.StartBotButton.Font = new System.Drawing.Font("Franklin Gothic Demi", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StartBotButton.Location = new System.Drawing.Point(12, 55);
            this.StartBotButton.Name = "StartBotButton";
            this.StartBotButton.Size = new System.Drawing.Size(141, 40);
            this.StartBotButton.TabIndex = 0;
            this.StartBotButton.Text = "Start Bot";
            this.StartBotButton.UseVisualStyleBackColor = true;
            this.StartBotButton.Click += new System.EventHandler(this.StartBotButton_Click);
            // 
            // StopBotButton
            // 
            this.StopBotButton.Font = new System.Drawing.Font("Franklin Gothic Demi", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StopBotButton.Location = new System.Drawing.Point(159, 55);
            this.StopBotButton.Name = "StopBotButton";
            this.StopBotButton.Size = new System.Drawing.Size(148, 40);
            this.StopBotButton.TabIndex = 1;
            this.StopBotButton.Text = "Stop Bot";
            this.StopBotButton.UseVisualStyleBackColor = true;
            this.StopBotButton.Click += new System.EventHandler(this.StopBotButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Franklin Gothic Demi", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 43);
            this.label1.TabIndex = 2;
            this.label1.Text = "SIM SELLER BOT";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // BotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 110);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StopBotButton);
            this.Controls.Add(this.StartBotButton);
            this.Name = "BotForm";
            this.Text = "Burtimax Bot Form (SIMSellerBot)";
            this.Load += new System.EventHandler(this.BotForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartBotButton;
        private System.Windows.Forms.Button StopBotButton;
        private System.Windows.Forms.Label label1;
    }
}


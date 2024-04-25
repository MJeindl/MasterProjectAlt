namespace MiscSettings
{
    partial class MiscSet
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
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.textBoxpxConversion = new System.Windows.Forms.TextBox();
            this.textBoxLaserPower = new System.Windows.Forms.TextBox();
            this.UpdateFitSettings = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.textBoxExpS = new System.Windows.Forms.TextBox();
            this.textBoxExpM = new System.Windows.Forms.TextBox();
            this.textBoxExpL = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxpxConversion
            // 
            this.textBoxpxConversion.Location = new System.Drawing.Point(13, 12);
            this.textBoxpxConversion.MaxLength = 6;
            this.textBoxpxConversion.Name = "textBoxpxConversion";
            this.textBoxpxConversion.Size = new System.Drawing.Size(100, 20);
            this.textBoxpxConversion.TabIndex = 0;
            this.textBoxpxConversion.TabStop = false;
            this.textBoxpxConversion.Text = "1";
            // 
            // textBoxLaserPower
            // 
            this.textBoxLaserPower.Location = new System.Drawing.Point(13, 39);
            this.textBoxLaserPower.MaxLength = 6;
            this.textBoxLaserPower.Name = "textBoxLaserPower";
            this.textBoxLaserPower.Size = new System.Drawing.Size(100, 20);
            this.textBoxLaserPower.TabIndex = 1;
            this.textBoxLaserPower.Text = "1e-6";
            // 
            // UpdateFitSettings
            // 
            this.UpdateFitSettings.Location = new System.Drawing.Point(24, 65);
            this.UpdateFitSettings.Name = "UpdateFitSettings";
            this.UpdateFitSettings.Size = new System.Drawing.Size(75, 23);
            this.UpdateFitSettings.TabIndex = 2;
            this.UpdateFitSettings.Text = "Enter";
            this.UpdateFitSettings.UseVisualStyleBackColor = true;
            this.UpdateFitSettings.Click += new System.EventHandler(this.UpdateFitSettings_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(119, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "µm per px";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(119, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "W";
            // 
            // buttonDefault
            // 
            this.buttonDefault.Location = new System.Drawing.Point(105, 65);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(75, 23);
            this.buttonDefault.TabIndex = 5;
            this.buttonDefault.Text = "Default";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // textBoxExpS
            // 
            this.textBoxExpS.Location = new System.Drawing.Point(229, 10);
            this.textBoxExpS.Name = "textBoxExpS";
            this.textBoxExpS.Size = new System.Drawing.Size(46, 20);
            this.textBoxExpS.TabIndex = 6;
            this.textBoxExpS.Text = "50";
            this.textBoxExpS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxExpM
            // 
            this.textBoxExpM.Location = new System.Drawing.Point(229, 36);
            this.textBoxExpM.Name = "textBoxExpM";
            this.textBoxExpM.Size = new System.Drawing.Size(46, 20);
            this.textBoxExpM.TabIndex = 7;
            this.textBoxExpM.Text = "150";
            this.textBoxExpM.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxExpL
            // 
            this.textBoxExpL.Location = new System.Drawing.Point(229, 62);
            this.textBoxExpL.Name = "textBoxExpL";
            this.textBoxExpL.Size = new System.Drawing.Size(46, 20);
            this.textBoxExpL.TabIndex = 8;
            this.textBoxExpL.Text = "1000";
            this.textBoxExpL.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "sExp / 1E-7 s";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(281, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "mExp / 1E-7 s";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(281, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "lExp / 1E-7 s";
            // 
            // MiscSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 101);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxExpL);
            this.Controls.Add(this.textBoxExpM);
            this.Controls.Add(this.textBoxExpS);
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UpdateFitSettings);
            this.Controls.Add(this.textBoxLaserPower);
            this.Controls.Add(this.textBoxpxConversion);
            this.Name = "MiscSet";
            this.Text = "Miscellanous Settings";
            this.Load += new System.EventHandler(this.MiscSet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox textBoxpxConversion;
        private System.Windows.Forms.TextBox textBoxLaserPower;
        private System.Windows.Forms.Button UpdateFitSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.TextBox textBoxExpS;
        private System.Windows.Forms.TextBox textBoxExpM;
        private System.Windows.Forms.TextBox textBoxExpL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
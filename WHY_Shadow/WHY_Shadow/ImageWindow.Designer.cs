namespace WHY_Shadow
{
    partial class ImageWindow
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
            this.imageArea = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageArea)).BeginInit();
            this.SuspendLayout();
            // 
            // imageArea
            // 
            this.imageArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageArea.BackColor = System.Drawing.Color.Transparent;
            this.imageArea.Location = new System.Drawing.Point(0, 0);
            this.imageArea.Margin = new System.Windows.Forms.Padding(0);
            this.imageArea.Name = "imageArea";
            this.imageArea.Size = new System.Drawing.Size(397, 308);
            this.imageArea.TabIndex = 0;
            this.imageArea.TabStop = false;
            // 
            // ImageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 305);
            this.ControlBox = false;
            this.Controls.Add(this.imageArea);
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.Name = "ImageWindow";
            this.ShowIcon = false;
            this.Text = "ImageWindow";
            ((System.ComponentModel.ISupportInitialize)(this.imageArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imageArea;



    }
}
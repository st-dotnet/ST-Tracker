namespace TimeTracker.Form
{
    partial class IdleDialog
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
            this.workingBtn = new System.Windows.Forms.Button();
            this.notWorking = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // workingBtn
            // 
            this.workingBtn.BackColor = System.Drawing.Color.CornflowerBlue;
            this.workingBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workingBtn.ForeColor = System.Drawing.Color.White;
            this.workingBtn.Location = new System.Drawing.Point(9, 9);
            this.workingBtn.Margin = new System.Windows.Forms.Padding(0);
            this.workingBtn.Name = "workingBtn";
            this.workingBtn.Size = new System.Drawing.Size(198, 62);
            this.workingBtn.TabIndex = 0;
            this.workingBtn.Text = "Yes I was working";
            this.workingBtn.UseVisualStyleBackColor = false;
            this.workingBtn.Click += new System.EventHandler(this.workingBtn_Click);
            // 
            // notWorking
            // 
            this.notWorking.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notWorking.Location = new System.Drawing.Point(213, 9);
            this.notWorking.Margin = new System.Windows.Forms.Padding(0);
            this.notWorking.Name = "notWorking";
            this.notWorking.Size = new System.Drawing.Size(198, 62);
            this.notWorking.TabIndex = 1;
            this.notWorking.Text = "No I was on break";
            this.notWorking.UseVisualStyleBackColor = true;
            this.notWorking.Click += new System.EventHandler(this.notWorking_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(31)))), ((int)(((byte)(75)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(402, 55);
            this.label1.TabIndex = 2;
            this.label1.Text = "Idle Time";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // IdleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(423, 156);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.notWorking);
            this.Controls.Add(this.workingBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "IdleDialog";
            this.Text = "Idle Time";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button workingBtn;
        private System.Windows.Forms.Button notWorking;
        private System.Windows.Forms.Label label1;
    }
}
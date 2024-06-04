using System.Windows.Forms;

namespace TimeTracker.Form
{
    partial class Application
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Application));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.idlePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.notWorking = new System.Windows.Forms.Button();
            this.workingBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.profilePictureBox = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.EmployeeName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statsTotalText = new System.Windows.Forms.ToolStripStatusLabel();
            this.internetStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.startTrackingToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopTrackingToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.trackingStartTimeToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.trackingElapsedTimeToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.idlePanel.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profilePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.idlePanel);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel2);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip);
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(181)))), ((int)(((byte)(173)))));
            resources.ApplyResources(this.toolStripContainer1.TopToolStripPanel, "toolStripContainer1.TopToolStripPanel");
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripMain);
            this.toolStripContainer1.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // idlePanel
            // 
            this.idlePanel.Controls.Add(this.label2);
            this.idlePanel.Controls.Add(this.notWorking);
            this.idlePanel.Controls.Add(this.workingBtn);
            resources.ApplyResources(this.idlePanel, "idlePanel");
            this.idlePanel.Name = "idlePanel";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(31)))), ((int)(((byte)(75)))));
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // notWorking
            // 
            resources.ApplyResources(this.notWorking, "notWorking");
            this.notWorking.Name = "notWorking";
            this.notWorking.UseVisualStyleBackColor = true;
            this.notWorking.Click += new System.EventHandler(this.notWorking_Click);
            // 
            // workingBtn
            // 
            this.workingBtn.BackColor = System.Drawing.Color.CornflowerBlue;
            resources.ApplyResources(this.workingBtn, "workingBtn");
            this.workingBtn.ForeColor = System.Drawing.Color.White;
            this.workingBtn.Name = "workingBtn";
            this.workingBtn.UseVisualStyleBackColor = false;
            this.workingBtn.Click += new System.EventHandler(this.workingBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.profilePictureBox);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.EmployeeName);
            this.panel2.Controls.Add(this.label1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // profilePictureBox
            // 
            resources.ApplyResources(this.profilePictureBox, "profilePictureBox");
            this.profilePictureBox.Image = global::TimeTracker.Properties.Resources.dummy_profile;
            this.profilePictureBox.Name = "profilePictureBox";
            this.profilePictureBox.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::TimeTracker.Properties.Resources.logo;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // EmployeeName
            // 
            resources.ApplyResources(this.EmployeeName, "EmployeeName");
            this.EmployeeName.Name = "EmployeeName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statsTotalText,
            this.internetStatus});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // statsTotalText
            // 
            this.statsTotalText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(184)))), ((int)(((byte)(225)))));
            this.statsTotalText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statsTotalText.Name = "statsTotalText";
            resources.ApplyResources(this.statsTotalText, "statsTotalText");
            // 
            // internetStatus
            // 
            this.internetStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(31)))), ((int)(((byte)(75)))));
            this.internetStatus.ForeColor = System.Drawing.Color.White;
            this.internetStatus.Name = "internetStatus";
            resources.ApplyResources(this.internetStatus, "internetStatus");
            // 
            // toolStripMain
            // 
            this.toolStripMain.AllowItemReorder = true;
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(181)))), ((int)(((byte)(173)))));
            this.toolStripMain.CanOverflow = false;
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startTrackingToolStripButton,
            this.stopTrackingToolStripButton,
            this.trackingStartTimeToolStripTextBox,
            this.toolStripButton1,
            this.trackingElapsedTimeToolStripTextBox});
            this.toolStripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // startTrackingToolStripButton
            // 
            resources.ApplyResources(this.startTrackingToolStripButton, "startTrackingToolStripButton");
            this.startTrackingToolStripButton.BackColor = System.Drawing.Color.Transparent;
            this.startTrackingToolStripButton.ForeColor = System.Drawing.Color.White;
            this.startTrackingToolStripButton.Margin = new System.Windows.Forms.Padding(10, 10, 0, 2);
            this.startTrackingToolStripButton.Name = "startTrackingToolStripButton";
            this.startTrackingToolStripButton.Click += new System.EventHandler(this.startTrackingToolStripButton_Click);
            // 
            // stopTrackingToolStripButton
            // 
            resources.ApplyResources(this.stopTrackingToolStripButton, "stopTrackingToolStripButton");
            this.stopTrackingToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopTrackingToolStripButton.Margin = new System.Windows.Forms.Padding(5, 10, 0, 2);
            this.stopTrackingToolStripButton.Name = "stopTrackingToolStripButton";
            this.stopTrackingToolStripButton.Click += new System.EventHandler(this.stopTrackingToolStripButton_Click);
            // 
            // trackingStartTimeToolStripTextBox
            // 
            this.trackingStartTimeToolStripTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(181)))), ((int)(((byte)(173)))));
            this.trackingStartTimeToolStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.trackingStartTimeToolStripTextBox, "trackingStartTimeToolStripTextBox");
            this.trackingStartTimeToolStripTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.trackingStartTimeToolStripTextBox.Margin = new System.Windows.Forms.Padding(5, 20, 1, 10);
            this.trackingStartTimeToolStripTextBox.Name = "trackingStartTimeToolStripTextBox";
            this.trackingStartTimeToolStripTextBox.ReadOnly = true;
            // 
            // toolStripButton1
            // 
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ForeColor = System.Drawing.Color.White;
            this.toolStripButton1.Margin = new System.Windows.Forms.Padding(5, 10, 0, 0);
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // trackingElapsedTimeToolStripTextBox
            // 
            resources.ApplyResources(this.trackingElapsedTimeToolStripTextBox, "trackingElapsedTimeToolStripTextBox");
            this.trackingElapsedTimeToolStripTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(181)))), ((int)(((byte)(173)))));
            this.trackingElapsedTimeToolStripTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.trackingElapsedTimeToolStripTextBox.Margin = new System.Windows.Forms.Padding(158, 0, 1, 10);
            this.trackingElapsedTimeToolStripTextBox.Name = "trackingElapsedTimeToolStripTextBox";
            this.trackingElapsedTimeToolStripTextBox.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Category";
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // notifyIcon
            // 
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::TimeTracker.Properties.Resources.SupremeLogo;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // Application
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "Application";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.idlePanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.profilePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripButton startTrackingToolStripButton;
        private System.Windows.Forms.ToolStripButton stopTrackingToolStripButton;
        private System.Windows.Forms.ToolStripTextBox trackingElapsedTimeToolStripTextBox;
        private System.Windows.Forms.ToolStripTextBox trackingStartTimeToolStripTextBox;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statsTotalText;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private Label label1;
        private Label EmployeeName;
        private PictureBox profilePictureBox;
        private ToolStripStatusLabel internetStatus;
        private Panel idlePanel;
        private Button workingBtn;
        private Button notWorking;
        private Label label2;
        private Panel panel1;
        private PictureBox pictureBox2;
    }
}


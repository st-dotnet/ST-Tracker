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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.alert = new System.Windows.Forms.Label();
            this.dataGridViewMain = new System.Windows.Forms.DataGridView();
            this.DateStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeSpan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CategoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statsTotalText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statsSelectedText = new System.Windows.Forms.ToolStripStatusLabel();
            this.statsCategoryText = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.addToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.startTrackingToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopTrackingToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.trackingStartTimeToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.trackingElapsedTimeToolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.categoryToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            resources.ApplyResources(this.toolStripContainer1.ContentPanel, "toolStripContainer1.ContentPanel");
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dataGridViewMain);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.statusStrip);
            resources.ApplyResources(this.toolStripContainer1, "toolStripContainer1");
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStripMain);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panel1.Controls.Add(this.alert);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Visible = false;
            this.panel1.Dock = DockStyle.Bottom;
            // 
            // alert
            // 
            this.alert.BackColor = System.Drawing.Color.IndianRed;
            this.alert.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.alert, "alert");
            this.alert.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.alert.Name = "alert";
            this.alert.UseMnemonic = false;
            // 
            // dataGridViewMain
            // 
            this.dataGridViewMain.AllowUserToAddRows = false;
            this.dataGridViewMain.AllowUserToDeleteRows = false;
            this.dataGridViewMain.AllowUserToOrderColumns = true;
            this.dataGridViewMain.AllowUserToResizeRows = false;
            this.dataGridViewMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewMain.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DateStart,
            this.EndDate,
            this.TimeSpan,
            this.CategoryName});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMain.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.dataGridViewMain, "dataGridViewMain");
            this.dataGridViewMain.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewMain.Name = "dataGridViewMain";
            this.dataGridViewMain.ReadOnly = true;
            this.dataGridViewMain.RowHeadersVisible = false;
            this.dataGridViewMain.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewMain.SelectionChanged += new System.EventHandler(this.dataGridViewMain_SelectionChanged);
            this.dataGridViewMain.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridViewMain_Paint);
            this.dataGridViewMain.Resize += new System.EventHandler(this.dataGridViewMain_Resize);
            // 
            // DateStart
            // 
            this.DateStart.DataPropertyName = "StartTime";
            dataGridViewCellStyle2.Format = "yyyy-MM-dd HH:mm:ss";
            this.DateStart.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.DateStart, "DateStart");
            this.DateStart.Name = "DateStart";
            this.DateStart.ReadOnly = true;
            // 
            // EndDate
            // 
            this.EndDate.DataPropertyName = "EndTime";
            dataGridViewCellStyle3.Format = "yyyy-MM-dd HH:mm:ss";
            this.EndDate.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.EndDate, "EndDate");
            this.EndDate.Name = "EndDate";
            this.EndDate.ReadOnly = true;
            // 
            // TimeSpan
            // 
            this.TimeSpan.DataPropertyName = "TimeElapsed";
            resources.ApplyResources(this.TimeSpan, "TimeSpan");
            this.TimeSpan.Name = "TimeSpan";
            this.TimeSpan.ReadOnly = true;
            // 
            // CategoryName
            // 
            this.CategoryName.DataPropertyName = "Category";
            resources.ApplyResources(this.CategoryName, "CategoryName");
            this.CategoryName.Name = "CategoryName";
            this.CategoryName.ReadOnly = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statsTotalText,
            this.statsSelectedText,
            this.statsCategoryText});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // statsTotalText
            // 
            this.statsTotalText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statsTotalText.Name = "statsTotalText";
            resources.ApplyResources(this.statsTotalText, "statsTotalText");
            // 
            // statsSelectedText
            // 
            this.statsSelectedText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statsSelectedText.Name = "statsSelectedText";
            resources.ApplyResources(this.statsSelectedText, "statsSelectedText");
            // 
            // statsCategoryText
            // 
            this.statsCategoryText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statsCategoryText.Name = "statsCategoryText";
            resources.ApplyResources(this.statsCategoryText, "statsCategoryText");
            // 
            // toolStripMain
            // 
            this.toolStripMain.CanOverflow = false;
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator,
            this.addToolStripButton,
            this.toolStripSeparator1,
            this.startTrackingToolStripButton,
            this.stopTrackingToolStripButton,
            this.trackingStartTimeToolStripTextBox,
            this.trackingElapsedTimeToolStripTextBox,
            this.categoryToolStripComboBox,
            this.toolStripButton1});
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            resources.ApplyResources(this.toolStripSeparator, "toolStripSeparator");
            // 
            // addToolStripButton
            // 
            resources.ApplyResources(this.addToolStripButton, "addToolStripButton");
            this.addToolStripButton.Name = "addToolStripButton";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // startTrackingToolStripButton
            // 
            resources.ApplyResources(this.startTrackingToolStripButton, "startTrackingToolStripButton");
            this.startTrackingToolStripButton.Name = "startTrackingToolStripButton";
            this.startTrackingToolStripButton.Click += new System.EventHandler(this.startTrackingToolStripButton_Click);
            // 
            // stopTrackingToolStripButton
            // 
            this.stopTrackingToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.stopTrackingToolStripButton, "stopTrackingToolStripButton");
            this.stopTrackingToolStripButton.Name = "stopTrackingToolStripButton";
            this.stopTrackingToolStripButton.Click += new System.EventHandler(this.stopTrackingToolStripButton_Click);
            // 
            // trackingStartTimeToolStripTextBox
            // 
            resources.ApplyResources(this.trackingStartTimeToolStripTextBox, "trackingStartTimeToolStripTextBox");
            this.trackingStartTimeToolStripTextBox.Name = "trackingStartTimeToolStripTextBox";
            this.trackingStartTimeToolStripTextBox.ReadOnly = true;
            // 
            // trackingElapsedTimeToolStripTextBox
            // 
            resources.ApplyResources(this.trackingElapsedTimeToolStripTextBox, "trackingElapsedTimeToolStripTextBox");
            this.trackingElapsedTimeToolStripTextBox.Name = "trackingElapsedTimeToolStripTextBox";
            this.trackingElapsedTimeToolStripTextBox.ReadOnly = true;
            // 
            // categoryToolStripComboBox
            // 
            this.categoryToolStripComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.categoryToolStripComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.categoryToolStripComboBox.Name = "categoryToolStripComboBox";
            resources.ApplyResources(this.categoryToolStripComboBox, "categoryToolStripComboBox");
            this.categoryToolStripComboBox.Sorted = true;
            this.categoryToolStripComboBox.TextUpdate += new System.EventHandler(this.categoryToolStripComboBox_TextUpdate);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
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
            // Application
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "Application";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Application_FormClosing);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMain)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.DataGridView dataGridViewMain;
        private System.Windows.Forms.ToolStripButton addToolStripButton;
        private System.Windows.Forms.ToolStripButton startTrackingToolStripButton;
        private System.Windows.Forms.ToolStripButton stopTrackingToolStripButton;
        private System.Windows.Forms.ToolStripTextBox trackingElapsedTimeToolStripTextBox;
        private System.Windows.Forms.ToolStripTextBox trackingStartTimeToolStripTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeSpan;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryName;
        private System.Windows.Forms.ToolStripComboBox categoryToolStripComboBox;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statsTotalText;
        private System.Windows.Forms.ToolStripStatusLabel statsSelectedText;
        private System.Windows.Forms.ToolStripStatusLabel statsCategoryText;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label alert;
    }
}


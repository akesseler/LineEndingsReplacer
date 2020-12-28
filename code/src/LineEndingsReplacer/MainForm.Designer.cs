/*
 * MIT License
 * 
 * Copyright (c) 2020 plexdata.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Plexdata.LineEndingsReplacer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbsMain = new System.Windows.Forms.ToolStrip();
            this.tbbExit = new System.Windows.Forms.ToolStripButton();
            this.tbbPlay = new System.Windows.Forms.ToolStripButton();
            this.tbbScan = new System.Windows.Forms.ToolStripButton();
            this.tbbSort = new System.Windows.Forms.ToolStripButton();
            this.tbbUndo = new System.Windows.Forms.ToolStripButton();
            this.tbbInfo = new System.Windows.Forms.ToolStripButton();
            this.sbsMain = new System.Windows.Forms.StatusStrip();
            this.sblMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.tipMain = new System.Windows.Forms.ToolTip(this.components);
            this.txtExcludes = new System.Windows.Forms.RichTextBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.lblFolder = new System.Windows.Forms.Label();
            this.panExcludes = new System.Windows.Forms.Panel();
            this.lblExcludes = new System.Windows.Forms.Label();
            this.cmbEnding = new System.Windows.Forms.ComboBox();
            this.lblEnding = new System.Windows.Forms.Label();
            this.lstResults = new Plexdata.LineEndingsReplacer.Controls.WorkResultUserControl();
            this.scxDisplay = new Plexdata.LineEndingsReplacer.Controls.SplitContainerEx();
            this.tbsMain.SuspendLayout();
            this.sbsMain.SuspendLayout();
            this.panExcludes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scxDisplay)).BeginInit();
            this.scxDisplay.Panel1.SuspendLayout();
            this.scxDisplay.Panel2.SuspendLayout();
            this.scxDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbsMain
            // 
            this.tbsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbExit,
            this.tbbPlay,
            this.tbbScan,
            this.tbbSort,
            this.tbbUndo,
            this.tbbInfo});
            this.tbsMain.Location = new System.Drawing.Point(0, 0);
            this.tbsMain.Name = "tbsMain";
            this.tbsMain.Size = new System.Drawing.Size(784, 35);
            this.tbsMain.TabIndex = 0;
            // 
            // tbbExit
            // 
            this.tbbExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbExit.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonExit;
            this.tbbExit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbExit.Name = "tbbExit";
            this.tbbExit.Padding = new System.Windows.Forms.Padding(2);
            this.tbbExit.Size = new System.Drawing.Size(32, 32);
            this.tbbExit.Text = "Exit";
            this.tbbExit.ToolTipText = "Close main window and exit application.";
            this.tbbExit.Click += new System.EventHandler(this.OnButtonExitClick);
            // 
            // tbbPlay
            // 
            this.tbbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbPlay.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonPlay;
            this.tbbPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbPlay.Name = "tbbPlay";
            this.tbbPlay.Padding = new System.Windows.Forms.Padding(2);
            this.tbbPlay.Size = new System.Drawing.Size(32, 32);
            this.tbbPlay.Text = "Play";
            this.tbbPlay.ToolTipText = "Execute tool with current settings.";
            this.tbbPlay.Click += new System.EventHandler(this.OnButtonPlayClick);
            // 
            // tbbScan
            // 
            this.tbbScan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbScan.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonScan;
            this.tbbScan.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbScan.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbScan.Name = "tbbScan";
            this.tbbScan.Padding = new System.Windows.Forms.Padding(2);
            this.tbbScan.Size = new System.Drawing.Size(32, 32);
            this.tbbScan.Text = "Scan";
            this.tbbScan.ToolTipText = "Scan source folder.";
            this.tbbScan.Click += new System.EventHandler(this.OnButtonScanClick);
            // 
            // tbbSort
            // 
            this.tbbSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbSort.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonSort;
            this.tbbSort.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbSort.Name = "tbbSort";
            this.tbbSort.Padding = new System.Windows.Forms.Padding(2);
            this.tbbSort.Size = new System.Drawing.Size(32, 32);
            this.tbbSort.Text = "Sort";
            this.tbbSort.ToolTipText = "Toggle sorting of exclude list content.";
            this.tbbSort.Click += new System.EventHandler(this.OnButtonSortClick);
            // 
            // tbbUndo
            // 
            this.tbbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbUndo.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonUndo;
            this.tbbUndo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbUndo.Name = "tbbUndo";
            this.tbbUndo.Padding = new System.Windows.Forms.Padding(2);
            this.tbbUndo.Size = new System.Drawing.Size(32, 32);
            this.tbbUndo.Text = "Undo";
            this.tbbUndo.ToolTipText = "Undo changes and restore original settings.";
            this.tbbUndo.Click += new System.EventHandler(this.OnButtonUndoClick);
            // 
            // tbbInfo
            // 
            this.tbbInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbbInfo.Image = global::Plexdata.LineEndingsReplacer.Properties.Resources.ButtonInfo;
            this.tbbInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbbInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbInfo.Name = "tbbInfo";
            this.tbbInfo.Padding = new System.Windows.Forms.Padding(2);
            this.tbbInfo.Size = new System.Drawing.Size(32, 32);
            this.tbbInfo.Text = "Info";
            this.tbbInfo.ToolTipText = "Show program info dialog.";
            this.tbbInfo.Click += new System.EventHandler(this.OnButtonInfoClick);
            // 
            // sbsMain
            // 
            this.sbsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sblMain});
            this.sbsMain.Location = new System.Drawing.Point(0, 539);
            this.sbsMain.Name = "sbsMain";
            this.sbsMain.Size = new System.Drawing.Size(784, 22);
            this.sbsMain.TabIndex = 2;
            // 
            // sblMain
            // 
            this.sblMain.Name = "sblMain";
            this.sblMain.Size = new System.Drawing.Size(22, 17);
            this.sblMain.Text = "???";
            // 
            // txtExcludes
            // 
            this.txtExcludes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtExcludes.CausesValidation = false;
            this.txtExcludes.DetectUrls = false;
            this.txtExcludes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExcludes.Location = new System.Drawing.Point(5, 0);
            this.txtExcludes.Name = "txtExcludes";
            this.txtExcludes.Size = new System.Drawing.Size(697, 134);
            this.txtExcludes.TabIndex = 0;
            this.txtExcludes.Tag = "";
            this.txtExcludes.Text = ".test\ntest\n*.test";
            this.tipMain.SetToolTip(this.txtExcludes, "For \"folder names\" just use the name of the folder. \r\nFor \"file names\" just use t" +
        "he file extension plus a leading asterisk as wildcard.");
            this.txtExcludes.WordWrap = false;
            this.txtExcludes.Enter += new System.EventHandler(this.OnTextBoxEnter);
            this.txtExcludes.Leave += new System.EventHandler(this.OnTextBoxLeave);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(70, 13);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(673, 20);
            this.txtFolder.TabIndex = 1;
            this.tipMain.SetToolTip(this.txtFolder, "Start with replacing line endings in this folder.");
            this.txtFolder.TextChanged += new System.EventHandler(this.OnFolderTextChanged);
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Location = new System.Drawing.Point(749, 11);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(25, 23);
            this.btnFolder.TabIndex = 2;
            this.btnFolder.Text = "...";
            this.tipMain.SetToolTip(this.btnFolder, "Click to open browse for folder.");
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.OnButtonFolderClick);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(13, 16);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(39, 13);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder:";
            // 
            // panExcludes
            // 
            this.panExcludes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panExcludes.BackColor = System.Drawing.SystemColors.Window;
            this.panExcludes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panExcludes.Controls.Add(this.txtExcludes);
            this.panExcludes.Location = new System.Drawing.Point(70, 66);
            this.panExcludes.Name = "panExcludes";
            this.panExcludes.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.panExcludes.Size = new System.Drawing.Size(704, 136);
            this.panExcludes.TabIndex = 6;
            // 
            // lblExcludes
            // 
            this.lblExcludes.AutoSize = true;
            this.lblExcludes.Location = new System.Drawing.Point(12, 68);
            this.lblExcludes.Name = "lblExcludes";
            this.lblExcludes.Size = new System.Drawing.Size(48, 13);
            this.lblExcludes.TabIndex = 5;
            this.lblExcludes.Text = "Exclude:";
            // 
            // cmbEnding
            // 
            this.cmbEnding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEnding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEnding.Location = new System.Drawing.Point(70, 39);
            this.cmbEnding.Name = "cmbEnding";
            this.cmbEnding.Size = new System.Drawing.Size(704, 21);
            this.cmbEnding.TabIndex = 4;
            this.cmbEnding.SelectedValueChanged += new System.EventHandler(this.OnSelectedValueChanged);
            // 
            // lblEnding
            // 
            this.lblEnding.AutoSize = true;
            this.lblEnding.Location = new System.Drawing.Point(12, 42);
            this.lblEnding.Name = "lblEnding";
            this.lblEnding.Size = new System.Drawing.Size(43, 13);
            this.lblEnding.TabIndex = 3;
            this.lblEnding.Text = "Ending:";
            // 
            // lstResults
            // 
            this.lstResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstResults.Location = new System.Drawing.Point(10, 5);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(764, 271);
            this.lstResults.TabIndex = 0;
            // 
            // scxDisplay
            // 
            this.scxDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scxDisplay.Location = new System.Drawing.Point(0, 35);
            this.scxDisplay.Name = "scxDisplay";
            this.scxDisplay.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scxDisplay.Panel1
            // 
            this.scxDisplay.Panel1.Controls.Add(this.lblExcludes);
            this.scxDisplay.Panel1.Controls.Add(this.panExcludes);
            this.scxDisplay.Panel1.Controls.Add(this.lblFolder);
            this.scxDisplay.Panel1.Controls.Add(this.lblEnding);
            this.scxDisplay.Panel1.Controls.Add(this.txtFolder);
            this.scxDisplay.Panel1.Controls.Add(this.cmbEnding);
            this.scxDisplay.Panel1.Controls.Add(this.btnFolder);
            this.scxDisplay.Panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.scxDisplay.Panel1MinSize = 150;
            // 
            // scxDisplay.Panel2
            // 
            this.scxDisplay.Panel2.Controls.Add(this.lstResults);
            this.scxDisplay.Panel2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.scxDisplay.Panel2MinSize = 200;
            this.scxDisplay.Size = new System.Drawing.Size(784, 504);
            this.scxDisplay.SplitterDistance = 210;
            this.scxDisplay.SplitterWidth = 8;
            this.scxDisplay.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.scxDisplay);
            this.Controls.Add(this.sbsMain);
            this.Controls.Add(this.tbsMain);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Line Endings Replacer";
            this.tbsMain.ResumeLayout(false);
            this.tbsMain.PerformLayout();
            this.sbsMain.ResumeLayout(false);
            this.sbsMain.PerformLayout();
            this.panExcludes.ResumeLayout(false);
            this.scxDisplay.Panel1.ResumeLayout(false);
            this.scxDisplay.Panel1.PerformLayout();
            this.scxDisplay.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scxDisplay)).EndInit();
            this.scxDisplay.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tbsMain;
        private System.Windows.Forms.StatusStrip sbsMain;
        private System.Windows.Forms.ToolStripButton tbbExit;
        private System.Windows.Forms.ToolTip tipMain;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label lblExcludes;
        private System.Windows.Forms.RichTextBox txtExcludes;
        private System.Windows.Forms.Panel panExcludes;
        private System.Windows.Forms.ComboBox cmbEnding;
        private System.Windows.Forms.Label lblEnding;
        private System.Windows.Forms.ToolStripButton tbbPlay;
        private System.Windows.Forms.ToolStripButton tbbScan;
        private System.Windows.Forms.ToolStripButton tbbSort;
        private System.Windows.Forms.ToolStripButton tbbUndo;
        private System.Windows.Forms.ToolStripStatusLabel sblMain;
        private Controls.WorkResultUserControl lstResults;
        private Controls.SplitContainerEx scxDisplay;
        private System.Windows.Forms.ToolStripButton tbbInfo;
    }
}


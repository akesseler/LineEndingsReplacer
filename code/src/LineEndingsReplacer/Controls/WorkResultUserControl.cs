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

using Plexdata.LineEndingsReplacer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Controls
{
    public class WorkResultUserControl : UserControl
    {
        #region Private Fields

        private ToolStrip tbsMain;
        private ItemsListView lstItems;
        private ColumnHeader clhAction;
        private ToolStripButton tbbFollow;
        private ToolStripButton tbbReport;
        private ToolStripLabel tslDeclined;
        private ToolStripLabel tslExcluded;
        private ToolStripLabel tslIncluded;
        private ColumnHeader clhResult;
        private ColumnHeader clhMessage;

        private Int32 declinedCount = 0;
        private Int32 excludedCount = 0;
        private Int32 includedCount = 0;

        #endregion

        #region Construction

        public WorkResultUserControl()
            : base()
        {
            this.InitializeComponent();
            this.Reset();
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            this.declinedCount = 0;
            this.excludedCount = 0;
            this.includedCount = 0;

            this.tslDeclined.Text = $"{WorkResultListItem.ActionDeclined}: {this.declinedCount:N0}";
            this.tslExcluded.Text = $"{WorkResultListItem.ActionExcluded}: {this.excludedCount:N0}";
            this.tslIncluded.Text = $"{WorkResultListItem.ActionIncluded}: {this.includedCount:N0}";

            this.lstItems.Items.Clear();

            // Playchild...
            Int32 width = base.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 3;
            this.lstItems.Columns[0].Width = (Int32)(width * (12f / 100));
            this.lstItems.Columns[1].Width = (Int32)(width * (44f / 100));
            this.lstItems.Columns[2].Width = (Int32)(width * (44f / 100));

            this.tbbReport.Enabled = false;
        }

        public void Append(WorkResultListItem value)
        {
            this.lstItems.Append(value, this.tbbFollow.Checked);

            if (value.IsDeclined)
            {
                this.declinedCount++;
                this.tslDeclined.Text = $"{WorkResultListItem.ActionDeclined}: {this.declinedCount:N0}";
            }
            else if (value.IsExcluded)
            {
                this.excludedCount++;
                this.tslExcluded.Text = $"{WorkResultListItem.ActionExcluded}: {this.excludedCount:N0}";
            }
            else if (value.IsIncluded)
            {
                this.includedCount++;
                this.tslIncluded.Text = $"{WorkResultListItem.ActionIncluded}: {this.includedCount:N0}";
            }
        }

        public void SetRunning(Boolean running)
        {
            this.tbbReport.Enabled = !running && this.lstItems.Items.Count > 0;
        }

        #endregion

        #region Private Events

        private void OnListViewSelectedIndexChanged(Object sender, EventArgs args)
        {
            this.tbbFollow.Checked = false;
        }

        private void OnReportButtonClick(Object sender, EventArgs args)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = $"LineEndingsReplacerReport-{DateTime.Now:yyyyMMdd-HHmmss}.txt",
                Filter = "Report files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (DialogResult.OK != dialog.ShowDialog(this.TopLevelControl))
            {
                return;
            }

            try
            {
                List<String[]> items = new List<String[]>()
                {
                    new String[] { "Action", "Origin", "Details", "Source" }
                };

                Int32 widthCell0 = items[0][0].Length;
                Int32 widthCell1 = items[0][1].Length;
                Int32 widthCell2 = items[0][2].Length;

                foreach (ListViewItem current in this.lstItems.Items)
                {
                    if (current.Tag is WorkResultListItem value)
                    {
                        String[] cells = value.GetReportCells();

                        if (widthCell0 < cells[0].Length) { widthCell0 = cells[0].Length; }
                        if (widthCell1 < cells[1].Length) { widthCell1 = cells[1].Length; }
                        if (widthCell2 < cells[2].Length) { widthCell2 = cells[2].Length; }

                        items.Add(cells);
                    }
                }

                using (StreamWriter writer = File.CreateText(dialog.FileName))
                {
                    foreach (String[] cells in items)
                    {
                        cells[0] = cells[0].PadRight(widthCell0, ' ');
                        cells[1] = cells[1].PadRight(widthCell1, ' ');
                        cells[2] = cells[2].PadRight(widthCell2, ' ');

                        writer.WriteLine(String.Join(" | ", cells));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(this.TopLevelControl, exception.Message, this.TopLevelControl.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(WorkResultUserControl));
            this.tbsMain = new ToolStrip();
            this.tbbFollow = new ToolStripButton();
            this.tbbReport = new ToolStripButton();
            this.tslDeclined = new ToolStripLabel();
            this.tslExcluded = new ToolStripLabel();
            this.tslIncluded = new ToolStripLabel();
            this.lstItems = new WorkResultUserControl.ItemsListView();
            this.clhAction = ((ColumnHeader)(new ColumnHeader()));
            this.clhMessage = ((ColumnHeader)(new ColumnHeader()));
            this.clhResult = ((ColumnHeader)(new ColumnHeader()));
            this.tbsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbsMain
            // 
            this.tbsMain.Items.AddRange(new ToolStripItem[] {
            this.tbbFollow,
            this.tbbReport,
            new ToolStripSeparator(),
            this.tslDeclined,
            this.tslExcluded,
            this.tslIncluded});
            this.tbsMain.Location = new Point(0, 0);
            this.tbsMain.Name = "tbsMain";
            this.tbsMain.Size = new Size(594, 25);
            this.tbsMain.TabIndex = 0;
            // 
            // tbbFollow
            // 
            this.tbbFollow.CheckOnClick = true;
            this.tbbFollow.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tbbFollow.Name = "tbbFollow";
            this.tbbFollow.Text = "Follow";
            this.tbbFollow.ToolTipText = "Enable or disable following mode.";
            // 
            // tbbReport
            // 
            this.tbbReport.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tbbReport.Name = "tbbReport";
            this.tbbReport.Text = "Report";
            this.tbbReport.ToolTipText = "Save processing results into an external file.";
            this.tbbReport.Click += this.OnReportButtonClick;
            // 
            // tslDeclined
            // 
            this.tslDeclined.Name = "tslDeclined";
            this.tslDeclined.Size = new Size(53, 22);
            this.tslDeclined.Text = "???";
            // 
            // tslExcluded
            // 
            this.tslExcluded.Name = "tslExcluded";
            this.tslExcluded.Size = new Size(55, 22);
            this.tslExcluded.Text = "???";
            // 
            // tslIncluded
            // 
            this.tslIncluded.Name = "tslIncluded";
            this.tslIncluded.Size = new Size(53, 22);
            this.tslIncluded.Text = "???";
            // 
            // lstItems
            // 
            this.lstItems.Columns.AddRange(new ColumnHeader[] {
            this.clhAction,
            this.clhMessage,
            this.clhResult});
            this.lstItems.Dock = DockStyle.Fill;
            this.lstItems.FullRowSelect = true;
            this.lstItems.HideSelection = false;
            this.lstItems.LabelWrap = false;
            this.lstItems.Location = new Point(0, 25);
            this.lstItems.MultiSelect = false;
            this.lstItems.Name = "lstItems";
            this.lstItems.ShowGroups = false;
            this.lstItems.Size = new Size(594, 251);
            this.lstItems.TabIndex = 1;
            this.lstItems.UseCompatibleStateImageBehavior = false;
            this.lstItems.View = View.Details;
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.OnListViewSelectedIndexChanged);
            // 
            // clhAction
            // 
            this.clhAction.Text = "Action";
            // 
            // clhMessage
            // 
            this.clhMessage.Text = "Message";
            // 
            // clhResult
            // 
            this.clhResult.Text = "Result";
            // 
            // WorkResultUserControl
            // 
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.tbsMain);
            this.Name = "WorkResultUserControl";
            this.Size = new Size(594, 276);
            this.tbsMain.ResumeLayout(false);
            this.tbsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Private Classes

        private class ItemsListView : ListView
        {
            public ItemsListView()
                : base()
            {
                // Have a flicker free list view.
                base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

                base.Font = new Font("Consolas", base.Font.Size + 1);
            }

            public void Append(WorkResultListItem value, Boolean follow)
            {
                try
                {
                    base.BeginUpdate();

                    ListViewItem item = new ListViewItem(new String[] { value.Action, value.Message, value.Result.Display });

                    item.Tag = value;
                    item.BackColor = value.Background;
                    item.ForeColor = value.Foreground;

                    base.Items.Add(item);

                    if (follow && base.Items.Count > 0)
                    {
                        base.EnsureVisible(base.Items.Count - 1);
                    }
                }
                finally
                {
                    base.EndUpdate();
                }
            }

            protected override void OnMouseDoubleClick(MouseEventArgs args)
            {
                base.OnMouseDoubleClick(args);

                if (!(base.HitTest(args.Location)?.Item?.Tag is WorkResultListItem item))
                {
                    return;
                }

                MessageBox.Show(this.TopLevelControl, item.ToMessage(), this.TopLevelControl.Text);
            }
        }

        #endregion
    }
}

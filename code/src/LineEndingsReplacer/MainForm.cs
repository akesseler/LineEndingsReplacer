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

using Plexdata.LineEndingsReplacer.Defines;
using Plexdata.LineEndingsReplacer.Dialogs;
using Plexdata.LineEndingsReplacer.Entities;
using Plexdata.LineEndingsReplacer.Events;
using Plexdata.LineEndingsReplacer.Extensions;
using Plexdata.LineEndingsReplacer.Helpers;
using Plexdata.LineEndingsReplacer.Processing;
using Plexdata.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer
{
    public partial class MainForm : Form
    {
        #region Private Fields

        private const String buttonPlayText = "Play";
        private const String buttonPlayTool = "Execute tool with current settings.";
        private const String buttonStopText = "Stop";
        private const String buttonStopTool = "Stop current execution.";

        private Settings settings;
        private WorkProcessor processor;

        #endregion

        #region Construction

        public MainForm()
            : base()
        {
            this.InitializeComponent();

            this.Text = InfoDialog.Title;

            this.processor = new WorkProcessor(this);
            this.processor.ProcessingProgress += this.OnProcessingProgress;
            this.processor.ProcessingCanceled += this.OnProcessingCanceled;
            this.processor.ProcessingFinished += this.OnProcessingFinished;
            this.processor.ProcessingException += this.OnProcessingException;

            this.txtExcludes.Font = new Font("Consolas", 9);

            this.RestoreDefaults();
        }

        #endregion

        #region Parent Overrides

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            try
            {
                this.settings = SettingsManager.Load();

                this.ApplySettings();

                this.CheckButtons();

                this.tbbSort.Enabled = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs args)
        {
            this.processor.Cancel();
            this.processor.Dispose();
            this.processor = null;

            try
            {
                this.StoreSettings();

                SettingsManager.Save(this.settings);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            base.OnClosing(args);
        }

        #endregion

        #region Event Handlers

        private void OnButtonExitClick(Object sender, EventArgs args)
        {
            this.Close();
            Application.Exit();
        }

        private void OnButtonPlayClick(Object sender, EventArgs args)
        {
            if (this.processor.IsRunning)
            {
                this.processor.Cancel();
                return;
            }

            String message = String.Format(
                "Do you really want to replace all line endings in all fitting files by {1}?{0}{0}" +
                "Please be aware, this operation cannot be undone!",
                Environment.NewLine, this.GetSelectedEndingDisplayValue());

            DialogResult result = MessageBox.Show(this, message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.No)
            {
                return;
            }

            String initial = this.txtFolder.Text.Trim();
            String replace = this.GetSelectedEndingUtilizeValue();
            String[] exclude = this.txtExcludes.Lines.Where(x => !String.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();

            WorkValues values = new WorkValues(initial, replace, exclude);

            this.processor.Start(values);

            if (this.processor.IsRunning)
            {
                this.lstResults.Reset();
                this.Cursor = Cursors.AppStarting;
                this.tbbPlay.Text = MainForm.buttonStopText;
                this.tbbPlay.ToolTipText = MainForm.buttonStopTool;
                this.tbbPlay.Image = Properties.Resources.ButtonStop;
            }
        }

        private void OnButtonScanClick(Object sender, EventArgs args)
        {
            ScanDialog dialog = new ScanDialog()
            {
                Source = this.txtFolder.Text
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txtExcludes.Lines = dialog.Excluded.ToArray();
            }
        }

        private void OnButtonSortClick(Object sender, EventArgs args)
        {
            if (sender is ToolStripButton button)
            {
                if (button.Tag is null)
                {
                    button.Tag = SortOrder.Ascending;
                }

                SortOrder order = (SortOrder)button.Tag;

                if (order == SortOrder.Ascending)
                {
                    button.Tag = SortOrder.Descending;
                }
                else
                {
                    button.Tag = SortOrder.Ascending;
                }

                if (this.txtExcludes.Focused)
                {
                    this.txtExcludes.Lines = this.SortLines(this.txtExcludes.Lines, order);
                }
            }
        }

        private void OnButtonUndoClick(Object sender, EventArgs args)
        {
            this.settings = new Settings();

            this.ApplySettings();
        }

        private void OnButtonInfoClick(Object sender, EventArgs args)
        {
            InfoDialog dialog = new InfoDialog();

            dialog.ShowDialog(this);
        }

        private void OnFolderTextChanged(Object sender, EventArgs args)
        {
            this.CheckButtons();
        }

        private void OnButtonFolderClick(Object sender, EventArgs args)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                SelectedPath = this.txtFolder.Text,
                ShowNewFolderButton = false,
                Description = "Choose the top folder to start from."
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.txtFolder.Text = dialog.SelectedPath;
            }
        }

        private void OnSelectedValueChanged(Object sender, EventArgs args)
        {
            if (sender == this.cmbEnding)
            {
                ComboBox affected = sender as ComboBox;

                if (affected.SelectedItem is AnnotationAttribute annotation)
                {
                    this.tipMain.SetToolTip(affected, annotation.Remarks);
                }
            }
        }

        private void OnTextBoxEnter(Object sender, EventArgs args)
        {
            this.tbbSort.Enabled = true;
        }

        private void OnTextBoxLeave(Object sender, EventArgs args)
        {
            this.tbbSort.Enabled = false;
        }

        private void OnProcessingProgress(Object sender, WorkResultEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args);

            if (args.IsFolder)
            {
                this.sblMain.Text = args.FullPath;
            }

            this.lstResults.Append(new WorkResultListItem(args));
        }

        private void OnProcessingCanceled(Object sender, EventArgs args)
        {
            this.lstResults.Append(new WorkResultListItem(true));

            this.RestoreDefaults();
        }

        private void OnProcessingFinished(Object sender, EventArgs args)
        {
            this.lstResults.Append(new WorkResultListItem(false));

            this.RestoreDefaults();
        }

        private void OnProcessingException(Object sender, ExceptionEventArgs args)
        {
            this.lstResults.Append(new WorkResultListItem(args));

            if (!args.IsAborted)
            {
                System.Diagnostics.Debug.WriteLine(args);
                return;
            }

            this.RestoreDefaults();
        }

        #endregion

        #region Private Methods

        private void RestoreDefaults()
        {
            this.Cursor = Cursors.Default;
            this.sblMain.Text = String.Empty;
            this.tbbPlay.Text = MainForm.buttonPlayText;
            this.tbbPlay.ToolTipText = MainForm.buttonPlayTool;
            this.tbbPlay.Image = Properties.Resources.ButtonPlay;
            this.lstResults.SetRunning(false);
        }

        private void ApplySettings()
        {
            List<AnnotationAttribute> annotations = typeof(LineEndingType).GetAnnotationAttributes().ToList();

            this.cmbEnding.DataSource = annotations;
            this.cmbEnding.ValueMember = nameof(AnnotationAttribute.Utilize);
            this.cmbEnding.DisplayMember = nameof(AnnotationAttribute.Display);

            this.txtFolder.Text = this.settings.Folder;
            this.cmbEnding.SelectedIndex = annotations.FindIndex(x => x.Utilize.Equals(this.settings.Ending));
            this.txtExcludes.Lines = this.settings.Detach(this.settings.Excludes).ToArray();
        }

        private void StoreSettings()
        {
            this.settings.Folder = this.txtFolder.Text;
            this.settings.Ending = (LineEndingType)(this.cmbEnding.SelectedItem as AnnotationAttribute).Utilize;
            this.settings.Excludes = this.settings.Attach(this.txtExcludes.Lines);
        }

        private void CheckButtons()
        {
            Boolean enabled = this.txtFolder.TextLength > 0;

            this.tbbPlay.Enabled = enabled;
            this.tbbScan.Enabled = enabled;
        }

        private String[] SortLines(String[] array, SortOrder order)
        {
            if (array == null)
            {
                return new String[0];
            }

            if (order == SortOrder.Ascending)
            {
                Array.Sort(array, new ArrayComparer(true));
            }
            else if (order == SortOrder.Descending)
            {
                Array.Sort(array, new ArrayComparer(false));
            }

            return array;
        }

        private String GetSelectedEndingDisplayValue()
        {
            return (this.cmbEnding.SelectedItem as AnnotationAttribute).Display;
        }

        private String GetSelectedEndingUtilizeValue()
        {
            switch ((LineEndingType)(this.cmbEnding.SelectedItem as AnnotationAttribute).Utilize)
            {
                case LineEndingType.CRLF:
                    return "\r\n";
                case LineEndingType.CR:
                    return "\r";
                case LineEndingType.LF:
                    return "\n";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}

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

using Plexdata.LineEndingsReplacer.Events;
using Plexdata.LineEndingsReplacer.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Dialogs
{
    public partial class ScanDialog : Form
    {
        #region Private Fields

        private ScanProcessor processor;

        #endregion

        #region Construction

        public ScanDialog()
            : base()
        {
            this.InitializeComponent();

            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.processor = new ScanProcessor(this);
            this.processor.ProcessScanResult += this.OnProcessScanResult;
            this.processor.ProcessingCanceled += this.OnProcessingCanceled;
            this.processor.ProcessingFinished += this.OnProcessingFinished;
            this.processor.ProcessingException += this.OnProcessingException;

            this.btnExclude.Enabled = false;
            this.btnRestore.Enabled = false;
            this.btnAccept.Enabled = false;
        }

        #endregion

        #region Public Properties

        public String Source
        {
            get
            {
                return this.txtSource.Text;
            }
            set
            {
                this.txtSource.Text = (value ?? String.Empty).Trim();
            }
        }

        public IEnumerable<String> Excluded
        {
            get
            {
                return this.lstItems.Excluded;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnClosing(CancelEventArgs args)
        {
            this.processor.Cancel();
            this.processor.Dispose();
            this.processor = null;

            base.OnClosing(args);
        }

        #endregion

        #region Event Handlers

        private void OnProcessScanResult(Object sender, ScanResultEventArgs args)
        {
            this.lstItems.AddScanResult(args.Value);
        }

        private void OnProcessingCanceled(Object sender, EventArgs args)
        {
            this.RestoreDefaults();
        }

        private void OnProcessingFinished(Object sender, EventArgs args)
        {
            this.RestoreDefaults();
        }

        private void OnProcessingException(Object sender, ExceptionEventArgs args)
        {
            // NOTE: Hitting this method has never been seen.
            MessageBox.Show(this, args.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.RestoreDefaults();
        }

        private void OnButtonStartClick(Object sender, EventArgs args)
        {
            if (this.processor.IsRunning)
            {
                this.processor.Cancel();
                this.RestoreDefaults();
                return;
            }

            this.lstItems.Items.Clear();
            this.btnExclude.Enabled = false;
            this.btnRestore.Enabled = false;
            this.btnAccept.Enabled = false;

            this.processor.Start(this.Source);

            if (this.processor.IsRunning)
            {
                this.Cursor = Cursors.AppStarting;
                this.btnStart.Text = "&Stop";
            }
        }

        private void OnButtonExcludeClick(Object sender, EventArgs args)
        {
            this.lstItems.MarkAsExcluded(true);
        }

        private void OnButtonRestoreClick(Object sender, EventArgs args)
        {
            this.lstItems.RestoreMarking();
        }

        #endregion

        #region Private Methods

        private void RestoreDefaults()
        {
            this.btnStart.Text = "&Start";
            this.Cursor = Cursors.Default;

            Boolean enabled = this.lstItems.Items.Count > 0;

            this.btnExclude.Enabled = enabled;
            this.btnRestore.Enabled = enabled;
            this.btnAccept.Enabled = enabled;
        }

        #endregion
    }
}

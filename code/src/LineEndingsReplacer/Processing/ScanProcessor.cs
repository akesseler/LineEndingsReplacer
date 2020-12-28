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
using Plexdata.LineEndingsReplacer.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Processing
{
    internal class ScanProcessor : IDisposable
    {
        #region Public Events

        public event ScanResultEventHandler ProcessScanResult;
        public event EventHandler ProcessingCanceled;
        public event EventHandler ProcessingFinished;
        public event ExceptionEventHandler ProcessingException;

        #endregion

        #region Private Fields

        private readonly Control control;
        private CancellationTokenSource cancellation;

        #endregion

        #region Construction

        public ScanProcessor(Control control)
            : base()
        {
            this.IsRunning = false;
            this.IsDisposed = false;

            this.control = control;
            this.cancellation = null;
        }

        ~ScanProcessor()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public Boolean IsRunning { get; private set; }

        public Boolean IsDisposed { get; private set; }

        #endregion

        #region Public Methods

        public void Start(String source)
        {
            if (this.IsRunning || this.IsDisposed || String.IsNullOrWhiteSpace(source))
            {
                return;
            }

            if (this.cancellation != null)
            {
                this.cancellation.Dispose();
                this.cancellation = null;
            }

            this.IsRunning = true;

            this.cancellation = new CancellationTokenSource();

            CancellationToken token = this.cancellation.Token;

            Task.Run(() => this.Execute(source, token), token);
        }

        public void Cancel()
        {
            if (!this.IsDisposed && this.IsRunning)
            {
                this.cancellation.Cancel();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(Boolean disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.cancellation?.Cancel();
                this.cancellation?.Dispose();
                this.cancellation = null;
            }

            this.IsDisposed = true;
        }

        #endregion

        #region Private Methods

        private void Execute(String source, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                DirectoryInfo folder = new DirectoryInfo(source);

                foreach (FileSystemInfo current in folder.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    this.DebugDelay(10);

                    this.RaiseProcessScanResult(Path.GetFullPath(current.FullName));

                    token.ThrowIfCancellationRequested();
                }

                this.RaiseProcessingFinished();
            }
            catch (OperationCanceledException)
            {
                this.RaiseProcessingCanceled();
            }
            catch (ObjectDisposedException)
            {
                this.RaiseProcessingCanceled();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                this.RaiseProcessingException(exception);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private void RaiseProcessScanResult(String fullpath)
        {
            if (this.control == null || this.ProcessScanResult == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessScanResult(fullpath); }));
                return;
            }

            this.ProcessScanResult.Invoke(this, new ScanResultEventArgs(new ScanResult(fullpath)));
        }

        private void RaiseProcessingCanceled()
        {
            if (this.control == null || this.ProcessingCanceled == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessingCanceled(); }));
                return;
            }

            this.ProcessingCanceled.Invoke(this, EventArgs.Empty);
        }

        private void RaiseProcessingFinished()
        {
            if (this.control == null || this.ProcessingFinished == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessingFinished(); }));
                return;
            }

            this.ProcessingFinished.Invoke(this, EventArgs.Empty);
        }

        private void RaiseProcessingException(Exception exception)
        {
            if (this.control == null || this.ProcessingException == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessingException(exception); }));
                return;
            }

            this.ProcessingException.Invoke(this, new ExceptionEventArgs(exception));
        }

        [Conditional("DEBUG")]
        private void DebugDelay(Int32 delay)
        {
            Thread.Sleep(delay);
        }

        #endregion
    }
}

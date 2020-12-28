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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Processing
{
    internal class WorkProcessor
    {
        #region Public Events

        public event WorkResultEventHandler ProcessingProgress;
        public event EventHandler ProcessingCanceled;
        public event EventHandler ProcessingFinished;
        public event ExceptionEventHandler ProcessingException;

        #endregion

        #region Private Fields

        private readonly Control control;
        private CancellationTokenSource cancellation;
        private readonly FileProcessor processor;

        private String[] pathExcludes;
        private String[] fileExcludes;

        #endregion

        #region Construction

        public WorkProcessor(Control control)
            : base()
        {
            this.IsRunning = false;
            this.IsDisposed = false;

            this.control = control;
            this.cancellation = null;

            this.processor = new FileProcessor();
        }

        ~WorkProcessor()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public Boolean IsRunning { get; private set; }

        public Boolean IsDisposed { get; private set; }

        #endregion

        #region Public Methods

        public void Start(WorkValues values)
        {
            if (this.IsRunning || this.IsDisposed || values == null)
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

            Task.Run(() => this.Execute(values, token), token);
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

        private void Execute(WorkValues values, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                this.pathExcludes = values.Exclude.Select(x => x.Trim()).Where(x => !x.StartsWith("*")).ToArray();
                DebugHelper.DumpAffected("Path", "Exclude", this.pathExcludes);

                this.fileExcludes = values.Exclude.Select(x => x.Trim()).Where(x => x.StartsWith("*")).Select(x => x.Remove(0, 1)).ToArray();
                DebugHelper.DumpAffected("File", "Exclude", this.fileExcludes);

                this.Execute(token, new DirectoryInfo(values.Initial), values.Replace);

                token.ThrowIfCancellationRequested();

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
                this.RaiseProcessingException(exception, true, null);
            }
            finally
            {
                this.IsRunning = false;
            }
        }

        private void Execute(CancellationToken token, DirectoryInfo folder, String ending)
        {
            foreach (FileSystemInfo current in folder.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly))
            {
                token.ThrowIfCancellationRequested();

                DebugHelper.DebugDelay(10);

                if (current is DirectoryInfo pathInfo)
                {
                    if (this.IsExcluded(pathInfo, this.pathExcludes))
                    {
                        this.RaiseProcessingProgress(pathInfo, true, new WorkResult($"Folder excluded."));
                        continue;
                    }

                    this.RaiseProcessingProgress(pathInfo, false, new WorkResult($"Folder processed."));
                    this.Execute(token, pathInfo, ending);
                    continue;
                }

                if (current is FileInfo fileInfo)
                {
                    if (this.IsExcluded(fileInfo, this.fileExcludes))
                    {
                        this.RaiseProcessingProgress(fileInfo, true, new WorkResult($"File excluded."));
                        continue;
                    }

                    this.Replace(token, fileInfo, ending);
                    continue;
                }

                // TODO: Report that item is neither a path nor a file.
            }
        }

        private void Replace(CancellationToken token, FileInfo source, String ending)
        {
            FileInfo target = this.GetTargetFileInfo(source);

            String cleanup = target.FullName;

            try
            {
                Debug.WriteLine($"Cancellation: {token.IsCancellationRequested}, EOL: {ending.Replace("\r", "\\r").Replace("\n", "\\n")}");

                DebugHelper.DebugException(5);

                WorkResult result = this.processor.Process(token, source, target, ending);

                Debug.WriteLine(result);

                this.Rename(source, target);

                // Source and target file are the same from here!

                this.RaiseProcessingProgress(source, false, result);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                this.RaiseProcessingException(exception, false, source);
            }
            finally
            {
                try { if (File.Exists(cleanup)) { File.Delete(cleanup); } } catch { }
            }
        }

        private void Rename(FileInfo source, FileInfo target)
        {
            // None of these exceptions should ever happen, but safety first!

            if (!String.Equals(source.DirectoryName, target.DirectoryName, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException(
                    String.Format("Mismatch of source and target path.{0}Source: \"{1}\"{0}Target: \"{2}\"",
                    Environment.NewLine, source.DirectoryName, target.DirectoryName));
            }

            if (!source.Exists)
            {
                throw new FileNotFoundException("Source file not found.", source.FullName);
            }

            if (!target.Exists)
            {
                throw new FileNotFoundException("Target file not found.", target.FullName);
            }

            Debug.WriteLine($"Try to rename file \"{target.Name}\" into \"{source.Name}\" (path: \"{source.DirectoryName}\").");

            source.Delete();

            target.MoveTo(source.FullName);
        }

        private void RaiseProcessingProgress(FileSystemInfo source, Boolean excluded, WorkResult result)
        {
            if (this.control == null || this.ProcessingProgress == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessingProgress(source, excluded, result); }));
                return;
            }

            this.ProcessingProgress.Invoke(this, new WorkResultEventArgs(source, excluded, result ?? WorkResult.Empty));
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

        private void RaiseProcessingException(Exception exception, Boolean aborted, FileSystemInfo source)
        {
            if (this.control == null || this.ProcessingException == null)
            {
                return;
            }

            if (this.control.InvokeRequired)
            {
                this.control.Invoke(new MethodInvoker(() => { this.RaiseProcessingException(exception, aborted, source); }));
                return;
            }

            this.ProcessingException.Invoke(this, new ExceptionEventArgs(exception, aborted, source));
        }

        private FileInfo GetTargetFileInfo(FileInfo source)
        {
            return new FileInfo(Path.Combine(source.DirectoryName, Path.GetFileName(Path.GetRandomFileName())));
        }

        private Boolean IsExcluded(DirectoryInfo source, String[] excludes)
        {
            // Exclude list is empty => not considered as excluded.
            if (excludes.Length < 1) { return false; }

            foreach (String current in excludes)
            {
                if (String.Equals(source.Name, current, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            // Not found in exclude list => not considered as excluded.
            return false;
        }

        private Boolean IsExcluded(FileInfo source, String[] excludes)
        {
            // Exclude list is empty => not considered as excluded.
            if (excludes.Length < 1) { return false; }

            foreach (String current in excludes)
            {
                if (String.Equals(source.Extension, current, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            // Not found in exclude list => not considered as excluded.
            return false;
        }

        #endregion

        #region Private Classes

        private static class DebugHelper
        {
            private static Int32 debugExceptionCount = 0;

            [Conditional("DEBUG")]
            public static void DebugException(Int32 throwWhenCountIsEqual)
            {
                DebugHelper.debugExceptionCount++;

                if (DebugHelper.debugExceptionCount % throwWhenCountIsEqual == 0)
                {
                    throw new NotSupportedException("Debug helper test exception will not occur in release build.");
                }
            }

            [Conditional("DEBUG")]
            public static void DumpAffected(String type, String kind, String[] values)
            {
                Debug.WriteLine($"{type}: {kind} => \"{String.Join(";", values)}\"");
            }

            [Conditional("DEBUG")]
            public static void DebugDelay(Int32 delay)
            {
                Thread.Sleep(delay);
            }
        }

        #endregion
    }
}

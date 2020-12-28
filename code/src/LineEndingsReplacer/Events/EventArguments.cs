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
using System.IO;

namespace Plexdata.LineEndingsReplacer.Events
{
    public class ScanResultEventArgs : EventArgs
    {
        public ScanResultEventArgs(ScanResult value)
            : base()
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ScanResult Value { get; }

        public override String ToString()
        {
            return $"{nameof(ScanResultEventArgs)}: {this.Value}";
        }
    }

    public class WorkResultEventArgs : EventArgs
    {
        public WorkResultEventArgs(FileSystemInfo source, Boolean excluded, WorkResult result)
            : base()
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.IsFolder = source is DirectoryInfo;
            this.IsExcluded = excluded;
            this.Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public FileSystemInfo Source { get; }

        public Boolean IsExcluded { get; }

        public Boolean IsFolder { get; }

        public String FullPath
        {
            get
            {
                return this.Source.FullName;
            }
        }

        public String ItemName
        {
            get
            {
                return this.Source.Name;
            }
        }

        public WorkResult Result { get; }

        public override String ToString()
        {
            return $"{nameof(WorkResultEventArgs)}: {nameof(this.IsExcluded)}: {this.IsExcluded}, {nameof(this.IsFolder)}: {this.IsFolder}, {nameof(this.FullPath)}: {this.FullPath}";
        }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception exception)
            : this(exception, true, null)
        {
        }

        public ExceptionEventArgs(Exception exception, Boolean aborted, FileSystemInfo source)
            : base()
        {
            this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            this.IsAborted = aborted;
            this.Source = source;
        }

        public Exception Exception { get; }

        public Boolean IsAborted { get; }

        public String Message
        {
            get
            {
                return this.Exception.Message;
            }
        }

        public FileSystemInfo Source { get; }

        public Boolean HasSource
        {
            get
            {
                return this.Source != null;
            }
        }

        public String FullPath
        {
            get
            {
                return this.Source?.FullName ?? String.Empty;
            }
        }

        public String ItemName
        {
            get
            {
                return this.Source?.Name ?? String.Empty;
            }
        }

        public override String ToString()
        {
            return $"{nameof(ExceptionEventArgs)}: {nameof(this.IsAborted)}: {this.IsAborted}, {nameof(this.Message)}: {this.Message}";
        }
    }
}

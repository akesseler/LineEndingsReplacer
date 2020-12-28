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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Text;

namespace Plexdata.LineEndingsReplacer.Entities
{
    public class WorkResultListItem
    {
        #region Private Fields

        public const String ActionCanceled = "Canceled";
        public const String ActionFinished = "Finished";
        public const String ActionDeclined = "Declined";
        public const String ActionExcluded = "Excluded";
        public const String ActionIncluded = "Included";

        #endregion

        #region Construction

        public WorkResultListItem(Boolean canceled)
        {
            this.Action = this.GetResultType(canceled);
            this.Message = this.GetMessage(canceled);
            this.Exception = null;
            this.Source = null;
            this.Result = WorkResult.Empty;
            this.Foreground = this.GetForeground(canceled);
            this.Background = this.GetBackground(canceled);
        }

        public WorkResultListItem(WorkResultEventArgs value)
        {
            this.Action = this.GetResultType(value);
            this.Message = this.GetMessage(value);
            this.Exception = null;
            this.Source = value.Source;
            this.Result = value.Result;
            this.Foreground = this.GetForeground(value);
            this.Background = this.GetBackground(value);
        }

        public WorkResultListItem(ExceptionEventArgs value)
        {
            this.Action = this.GetResultType(value);
            this.Message = this.GetMessage(value);
            this.Exception = value.Exception;
            this.Source = value.Source;
            this.Result = new WorkResult($"Error: \"{value.Exception.Message}\"");
            this.Foreground = this.GetForeground(value);
            this.Background = this.GetBackground(value);
        }

        #endregion

        #region Public Properties

        public String Action { get; }

        public String Message { get; }

        public Exception Exception { get; }

        public FileSystemInfo Source { get; }

        public WorkResult Result { get; }

        public Color Foreground { get; }

        public Color Background { get; }

        public Boolean IsCanceled
        {
            get
            {
                return String.Equals(this.Action, WorkResultListItem.ActionCanceled);
            }
        }

        public Boolean IsFinished
        {
            get
            {
                return String.Equals(this.Action, WorkResultListItem.ActionFinished);
            }
        }

        public Boolean IsDeclined
        {
            get
            {
                return String.Equals(this.Action, WorkResultListItem.ActionDeclined);
            }
        }

        public Boolean IsExcluded
        {
            get
            {
                return String.Equals(this.Action, WorkResultListItem.ActionExcluded);
            }
        }

        public Boolean IsIncluded
        {
            get
            {
                return String.Equals(this.Action, WorkResultListItem.ActionIncluded);
            }
        }

        #endregion

        #region Public Methods

        public String ToMessage()
        {
            StringBuilder builder = new StringBuilder(128);

            if (this.IsDeclined)
            {
                if (this.Exception != null)
                {
                    builder.AppendLine("An exception with message below has occurred while processing item!");
                    builder.AppendLine();

                    builder.AppendLine(this.Exception.Message);
                    builder.AppendLine();

                    this.AddSourceToMessage(builder);
                }
                else
                {
                    builder.AppendLine("An unspecified error has occurred while processing item!");
                    builder.AppendLine();

                    this.AddSourceToMessage(builder);
                }
            }
            else if (this.IsExcluded)
            {
                builder.AppendLine("Current item has been excluded and remains unchanged.");
                builder.AppendLine();

                this.AddSourceToMessage(builder);
            }
            else if (this.IsIncluded)
            {
                builder.AppendLine("Current item has been included and was changed as shown below.");
                builder.AppendLine();

                builder.AppendLine(this.Result?.ToMessage() ?? String.Empty);

                this.AddSourceToMessage(builder);
            }
            else
            {
                builder.AppendLine(this.Message);
            }

            return builder.ToString().Trim();
        }

        public String[] GetReportCells()
        {
            String[] results = new String[4] { String.Empty, String.Empty, String.Empty, String.Empty };

            results[0] = this.Action;

            if (this.IsDeclined)
            {
                results[1] = this.GetReportOrigin(this.Exception);
                results[2] = this.GetReportDetails(this.Exception);
            }
            else if (this.IsExcluded)
            {
                results[1] = this.GetReportOrigin(this.Source);
                results[2] = this.Result.Display;
            }
            else if (this.IsIncluded)
            {
                results[1] = this.GetReportOrigin(this.Source);
                results[2] = this.GetReportDetails(this.Source);
            }
            else
            {
                results[1] = this.GetReportOrigin(null);
                results[2] = this.Message;
            }

            results[3] = this.GetReportSource();

            return results;
        }

        #endregion

        #region Private Methods

        private String GetResultType(Boolean canceled)
        {
            return canceled ? WorkResultListItem.ActionCanceled : WorkResultListItem.ActionFinished;
        }

        private String GetMessage(Boolean canceled)
        {
            return canceled ? "The operation has been canceled." : "The operation has finished.";
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private Color GetForeground(Boolean canceled)
        {
            return SystemColors.WindowText;
        }

        private Color GetBackground(Boolean canceled)
        {
            return Color.FromArgb(canceled ? unchecked((Int32)0xFFFFBFBF) : unchecked((Int32)0xFFA5FFA5));
        }

        private String GetResultType(WorkResultEventArgs value)
        {
            if (value.IsExcluded)
            {
                return WorkResultListItem.ActionExcluded;
            }

            return WorkResultListItem.ActionIncluded;
        }

        private String GetMessage(WorkResultEventArgs value)
        {
            StringBuilder builder = new StringBuilder(128);

            if (value.IsFolder)
            {
                builder.AppendFormat("Folder: \"{0}\"", value.ItemName);
            }
            else
            {
                builder.AppendFormat("File: \"{0}\"", value.ItemName);
            }

            return builder.ToString();
        }

        private Color GetForeground(WorkResultEventArgs value)
        {
            if (value.IsExcluded)
            {
                return SystemColors.WindowText;
            }

            return SystemColors.WindowText;
        }

        private Color GetBackground(WorkResultEventArgs value)
        {
            if (value.IsExcluded)
            {
                return Color.FromArgb(unchecked((Int32)0xFFFFFFCC));
            }

            return Color.White;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private String GetResultType(ExceptionEventArgs value)
        {
            return WorkResultListItem.ActionDeclined;
        }

        private String GetMessage(ExceptionEventArgs value)
        {
            StringBuilder builder = new StringBuilder(128);

            builder.Append("An error occurred");

            if (value.HasSource)
            {
                builder.AppendFormat(" while processing \"{0}\"", Path.GetFileName(value.ItemName));
            }

            builder.Append($". Operation {(value.IsAborted ? "has been aborted" : "continues with next item")}.");

            return builder.ToString();
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private Color GetForeground(ExceptionEventArgs value)
        {
            return SystemColors.WindowText;
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private Color GetBackground(ExceptionEventArgs value)
        {
            return Color.FromArgb(unchecked((Int32)0xFFFF7F7F));
        }

        private void AddSourceToMessage(StringBuilder builder)
        {
            if (this.Source is FileInfo)
            {
                builder.AppendLine($"File: {this.Source.Name}");
                builder.AppendLine($"Path: {Path.GetDirectoryName(this.Source.FullName)}");
                builder.AppendLine();
            }
            else if (this.Source is DirectoryInfo)
            {
                builder.AppendLine($"Path: {this.Source.FullName}");
                builder.AppendLine();
            }
        }

        private String GetReportOrigin(Object value)
        {
            if (value is Exception)
            {
                return "Error";
            }
            else if (value is FileInfo)
            {
                return "File";
            }
            else if (value is DirectoryInfo)
            {
                return "Folder";
            }
            else if (value == null && (this.IsCanceled || this.IsFinished))
            {
                return "Global";
            }
            else
            {
                return "Unknown";
            }
        }

        private String GetReportDetails(Object value)
        {
            if (value is Exception)
            {
                if (value != null)
                {
                    return this.Exception.Message;
                }
                else
                {
                    return "An unspecified error has occurred.";
                }
            }
            else if (this.Source is FileInfo)
            {
                return this.Result.ToReport();
            }
            else if (this.Source is DirectoryInfo)
            {
                return this.Result.Display;
            }
            else
            {
                return "Unknown";
            }
        }

        private String GetReportSource()
        {
            if (this.Source != null)
            {
                return $"\"{this.Source.FullName}\"";
            }
            else if (this.IsCanceled || this.IsFinished)
            {
                return "Processing";
            }
            else
            {
                return "Unknown";
            }
        }

        #endregion
    }
}

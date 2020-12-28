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

using System;
using System.Collections.Generic;
using System.Text;

namespace Plexdata.LineEndingsReplacer.Entities
{
    public class WorkResult
    {
        public static WorkResult Empty = new WorkResult();

        private WorkResult()
            : base()
        {
            this.IsEmpty = true;
            this.Replacement = String.Empty;
            this.TotalLineCount = 0;
            this.SingleCarriageReturns = 0;
            this.SingleLineFeeds = 0;
            this.CarriageReturnsPlusLineFeeds = 0;
            this.Display = String.Empty;
        }

        public WorkResult(Byte[] replacement, Int32 totalCount, Int32 countCR, Int32 countLF, Int32 countCRLF)
            : base()
        {
            this.IsEmpty = false;
            this.Replacement = this.GetReplacement(replacement);
            this.TotalLineCount = totalCount;
            this.SingleCarriageReturns = countCR;
            this.SingleLineFeeds = countLF;
            this.CarriageReturnsPlusLineFeeds = countCRLF;
            this.Display = $"Total Lines Processed: {this.TotalLineCount:N0}";

            this.ThrowIfInvalid();
        }

        public WorkResult(String display)
            : base()
        {
            if (String.IsNullOrWhiteSpace(display))
            {
                throw new ArgumentOutOfRangeException(nameof(display), $"Parameter '{nameof(display)}' cannot be null or whitespace.");
            }

            this.IsEmpty = false;
            this.Replacement = String.Empty;
            this.TotalLineCount = 0;
            this.SingleCarriageReturns = 0;
            this.SingleLineFeeds = 0;
            this.CarriageReturnsPlusLineFeeds = 0;
            this.Display = display.Trim();
        }

        public Boolean IsEmpty { get; }

        public String Replacement { get; }

        public Int32 TotalLineCount { get; }

        public Int32 SingleCarriageReturns { get; }

        public Int32 SingleLineFeeds { get; }

        public Int32 CarriageReturnsPlusLineFeeds { get; }

        public String Display { get; }

        public String ToMessage()
        {
            if (this.IsEmpty)
            {
                return String.Empty;
            }

            StringBuilder builder = new StringBuilder(128);

            if (!String.IsNullOrWhiteSpace(this.Display))
            {
                builder.AppendLine(this.Display);
            }

            if (this.TotalLineCount > 0)
            {
                builder.AppendLine($"- CR:\t{this.SingleCarriageReturns:N0}\t(Single Carriage Returns)");
                builder.AppendLine($"- LF:\t{this.SingleLineFeeds:N0}\t(Single Line Feeds)");
                builder.AppendLine($"- CRLF:\t{this.CarriageReturnsPlusLineFeeds:N0}\t(Carriage Returns Plus Line Feeds)");
            }

            return builder.ToString();
        }

        public String ToReport()
        {
            if (this.IsEmpty)
            {
                return String.Empty;
            }

            if (this.TotalLineCount > 0)
            {
                return $"Total Lines: {this.TotalLineCount}, CR: {this.SingleCarriageReturns}, LF: {this.SingleLineFeeds}, CR+LF: {this.CarriageReturnsPlusLineFeeds}, Replacement: {this.Replacement}";
            }
            else
            {
                return this.Display;
            }
        }

        public override String ToString()
        {
            return $"Total Lines: {this.TotalLineCount}, CR: {this.SingleCarriageReturns}, LF: {this.SingleLineFeeds}, CR+LF: {this.CarriageReturnsPlusLineFeeds}, Replacement: {this.Replacement}";
        }

        private String GetReplacement(Byte[] replacement)
        {
            if (replacement is null || replacement.Length < 1)
            {
                throw new ArgumentNullException(nameof(replacement));
            }

            List<String> result = new List<String>();

            foreach (Byte current in replacement)
            {
                if (current == 0x0D)
                {
                    result.Add("CR");
                    continue;
                }

                if (current == 0x0A)
                {
                    result.Add("LF");
                    continue;
                }
            }

            return String.Join("+", result);
        }

        private void ThrowIfInvalid()
        {
            if (this.TotalLineCount != this.SingleCarriageReturns + this.SingleLineFeeds + this.CarriageReturnsPlusLineFeeds)
            {
                throw new InvalidOperationException("Sum of individual counts does not match the total count.");
            }
        }
    }
}

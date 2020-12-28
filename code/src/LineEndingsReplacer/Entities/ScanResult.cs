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
using System.IO;

namespace Plexdata.LineEndingsReplacer.Entities
{
    public class ScanResult
    {
        public ScanResult(String path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentOutOfRangeException(nameof(path), "Argument cannot be null or whitespace.");
            }

            this.ApplyProperties(path);
        }

        public String Label { get; private set; }

        public Boolean IsFolder { get; private set; }

        public override String ToString()
        {
            return $"{nameof(this.Label)}: {this.Label}, {nameof(this.IsFolder)}: {this.IsFolder}";
        }

        private void ApplyProperties(String path)
        {
            FileInfo helper = new FileInfo(path);

            this.IsFolder = helper.Attributes.HasFlag(FileAttributes.Directory);

            if (this.IsFolder)
            {
                this.Label = helper.Name;
            }
            else
            {
                this.Label = $"*{helper.Extension}";
            }
        }
    }
}

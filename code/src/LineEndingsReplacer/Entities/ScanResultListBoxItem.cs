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

using Plexdata.LineEndingsReplacer.Native;
using System;
using System.Drawing;

namespace Plexdata.LineEndingsReplacer.Entities
{
    internal class ScanResultListBoxItem : IEquatable<ScanResult>, IEquatable<ScanResultListBoxItem>, IFormattable
    {
        public ScanResultListBoxItem(ScanResult result)
            : base()
        {
            this.Source = result ?? throw new ArgumentNullException(nameof(result));
            this.Name = result.Label;
            this.Icon = WindowsHelpers.GetSystemIcon(this.Source.Label, true, this.Source.IsFolder, false);
            this.IsExcluded = false;
        }

        public ScanResult Source { get; private set; }

        public String Name { get; private set; }

        public Icon Icon { get; private set; }

        public Boolean IsExcluded { get; set; }

        public override Int32 GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override Boolean Equals(Object other)
        {
            if (other is ScanResult)
            {
                return this.Equals(other as ScanResult);
            }

            if (other is ScanResultListBoxItem)
            {
                return this.Equals(other as ScanResultListBoxItem);
            }

            return base.Equals(other);
        }

        public Boolean Equals(ScanResult other)
        {
            if (other == null)
            {
                return false;
            }

            return String.Equals(this.Source.Label, other.Label, StringComparison.InvariantCultureIgnoreCase);
        }

        public Boolean Equals(ScanResultListBoxItem other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Equals(other.Source);
        }

        public override String ToString()
        {
            return $"{nameof(this.Name)}: {this.Name}, {nameof(this.Source)}: [{this.Source}]";
        }

        public String ToString(String format, IFormatProvider provider)
        {
            // HACK: This is indeed a hack just to get items sorted.
            // At the moment there does not seem to be an other way 
            // to get list-box items sorted. Neither implementing 
            // IComparable nor implementing IComparer has any kind 
            // of effect. As well, method Sort is also not called.

            return $"{(this.Source.IsFolder ? "ZZZ" : "AAA")}-{this.Source.Label}";
        }
    }
}

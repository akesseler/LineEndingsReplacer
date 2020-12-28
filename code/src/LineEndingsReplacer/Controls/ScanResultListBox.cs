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
using Plexdata.LineEndingsReplacer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Controls
{
    public class ScanResultListBox : ListBox
    {
        private const Int32 padding = 5;
        private const Int32 maximum = 16;

        private readonly Bitmap IconExclude = null;
        private readonly Bitmap IconNeutral = null;

        public ScanResultListBox()
            : base()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            base.DoubleBuffered = true;
            base.DrawMode = DrawMode.OwnerDrawVariable;
            base.Sorted = true;
            base.ResizeRedraw = true;
            base.SelectionMode = SelectionMode.MultiExtended;
            base.FormattingEnabled = false;
            base.IntegralHeight = false;
            base.Font = new Font("Consolas", 9);

            this.IconExclude = Resources.IconExclude;
            this.IconNeutral = Resources.IconNeutral;
        }

        public IEnumerable<String> Excluded
        {
            get
            {
                foreach (Object item in this.Items)
                {
                    if (item is ScanResultListBoxItem scan && scan.IsExcluded)
                    {
                        yield return scan.Name;
                    }
                }
            }
        }

        public void AddScanResult(ScanResult result)
        {
            if (!base.Items.Contains(result))
            {
                base.Items.Add(new ScanResultListBoxItem(result));
            }
        }

        public void MarkAsExcluded(Boolean value)
        {
            foreach (Object current in base.SelectedItems)
            {
                if (current is ScanResultListBoxItem affected)
                {
                    affected.IsExcluded = value;
                }
            }

            this.Invalidate();
        }

        public void RestoreMarking()
        {
            foreach (Object current in base.Items)
            {
                if (current is ScanResultListBoxItem affected)
                {
                    affected.IsExcluded = false;
                }
            }

            this.Invalidate();
        }

        protected override void OnMeasureItem(MeasureItemEventArgs args)
        {
            args.ItemWidth = this.ClientSize.Width;
            args.ItemHeight = 2 * ScanResultListBox.padding + ScanResultListBox.maximum;
        }

        protected override void OnDrawItem(DrawItemEventArgs args)
        {
            this.DrawPrimer(args);

            if (args.Index < 0 || args.Index > base.Items.Count - 1)
            {
                return;
            }

            Object current = base.Items[args.Index];

            if (current is ScanResultListBoxItem affected)
            {
                this.DrawResult(args, affected);
            }
            else
            {
                this.DrawObject(args, current);
            }
        }

        private void DrawPrimer(DrawItemEventArgs args)
        {
            Brush brush = null;

            try
            {
                if (args.State.HasFlag(DrawItemState.Selected))
                {
                    RectangleF bounds = args.Bounds;

                    // Seems to be there are some rounding faults.
                    bounds.Y -= 1;
                    bounds.Height += 2;

                    brush = new LinearGradientBrush(bounds,
                        ControlPaint.Light(args.BackColor),
                        args.BackColor, LinearGradientMode.Vertical);

                    args.Graphics.FillRectangle(brush, args.Bounds);
                }
                else
                {
                    brush = new SolidBrush(args.BackColor);
                    args.Graphics.FillRectangle(brush, args.Bounds);
                }
            }
            finally
            {
                if (brush != null)
                {
                    brush.Dispose();
                    brush = null;
                }
            }
        }

        private void DrawResult(DrawItemEventArgs args, ScanResultListBoxItem value)
        {
            if (value == null)
            {
                return;
            }

            Font font = args.Font;
            Int32 x = args.Bounds.X + ScanResultListBox.padding;
            Int32 y = args.Bounds.Y + ScanResultListBox.padding;
            Int32 h = ScanResultListBox.maximum;
            Int32 w = args.Bounds.Width - 2 * ScanResultListBox.padding;

            if (value.Icon != null)
            {
                args.Graphics.DrawIcon(value.Icon, x, y);

                x += value.Icon.Width + ScanResultListBox.padding;

                w -= x;
            }

            TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

            Rectangle bounds = new Rectangle(x, y, w, h);

            bounds.Width -= this.DrawStatus(args, value, bounds.Top, bounds.Right);

            //args.Graphics.FillRectangle(Brushes.AliceBlue, bounds);

            TextRenderer.DrawText(args.Graphics, value.Name, font, bounds, args.ForeColor, flags);
        }

        private Int32 DrawStatus(DrawItemEventArgs args, ScanResultListBoxItem value, Int32 top, Int32 right)
        {
            Int32 x;
            Int32 y;
            Int32 w;
            Int32 h;

            Bitmap exclude = value.IsExcluded ? this.IconExclude : this.IconNeutral;

            x = right - exclude.Width;
            y = top;
            w = exclude.Width;
            h = exclude.Height;

            args.Graphics.DrawImage(exclude, new RectangleF(x, y, w, h));

            return ScanResultListBox.padding + exclude.Width;
        }

        private void DrawObject(DrawItemEventArgs args, Object value)
        {
            // Attention: This method just serves a fallback and might never be called. 
            // But it might be called anyway if items are added by skipping the call of 
            // method AddItem().

            if (value == null)
            {
                return;
            }

            TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.VerticalCenter;

            Int32 x = args.Bounds.X + ScanResultListBox.padding;
            Int32 y = args.Bounds.Y + ScanResultListBox.padding;
            Int32 h = args.Bounds.Height - 2 * ScanResultListBox.padding;
            Int32 w = args.Bounds.Width - 2 * ScanResultListBox.padding;

            Rectangle bounds = new Rectangle(x, y, w, h);

            TextRenderer.DrawText(args.Graphics, value.ToString(), args.Font, bounds, args.ForeColor, flags);
        }
    }
}

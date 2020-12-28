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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Plexdata.LineEndingsReplacer.Controls
{
    public class SplitContainerEx : SplitContainer
    {
        public SplitContainerEx()
            : base()
        {
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            base.OnKeyUp(args);

            // Needed to disable painted focus rectangle...
            base.Refresh();
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            try
            {
                Size size = new Size(4, 40); // Gripper dimension...
                Rectangle rect = base.SplitterRectangle;
                VisualStyleRenderer renderer = null;

                if (base.Orientation == Orientation.Vertical)
                {
                    rect.Y = (rect.Bottom - (rect.Top + size.Height)) / 2;
                    rect.Height = size.Height;
                    rect.Width = size.Width;
                    rect.X += Math.Max((base.SplitterWidth - rect.Width) / 2, 0) - 1;

                    if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Gripper.Normal))
                    {
                        renderer = new VisualStyleRenderer(VisualStyleElement.Rebar.Gripper.Normal);
                    }
                }
                else
                {
                    rect.X = (rect.Right - (rect.Left + size.Height)) / 2;
                    rect.Height = size.Width;
                    rect.Width = size.Height;
                    rect.Y += Math.Max((base.SplitterWidth - rect.Height) / 2, 0) - 1;

                    if (VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.GripperVertical.Normal))
                    {
                        renderer = new VisualStyleRenderer(VisualStyleElement.Rebar.GripperVertical.Normal);
                    }
                }

                if (renderer != null)
                {
                    renderer.DrawBackground(args.Graphics, rect, args.ClipRectangle);
                }

                if (base.Focused && base.TabStop)
                {
                    ControlPaint.DrawFocusRectangle(args.Graphics,
                        Rectangle.Inflate(base.SplitterRectangle, -1, -1),
                        base.ForeColor, base.BackColor);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        protected override void OnDoubleClick(EventArgs args)
        {
            base.OnDoubleClick(args);
            if (base.Orientation == Orientation.Vertical)
            {
                base.SplitterDistance = base.ClientSize.Width / 2;
            }
            else
            {
                base.SplitterDistance = base.ClientSize.Height / 2;
            }

            base.Refresh();
        }
    }
}

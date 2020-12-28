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

using Plexdata.LineEndingsReplacer.Defines;
using Plexdata.LineEndingsReplacer.Extensions;
using System;

namespace Plexdata.LineEndingsReplacer
{
    internal class Settings
    {
        #region Private Definitions

        public const Char Delimiter = ';';
        public static readonly Char[] Delimiters = new Char[] { Settings.Delimiter };
        private static readonly String[] DefaultExcludes = new String[] { ".vs", ".git", "bin", "obj", "packages", "help", "wiki", "*.png", "*.ico", "*.zip", "*.7z" };

        #endregion

        #region Private Fields

        private String folder = null;
        private String excludes = null;
        private LineEndingType ending = (LineEndingType)(-1);

        #endregion

        #region Construction

        public Settings()
            : base()
        {
            this.Folder = String.Empty;
            this.Excludes = this.Attach(Settings.DefaultExcludes);
            this.Ending = LineEndingType.Default;
        }

        #endregion

        #region Public Properties

        public String Folder
        {
            get
            {
                return this.folder;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }

                this.folder = value.Trim();
            }
        }

        public String Excludes
        {
            get
            {
                return this.excludes;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }

                this.excludes = value.Trim();
            }
        }

        public LineEndingType Ending
        {
            get
            {
                return this.ending;
            }
            set
            {
                if (!Enum.IsDefined(typeof(LineEndingType), value))
                {
                    value = LineEndingType.CRLF;
                }

                this.ending = value;
            }
        }

        #endregion
    }
}

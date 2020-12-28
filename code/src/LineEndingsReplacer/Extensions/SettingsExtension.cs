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

namespace Plexdata.LineEndingsReplacer.Extensions
{
    internal static class SettingsExtension
    {
        public static String Attach(this Settings settings, IEnumerable<String> source)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (source == null)
            {
                return String.Empty;
            }

            StringBuilder builder = new StringBuilder(512);

            foreach (String target in source)
            {
                String current = (target ?? String.Empty).Trim();

                if (current.Length > 0)
                {
                    builder.AppendFormat("{0}{1}", current, Settings.Delimiter);
                }
            }

            if (builder.Length > 0)
            {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        public static IEnumerable<String> Detach(this Settings settings, String source)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (source == null)
            {
                yield break;
            }

            foreach (String current in source.Split(Settings.Delimiters))
            {
                if (!String.IsNullOrWhiteSpace(current))
                {
                    yield return current.Trim();
                }
            }
        }
    }
}

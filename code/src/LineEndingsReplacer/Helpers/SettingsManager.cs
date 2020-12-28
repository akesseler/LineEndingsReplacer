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

using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;

namespace Plexdata.LineEndingsReplacer.Helpers
{
    internal static class SettingsManager
    {
        #region Public Methods

        public static Settings Load()
        {
            return SettingsManager.Load(SettingsManager.GetSettingsFilename());
        }

        public static Settings Load(String filename)
        {
            if (!File.Exists(filename))
            {
                return Activator.CreateInstance<Settings>();
            }

            using (StreamReader reader = File.OpenText(filename))
            {
                return JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd()) ?? Activator.CreateInstance<Settings>();
            }
        }

        public static void Save(Settings settings)
        {
            SettingsManager.Save(SettingsManager.GetSettingsFilename(), settings);
        }

        public static void Save(String filename, Settings settings)
        {
            if (settings == null)
            {
                return;
            }

            using (StreamWriter writer = File.CreateText(filename))
            {
                writer.Write(JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the fully qualified settings file name by preferring path of 
        /// the executable. The path of user's local application data is returned 
        /// in case of missing write permission on preferred path.
        /// </summary>
        /// <returns>
        /// The fully qualified settings file name.
        /// </returns>
        private static String GetSettingsFilename()
        {
            String site = Path.GetFullPath(Application.ExecutablePath);
            String file = Path.ChangeExtension(Path.GetFileName(site), ".settings");
            String path = Path.GetDirectoryName(site);

            if (SettingsManager.IsAccessPermitted(path, FileSystemRights.Write))
            {
                return Path.Combine(path, file);
            }

            if (String.IsNullOrWhiteSpace(Application.CompanyName))
            {
                // May never happen, but safety first...
                throw new ArgumentException("Company name is invalid.");
            }

            if (String.IsNullOrWhiteSpace(Application.ProductName))
            {
                // May never happen, but safety first...
                throw new ArgumentException("Product name is invalid.");
            }

            path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            path = Path.Combine(path, Application.CompanyName, Application.ProductName);

            Directory.CreateDirectory(path);

            return Path.Combine(path, file);
        }

        /// <summary>
        /// Checks whether access <paramref name="rights"/> are granted for 
        /// provided <paramref name="folder"/> path.
        /// </summary>
        /// <param name="folder">
        /// The folder path for which the access rights are to be determined.
        /// </param>
        /// <param name="rights">
        /// The bit-set of access rights to be evaluated.
        /// </param>
        /// <returns>
        /// True, if requested access rights are granted and false otherwise.
        /// </returns>
        private static Boolean IsAccessPermitted(String folder, FileSystemRights rights)
        {
            if (String.IsNullOrEmpty(folder))
            {
                return false;
            }

            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                AuthorizationRuleCollection rules = Directory
                    .GetAccessControl(folder)
                    .GetAccessRules(true, true, typeof(SecurityIdentifier));

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (identity.Groups.Contains(rule.IdentityReference))
                    {
                        if (rule.AccessControlType == AccessControlType.Allow && rule.FileSystemRights.HasFlag(rights))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        #endregion
    }
}

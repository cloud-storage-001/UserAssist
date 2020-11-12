using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;

namespace UserAssist
{
    class clsConfig
    {
        private const string fileName = "UserAssist.config";
        private const string configFileVersion = "1.0";
        private const string tag_version = "version";
        private const string tag_loadAtStartup = "loadAtStartup";

        private bool _loadAtStartup;

        public clsConfig(bool defaultLoadAtStartup)
        {
            _loadAtStartup = defaultLoadAtStartup;
            Load();
        }

        public bool loadAtStartup
        {
            get { return _loadAtStartup; }
            set 
            {
                if (_loadAtStartup != value)
                {
                    _loadAtStartup = value;
                    Save();
                }
            }
        }

        private void Load()
        {
            IsolatedStorageFileStream isfs;

            try
            {
                isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, FileAccess.Read);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(isfs);
                string version = doc.DocumentElement.Attributes[tag_version].Value;
                string savedLoadAtStartup = doc.DocumentElement.Attributes[tag_loadAtStartup].Value;
                bool result;
                if (bool.TryParse(savedLoadAtStartup, out result))
                    _loadAtStartup = result;
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168
            finally
            {
                isfs.Close();
                isfs.Dispose();
            }
        }

        private void Save()
        {
            IsolatedStorageFileStream isfs;

            try
            {
                isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.Write);
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168

            try
            {
                XmlTextWriter xmltw = new XmlTextWriter(isfs, Encoding.Unicode);
                xmltw.WriteStartElement("config");
                xmltw.WriteAttributeString(tag_version, configFileVersion);
                xmltw.WriteAttributeString(tag_loadAtStartup, _loadAtStartup.ToString());
                xmltw.WriteEndElement();
                xmltw.Close();
            }
#pragma warning disable 0168
            catch (Exception e)
            {
                return;
            }
#pragma warning restore 0168
            finally
            {
                isfs.Close();
                isfs.Dispose();
            }
        }
    }
}

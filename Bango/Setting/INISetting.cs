using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myro.Base.INI;
using Myro.Base;
using Myro.Base.List;

namespace Myro
{
    public class INISetting
    {
        #region Member variables.
        private string _fileName = string.Empty;
        private string _defaultSetting = string.Empty;
        private DictionaryFx<string, DynamicDictionary> _settings = new DictionaryFx<string, DynamicDictionary>();
        private Encoding _encode = Encoding.UTF8;
        private string[] _sections = null;
        private bool _isKeyCaseSensetive = true;
        #endregion

        #region Constructors & Finalizers.
        public INISetting(string settingFilePath)
            : this(settingFilePath, string.Empty)
        {
        }

        public INISetting(string settingFilePath, string defaultSetting)
        {
            _fileName = settingFilePath;
            _defaultSetting = defaultSetting;
        }
        #endregion

        #region Nested Enums, Structs, and Classes.
        #endregion

        #region Properties

        public string SettingFilePath
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public DictionaryFx<string, DynamicDictionary> Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public string this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public string this[string section, string key]
        {
            get { return GetValue(section, key); }
            set { SetValue(section, key, value); }
        }


        public Encoding Encode
        {
            get { return _encode; }
            set { _encode = value; }
        }

        /// <summary>
        /// Gets or Sets bool representing if the key of the setting is case sensetive or not.
        /// </summary>
        public bool IsKeyCaseSensetive
        {
            get { return _isKeyCaseSensetive; }
            set { _isKeyCaseSensetive = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Loads all the settings from INI file.
        /// </summary>
        /// <returns></returns>
        public bool Load(Encoding encode)
        {
            _encode = encode;
            return Load();
        }


        /// <summary>
        /// Loads all the settings from INI file.
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            CINI ini = GetINIObject(false);

            if (ini != null)
            {
                Reset(ini);

                if (_sections != null && _sections.Length > 0)
                {
                    foreach (string section in _sections)
                    {
                        Load(ini, section);
                    }
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Loads the content of a particular section from the INI file.
        /// </summary>
        /// <param name="section">Name of the section from where to load the settings.</param>
        /// <returns></returns>
        public bool Load(string section)
        {
            CINI ini = GetINIObject(false);
            if (ini != null)
            {
                Reset(ini);
                return Load(ini, section);
            }
            return false;
        }

        private void Reset(CINI ini)
        {
            _settings.Clear();
            if (_sections != null && _sections.Length > 0)
            {
                foreach (string itm in _sections)
                {
                    AddSection(itm);
                }
            }
        }

        private bool Load(CINI ini, string section)
        {
            if (ini != null)
            {
                //load the section contain only if the 
                if (_settings.ContainsKey(section))
                {
                    _settings[section] = new StringParamDict();

                    string[] entries = ini.GetEntryNames(section);
                    if (entries != null && entries.Length > 0)
                    {
                        foreach (string key in entries)
                        {
                            string lkey = key;
                            if (_isKeyCaseSensetive)
                                lkey = key.ToLower();

                            _settings[section][lkey] = ini.GetValue(section, key).ToString();
                        }
                    }
                }
            }

            return true;
        }

        public bool Save()
        {
            CINI ini = GetINIObject(false);

            if (ini != null)
            {

                foreach (string section in _settings.Keys)
                {
                    Save(ini, section);
                }
                return true;
            }

            return false;
        }

        public bool Save(string section)
        {
            CINI ini = GetINIObject(false);
            if (ini != null)
            {
                return Save(ini, section);
            }
            return false;
        }

        /// <summary>
        /// Saves the content of the INI file 
        /// </summary>
        /// <param name="ini"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        private bool Save(CINI ini, string section)
        {
            if (ini != null)
            {
                //load the section contain only if the section exists.
                if (_settings.ContainsKey(section))
                {
                    //saving each key and its value in ini file.
                    foreach (KeyValuePair<string, string> item in _settings[section])
                    {
                        ini.SetValue(section, item.Key, item.Value);
                    }
                }
            }
            return true;
        }

        public void AddSection(string section)
        {
            _settings.Add(section, new DynamicDictionary());
        }

        public void SetValue(string section, string key, string value)
        {
            DictionaryFx<string, string> dic = _settings[section];

            if (dic != null)
            {
                dic[key] = value;
            }
        }

        public string GetValue(string section, string key)
        {
            if (_isKeyCaseSensetive)
                key = key.ToLower();

            return _settings[section][key].ToString;
        }

        public void SetValue(string key, string value)
        {
            if (_isKeyCaseSensetive)
                key = key.ToLower();

            _settings[_defaultSetting][key] = value;
        }

        public string GetValue(string key)
        {
            if (_isKeyCaseSensetive)
                key = key.ToLower();

            return _settings[_defaultSetting][key];
        }

        private DictionaryFx<string, string> GetNewSection()
        {
            return new DictionaryFx<string, string>();
        }

        /// <summary>
        /// Intializes the CINI class and loads its section in an array named _sections
        /// </summary>
        /// <returns></returns>
        private CINI GetINIObject(bool ReadOnly)
        {
            CINI ini = new CINI(_fileName, _encode, ReadOnly);
            _sections = ini.GetSectionNames();
            return ini;
        }
        #endregion
    }
}

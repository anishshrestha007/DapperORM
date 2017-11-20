using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace Bango.Setting
{
    public class JsonSetting
    {
        public bool IsLoaded { get; set; }
        public string SettingFileName { get; set; }
        public string TableName { get; set; }
        public string IdColumn { get; set; }
        public string CodeColumn { get; set; }
        private Format1 _setting = null;
        public JsonSetting(string settingFileName)
            : this(settingFileName, string.Empty, string.Empty, string.Empty)
        {

        }
        public JsonSetting(string settingFileName, string tableName, string id_column)
            : this(settingFileName, tableName, id_column, "code")
        {

        }
        public JsonSetting(string settingFileName, string tableName, string id_column, string code_column)
        {
            SettingFileName = settingFileName;
            TableName = tableName;
            IdColumn = id_column;
            CodeColumn = code_column;
            IsLoaded = false;
        }
        public Format1 Setting
        {
            get
            {
                return _setting;
            }
        }
        private string FilePath
        {
            get
            {
                return FileBox.SystemParamFilePath + SettingFileName;
            }
        }

        private bool isSettingLoadedFromFile = false;

        private bool LoadSettingFromFile()
        {
            if (isSettingLoadedFromFile)
                return true;
            if (File.Exists(FilePath))
            {
                try
                {
                    _setting = JsonConvert.DeserializeObject<Format1>(File.ReadAllText(FilePath));
                    isSettingLoadedFromFile = true;
                }
                catch (Exception ex)
                {
                    //
                }
                return true;

            }

            else
                return false;
        }
        public bool Load()
        {
            if (IsLoaded)
                return true;
            return ReLoad();
        }

        public bool ReLoad()
        {
            if (LoadSettingFromFile())
            {
                return LoadIdsFromDb();
            }
            return false;
        }
        private bool LoadIdsFromDb()
        {
            if (TableName.Length == 0)
                return true;
            if (_setting.Codes == null)
                return false;
            if (_setting.Codes.Count > 0)
            {
                //creating sql to load the IDs
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"
                SELECT {0} as id, TRIM({1}) as code
                    -- LOWER({1}) as code
                FROM {2}
                WHERE NVL(is_deleted, 0) = 0", IdColumn, CodeColumn, TableName);
                String[] st = _setting.Codes.Values.ToArray();
                for (int i = 0; i < st.Length; i++)
                {
                    st[i] = "'" + st[i].Trim() + "'";
                    //st[i] = "'" + st[i].ToLower().Trim() + "'";
                }
                sb.AppendFormat(" AND TRIM({0}) in ({1})", CodeColumn, String.Join(",", st));
                //sb.AppendFormat(" AND LOWER({0}) in ({1})", CodeColumn, String.Join(",", st));
                //loading the Ids
                //DataTable dt = Bango.DB.GetConnection().ExecuteDataTable(sb.ToString());
                //TODO 
                DataTable dt = new DataTable();
                if (dt != null & dt.Rows.Count > 0)
                {
                    _setting.Ids.Clear();
                    int len = dt.Rows.Count;
                    for (int i = 0; i < len; i++)
                    {
                        if (_setting.Ids.ContainsKey(Conversion.ToString(dt.Rows[i], "code")))
                            _setting.Ids[Conversion.ToString(dt.Rows[i], "code")] = Conversion.ToInt32(dt.Rows[i], "id");
                        else
                            _setting.Ids.Add(Conversion.ToString(dt.Rows[i], "code"), Conversion.ToInt32(dt.Rows[i], "id"));
                    }
                }
                IsLoaded = true;
                return true;
            }
            return false;

        }

        public Dictionary<string, int> GetIdArray()
        {

            Dictionary<string, int> lst = new Dictionary<string, int>();
            if (Load())
            {
                foreach (KeyValuePair<string, string> itm in _setting.Codes)
                {
                    lst.Add(itm.Key, GetId(itm.Key));
                }
            }
            return lst;
        }

        public string GetCode(string name)
        {
            //load the setting if not loaded already
            if (!Load())
            {
                return string.Empty;
            }

            if (_setting != null)
            {
                if (_setting.Codes.ContainsKey(name))
                {
                    return _setting.Codes[name];
                }
            }
            return String.Empty;
        }
        public int GetId(string name)
        {
            //load the setting if not loaded already
            if (!Load())
            {
                return -1;
            }
            if (_setting != null)
            {
                if (_setting.Codes.ContainsKey(name))
                {
                    string code = GetCode(name);
                    if (_setting.Ids.ContainsKey(code))
                    {
                        return _setting.Ids[code];
                    }
                }
            }
            return -1;
        }
        public int GetIdFromCode(string code)
        {
            if (!Load())
            {
                return -1;
            }
            if (_setting != null)
            {
                if (_setting.Ids.ContainsKey(code))
                {
                    return _setting.Ids[code];
                }
            }
            return -1;
        }
        public string GetCodeFromId(int id)
        {
            if (!Load())
            {
                return string.Empty;
            }
            if (_setting != null)
            {
                if (_setting.Ids.ContainsValue(id))
                {
                    foreach (KeyValuePair<string, int> item in _setting.Ids)
                    {
                        if (id == item.Value)
                        {
                            return item.Key;
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string GetSetting(string category, string name)
        {
            LoadSettingFromFile();
            if (_setting != null)
            {
                if (_setting.Settings != null && _setting.Settings.Count > 0)
                {
                    if (_setting.Settings.ContainsKey(category))
                    {
                        if (_setting.Settings[category].ContainsKey(name))
                        {
                            return _setting.Settings[category][name];
                        }
                    }
                }
                //if(_setting)
            }
            return string.Empty;
        }
        //public string GetName(string name)
        //{
        //    //load the setting if not loaded already
        //    if (!Load())
        //    {
        //        return -1;
        //    }
        //    if (_setting != null)
        //    {
        //        if (_setting.Codes.ContainsKey(name))
        //        {
        //            string code = GetCode(name);
        //            if (_setting.Ids.ContainsKey(code))
        //            {
        //                return _setting.Ids[code];
        //            }
        //        }
        //    }
        //    return -1;
        //}
    }

    public class Format1
    {
        public Dictionary<string, Dictionary<string, string>> Settings = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, string> Codes = new Dictionary<string, string>();
        public Dictionary<string, int> Ids = new Dictionary<string, int>();
    }
}

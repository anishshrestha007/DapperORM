using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango.Base.List;
using Bango;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Bango.Helpers
{
    public class LangLoader
    {
        public string DefaultFile { get; set; }
        public DynamicDictionary Lang { get; set; } = new DynamicDictionary();
        public LangLoader()
        {
            var fb = new FileBox();
            DefaultFile = fb.WebAppRoot + @"lang\";
        }

        public void Load()
        {
            DirSearch(DefaultFile);
        }

        public void DirSearch(string sDir)
        {
            try
            {
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    foreach (var f1 in Directory.GetFiles(d).Select(f => new FileInfo(f)))
                    {
                        jsonFilesAddDictinonary(f1.Directory);
                    }
                    DirSearch(d);
                }
            }
            catch (Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }
        }

        void jsonFilesAddDictinonary(DirectoryInfo dir)
        {
            var files = dir.GetFiles("*.json");

            for (int i = 0, len = files.Length; i < len; i++)
            {
                string line;
                using (StreamReader reader = new StreamReader(files[i].FullName))
                {
                    //line = "[" + reader.ReadToEnd() + "]";
                    line = reader.ReadToEnd();
                    string json_Data = line;

                    if (json_Data.Length > 2)
                    {
                        try {
                            
                            //System.Data.DataTable Setting_data = (System.Data.DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(json_Data);
                            JToken token = JObject.Parse(json_Data);
                            Lang.Add(files[i].Name.Substring(0, files[i].Name.Length - 5).ToLower(), set_json_Data(token));
                        }
                        catch ( Exception ex )
                        {
                        }
                    }
                }
            }

            var dirs = dir.GetDirectories();
            for (int i = 0, len = dirs.Length; i < len; i++)
            {
                jsonFilesAddDictinonary(dirs[i]);
            }
        }

        DynamicDictionary set_json_Data(JToken data )
        {
            DynamicDictionary item = new DynamicDictionary();
            
            foreach(var r in data)
            {
                Newtonsoft.Json.Linq.JProperty pp = (Newtonsoft.Json.Linq.JProperty)r;

                if (pp.Count > 1)
                    set_json_Data((JToken)pp);
                else
                    item.SetValue(pp.Name, pp.Value);
            }

            return item;
        }

        public LangLoader(string defaultFile)
        {
            DefaultFile = defaultFile;
        }

        public string GetLang(string file, string key)
        {
            string val = "";
            DynamicDictionary temp = null;
            try
            {
                temp = (DynamicDictionary)Lang.GetValue(file);
                if (temp != null)
                {
                    if (temp.ContainsKey(key.ToLower()))
                        val = temp.GetValue(key.ToLower()).ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return val;
        }
        
    }
}
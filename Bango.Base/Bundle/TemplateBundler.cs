using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;


namespace Bango.Base.Bundle
{
    public class TemplateBundler
    {
        public string Path { get; set; }
        public string BundledFile { get; set; }
        public TemplateBundler()
        {
            //FileBox fb = new FileBox();
            Path = @"app\tpl\";
            FinalizePath();
        }

        public void FinalizePath()
        {
            if (BundledFile.Trim().Length == 0)
                BundledFile = Path + "template_bundled.xml";
        }

        public string Bundle()
        {
            if (File.Exists(BundledFile))
            {
                File.Delete(BundledFile);
            }

            Regex REGEX_FOR_BREAKS = new Regex(@">[\s]*<", RegexOptions.Compiled);
            Regex REGEX_FOR_TAGS = new Regex(@">s+<", RegexOptions.Compiled);
            using (XmlWriter writer = XmlWriter.Create(BundledFile))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("templates");

                string[] files = Directory.GetFiles(Path, "*.html", SearchOption.AllDirectories);
                int len = files.Length;
                string name = string.Empty;
                int path_len = Path.Length;
                for (int i = 0; i < len; i++)
                {
                    writer.WriteStartElement("template");
                    writer.WriteAttributeString("name", files[i].Replace(Path, string.Empty).Replace(".html", string.Empty).Replace("\\", "/"));
                    writer.WriteCData(REGEX_FOR_BREAKS.Replace(REGEX_FOR_TAGS.Replace(File.ReadAllText(files[i]), "> <"), "><"));
                    //writer.WriteCData(Regex.Replace(File.ReadAllText(files[i]), @"\s*(?<capture><(?<markUp>\w+)>.*<\/\k<markUp>>)\s*", "${capture}", RegexOptions.Singleline));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            return string.Empty;
        }
    }
}

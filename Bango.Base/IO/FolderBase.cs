using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base.IO
{
    public class FolderBase : IFolderBase
    {
        public string ParentPath { get; set; }
        public string FolderName { get; set; }

        private string _path = string.Empty;

        public string FileNameInitial = "file_name";
        public string FileNameUniqueFormat = "yyyMMdd_hhmmss_fffff";
        public bool UseRandom = true;
        private Random r = new Random();
        public string Path
        {
            get
            {
                return _path;
            }
        }
        public FolderBase(string parentPath, string folderName)
        {
            ParentPath= parentPath;
            FolderName = folderName;

            initFolder();
        }

        public void initFolder()
        {
            if (!ParentPath.EndsWith("\\"))
                ParentPath += "\\";
            if (!Directory.Exists(ParentPath))
                Directory.CreateDirectory(ParentPath);
            _path = ParentPath + FolderName + "\\";
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public int GetRandom()
        {
            return r.Next(1000, 9999);
        }

        public string GetNewFileName(string extension)
        {
            return _path + FileNameInitial + "_" + DateTime.Now.ToString(FileNameUniqueFormat) + "_" + GetRandom().ToString() + "." + extension;
        }

        public string GetRelativeFilePath(string fileName)
        {
            return _path + fileName;
        }

    }
}

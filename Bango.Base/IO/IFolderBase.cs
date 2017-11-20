using System;
namespace Bango.Base.IO
{
    interface IFolderBase
    {
        string FolderName { get; set; }
        string GetNewFileName(string extension);
        int GetRandom();
        string GetRelativeFilePath(string fileName);
        void initFolder();
        string ParentPath { get; set; }
        string Path { get; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace AwiUtils
{
    /// <summary> An adpter which adapts an FileSystemInfo to an IDirEntry. </summary>
    public class FileSystemInfoDirEntry : IDirEntry
    {
        public FileSystemInfoDirEntry(FileSystemInfo f) { this.entry = f; }

        #region IDirEntry Member
        public string Path { get { return entry.FullName; } }
        public string Name { get { return entry.Name; } }
        public bool IsDir { get { return entry is DirectoryInfo; } }
        public DateTime LastModified { get { return entry.LastWriteTime; } }
        public long Size
        {
            get
            {
                FileInfo fi = entry as FileInfo;
                return fi != null ? fi.Length : -1;
            }
        }
        #endregion


        public FileSystemInfo FileSystemInfo { get { return entry; } }

        public FileSystemInfo[] GetChildren()
        {
            DirectoryInfo di = entry as DirectoryInfo;
            if (di != null)
                return di.GetFileSystemInfos();
            else
                return new FileSystemInfo[] { };
        }

        public override string ToString()
        {
            return Path + " " + (IsDir ? "DIR" : Size.ToString());
        }

        FileSystemInfo entry;
    }

    /// <summary> A simple class that lists file system contents while respecting some filters. </summary>
    public static class LocalFileSystemHelper
    {
        public static List<IDirEntry> GetFileList(string folder)
        {
            FileSystemInfoDirEntry de = new FileSystemInfoDirEntry(new DirectoryInfo(folder));
            FileSystemInfo[] files = de.GetChildren();
            List<IDirEntry> result = new List<IDirEntry>();
            foreach (var f in files)
                result.Add(new FileSystemInfoDirEntry(f));

            return result;
        }

        public static List<IDirEntry> GetFileListRecursive(string folder, IDirEntryFilter filter)
        {
            List<IDirEntry> allEntries = new List<IDirEntry>();
            List<IDirEntry> entries = GetFileList(folder);
            entries.RemoveAll(f => !filter.IsPassing(f));
            allEntries.AddRange(entries);

            foreach (var entry in entries)
                if (entry.IsDir && filter.IsPassing(entry))
                    allEntries.AddRange(GetFileListRecursive(entry.Path, filter));
            return allEntries;
        }
    }
}
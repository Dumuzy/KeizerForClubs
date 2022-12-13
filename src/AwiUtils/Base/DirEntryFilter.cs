using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AwiUtils
{
    /// <summary> A common interface for FileInfo and FTPFile, used also in IDirEntryFilter. </summary>
    public interface IDirEntry
    {
        string Path { get; }
        string Name { get; }
        bool IsDir { get; }
        DateTime LastModified { get; }
        long Size { get; }
    }

    /// <summary> Null-Element of type IDirEntry. </summary>
    public class NullDirEntry : IDirEntry
    {
        public static NullDirEntry Null { get { return item; } }
        #region IDirEntry Member
        public string Path { get { return ""; } }
        public string Name { get { return ""; } }
        public bool IsDir { get { return false; } }
        public DateTime LastModified { get { return DateTime.FromBinary(0); } }
        public long Size { get { return 0; } }
        #endregion

        static NullDirEntry item = new NullDirEntry();
    }

    /// <summary> Ein Filter für IDirEntry directory-entries.</summary>
    public interface IDirEntryFilter
    {
        /// <summary> True, if entry is passing throgh the filter, false if not. </summary>
        bool IsPassing(IDirEntry f);
    }

    public class BaseDirEntryFilter : IDirEntryFilter
    {
        public BaseDirEntryFilter(string dirNames, bool shallOnlyDirectoriesBeFiltered,
            bool shallOnlyFilesBeFiltered, bool shallIgnoreCase) :
            this(dirNames.Split(" ".ToCharArray()), shallOnlyDirectoriesBeFiltered, shallOnlyFilesBeFiltered, shallIgnoreCase)
        { }

        public BaseDirEntryFilter(IEnumerable<string> dirNames, bool shallOnlyDirectoriesBeFiltered,
            bool shallOnlyFilesBeFiltered, bool shallIgnoreCase)
        {
            this.shallOnlyDirectoriesBeFiltered = shallOnlyDirectoriesBeFiltered;
            this.shallOnlyFilesBeFiltered = shallOnlyFilesBeFiltered;
            this.shallIgnoreCase = shallIgnoreCase;
            this.dirNames = new List<string>();
            foreach (var s in dirNames)
            {
                if (s != "")
                {
                    string z = s.Trim(@"/\".ToCharArray());
                    this.dirNames.Add(z);
                }
            }
        }

        public bool IsPassing(IDirEntry f)
        {
            if (shallOnlyDirectoriesBeFiltered && !f.IsDir)
                return true;

            if (shallOnlyFilesBeFiltered && f.IsDir)
                return true;

            bool isPassing = true;
            foreach (var s in dirNames)
                if ((shallIgnoreCase && f.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    || (!shallIgnoreCase && f.Name.Equals(s)))
                {
                    isPassing = false;
                    break;
                }
            return isPassing;
        }

        public override string ToString()
        {
            string s = shallOnlyDirectoriesBeFiltered ? "Directories " : (shallOnlyFilesBeFiltered ? "Files " : "");
            s += dirNames != null ? string.Join(", ", dirNames.ToArray()) : "(null)";
            return s;
        }

        List<string> dirNames;
        bool shallOnlyDirectoriesBeFiltered, shallOnlyFilesBeFiltered, shallIgnoreCase;
    }


    /// <summary> Directory entries which are == one of the dirNames are not passing. </summary>
    public class DirectoryDirEntryFilter : BaseDirEntryFilter
    {
        public DirectoryDirEntryFilter(string dirNames) : base(dirNames, true, false, true) { }
    }


    /// <summary> File entries which are == one of the names are not passing. </summary>
    public class FileDirEntryFilter : BaseDirEntryFilter
    {
        public FileDirEntryFilter(string names) : base(names, false, true, true) { }
    }

    public class RegexDirEntryFilter : IDirEntryFilter
    {
        public RegexDirEntryFilter(Regex filter, bool shallOnlyDirectoriesBeFiltered, bool shallOnlyFilesBeFiltered)
        {
            this.shallOnlyDirectoriesBeFiltered = shallOnlyDirectoriesBeFiltered;
            this.shallOnlyFilesBeFiltered = shallOnlyFilesBeFiltered;
            this.filter = filter;
        }

        public bool IsPassing(IDirEntry f)
        {
            if (shallOnlyDirectoriesBeFiltered && !f.IsDir)
                return true;

            if (shallOnlyFilesBeFiltered && f.IsDir)
                return true;

            bool isPassing = !filter.IsMatch(f.Path);
            return isPassing;
        }

        public override string ToString()
        {
            string s = shallOnlyDirectoriesBeFiltered ? "RegexDirectories " : (shallOnlyFilesBeFiltered ? "RegexFiles " : "");
            s += filter.ToString();
            return s;
        }

        bool shallOnlyDirectoriesBeFiltered, shallOnlyFilesBeFiltered;
        Regex filter;
    }

    public class FileRegexDirEntryFilter : RegexDirEntryFilter
    {
        public FileRegexDirEntryFilter(string pattern) : base(new Regex(pattern, RegexOptions.IgnoreCase), false, true) { }
    }

    public class FileTimeNewerThanDirEntryFilter : IDirEntryFilter
    {
        public FileTimeNewerThanDirEntryFilter(DateTime newerThan)
        {
            this.newerThan = newerThan;
        }

        public bool IsPassing(IDirEntry f)
        {
            if (f.IsDir)
                return true;

            bool isPassing = f.LastModified > newerThan;
            return isPassing;
        }

        public override string ToString()
        {
            string s = "newerThan " + newerThan.ToString("yyyy-MM-dd");
            return s;
        }

        DateTime newerThan;
    }

    /// <summary> A DirEntryFilter as a combination of other filters. Only what passes all inner filters, passes also the combined one. </summary>
    public class CombinedDirEntryFilter : IDirEntryFilter
    {
        public CombinedDirEntryFilter(IDirEntryFilter f1, IDirEntryFilter f2) : this(f1, f2, null, null, null) { }
        public CombinedDirEntryFilter(IDirEntryFilter f1, IDirEntryFilter f2, IDirEntryFilter f3)
            : this(f1, f2, f3, null, null) { }
        public CombinedDirEntryFilter(IDirEntryFilter f1, IDirEntryFilter f2, IDirEntryFilter f3, IDirEntryFilter f4)
            : this(f1, f2, f3, f4, null) { }
        public CombinedDirEntryFilter(IDirEntryFilter f1, IDirEntryFilter f2, IDirEntryFilter f3
            , IDirEntryFilter f4, IDirEntryFilter f5)
        {
            filters = new List<IDirEntryFilter>();
            filters.Add(f1);
            filters.Add(f2);
            filters.Add(f3);
            filters.Add(f4);
            filters.Add(f5);
            filters.RemoveAll(f => f == null);
        }

        public bool IsPassing(IDirEntry f)
        {
            bool isPassing = true;
            foreach (var filter in filters)
                if (!filter.IsPassing(f))
                {
                    isPassing = false;
                    break;
                }
            return isPassing;
        }

        List<IDirEntryFilter> filters;
    }
}
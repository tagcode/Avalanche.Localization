// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ILocalizationFileSystem"/>.</summary>
public static class LocalizationFileSystemPrintTreeExtensions
{
    /// <summary>
    /// Vists localizationFileSystem as tree structure
    /// 
    /// ├──""
    /// │  ├──"path"
    /// │  │  └──"path"
    /// </summary>
    /// <param name="localizationFileSystem"></param>
    /// <param name="depth">Maximum visit depth</param>
    /// <returns></returns>
    public static IEnumerable<Line> VisitTree(this ILocalizationFileSystem localizationFileSystem, string path = "", int depth = int.MaxValue)
    {
        // Init queue
        List<Line> queue = new List<Line>();
        // Add to queue
        queue.Add(new Line(localizationFileSystem, path, false, 0, 0UL));
        // Process queue
        while (queue.Count > 0)
        {
            // Next line
            int lastIx = queue.Count - 1;
            Line line = queue[lastIx];
            queue.RemoveAt(lastIx);


            // Place directories and files here
            string[]? files = null, directories = null!;
            try
            {
                // Read directory
                line.LocalizationFileSystem.TryListFiles(line.Path, out files);
                line.LocalizationFileSystem.TryListDirectories(line.Path, out directories);
            }
            catch (Exception e)
            {
                // Add error to be yielded along
                line.Error = e;
            }
            // Total entry count
            int count = (files == null ? 0 : files.Length) + (directories == null ? 0 : directories.Length);
            //
            if (count > 0)
            {
                // 
                int startIndex = queue.Count;
                // Add Files
                if (files != null && files.Length > 0 && line.Level < depth)
                {
                    // Bitmask when this level continues
                    ulong levelContinuesBitMask = line.LevelContinuesBitMask | (line.Level < 64 ? 1UL << line.Level : 0UL);
                    // Add files to queue
                    for (int i = files.Length - 1; i >= 0; i--)
                    {
                        // Create line
                        Line fileLine = new Line(line.LocalizationFileSystem, files[i], true, line.Level + 1, levelContinuesBitMask);
                        // yield line
                        queue.Add(fileLine);
                    }
                }

                // Got children
                if (directories != null && directories.Length > 0 && line.Level < depth)
                {
                    // Bitmask when this level continues
                    ulong levelContinuesBitMask = line.LevelContinuesBitMask | (line.Level < 64 ? 1UL << line.Level : 0UL);
                    // Add children in reverse order
                    for (int i = directories.Length - 1; i >= 0; i--)
                    {
                        // Create line
                        Line childLine = new Line(line.LocalizationFileSystem, directories[i], false, line.Level + 1, levelContinuesBitMask);
                        // Ad to queue
                        queue.Add(childLine);
                    }
                }
                // Last entry doesn't continue on its level.
                if (count >= 1) queue[startIndex] = queue[startIndex].NewLevelContinuesBitMask(line.LevelContinuesBitMask);
            }

            // Yield 
            yield return line;
        }
    }

    /// <summary>Print as tree structure.</summary>
    public static void PrintTreeTo(this ILocalizationFileSystem localizationFileSystem, TextWriter output, string path = "", int depth = int.MaxValue, PrintFormat format = PrintFormat.Default)
    {
        StringBuilder sb = new StringBuilder();
        List<int> columns = new List<int>();
        foreach (Line line in localizationFileSystem.VisitTree(path, depth))
        {
            line.AppendTo(sb, format, columns);
            output.WriteLine(sb);
            sb.Clear();
        }
        output.Write(sb);
    }

    /// <summary>Print as tree structure.</summary>
    public static void PrintTreeTo(this ILocalizationFileSystem localizationFileSystem, StringBuilder output, string path = "", int depth = int.MaxValue, PrintFormat format = PrintFormat.Default)
    {
        List<int> columns = new List<int>();
        foreach (Line line in localizationFileSystem.VisitTree(path, depth))
        {
            line.AppendTo(output, format, columns);
            output.AppendLine();
        }
    }

    /// <summary>Print as tree structure.</summary>
    /// <returns>Tree as string</returns>
    public static String PrintTree(this ILocalizationFileSystem localizationFileSystem, string path = "", int depth = int.MaxValue, PrintFormat format = PrintFormat.Default)
    {
        StringBuilder sb = new StringBuilder();
        List<int> columns = new List<int>();
        foreach (Line line in localizationFileSystem.VisitTree(path, depth))
        {
            line.AppendTo(sb, format, columns);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>Line.</summary>
    public struct Line
    {
        /// <summary></summary>
        public ILocalizationFileSystem LocalizationFileSystem = null!;
        /// <summary>Path</summary>
        public string Path = null!;
        /// <summary></summary>
        public bool IsFile;
        /// <summary>Depth level, starts at 0.</summary>
        public int Level;
        /// <summary>Bitmask for each level on whether the level has more lines.</summary>
        public ulong LevelContinuesBitMask;
        /// <summary>(optional) error is placed here.</summary>
        public Exception? Error;

        /// <summary>Create request line</summary>
        public Line(
            ILocalizationFileSystem localizationFileSystem,
            string path,
            bool isFile,
            int level,
            ulong levelContinuesBitMask,
            Exception? error = null)
        {
            LocalizationFileSystem = localizationFileSystem;
            Path = path;
            IsFile = isFile;
            Level = level;
            LevelContinuesBitMask = levelContinuesBitMask;
            Error = error;
        }

        /// <summary>Create line with new value to <see cref="LevelContinuesBitMask"/>.</summary>
        /// <param name="newLevelContinuesBitMask"></param>
        /// <returns>line with new mask</returns>
        public Line NewLevelContinuesBitMask(ulong newLevelContinuesBitMask) => new Line(LocalizationFileSystem, Path, IsFile, Level, newLevelContinuesBitMask, Error);

        /// <summary>Tests whether there will be more sections to specific <paramref name="level"/>.</summary>
        public bool LevelContinues(int level)
        {
            // Undefined
            if (level == 0) return false;
            // Not supported after 64 levels
            if (level > 64) return false;
            // Read the bit
            return (LevelContinuesBitMask & 1UL << (level - 1)) != 0UL;
        }

        /// <summary>Write to <see cref="StringBuilder"/> <paramref name="output"/>.</summary>
        public void AppendTo(StringBuilder output, PrintFormat format, IList<int> columns)
        {
            // Number of info fields printed
            int column = 0;

            // Print tree
            if (format.HasFlag(PrintFormat.Tree) && Level > 0)
            {
                // Print indents
                for (int l = 1; l < Level; l++)
                {
                    // Append " "
                    if (column++ > 0) output.Append(" ");
                    //
                    output.Append(LevelContinues(l) ? "│  " : "   ");
                }
                // Print last indent
                if (Level >= 1)
                {
                    // Append " "
                    if (column++ > 0) output.Append(" ");
                    //
                    output.Append(LevelContinues(Level) ? "├──" : "└──");
                }
            }
            // Print Name
            if ((format & PrintFormat.Name) != 0)
            {
                if (column++ > 0) output.Append(" ");
                // Print Name
                string name = System.IO.Path.GetFileName(Path);
                // Print 
                output.Append(name);
                // '/'
                if ((format & PrintFormat.DirectorySlash) != 0 && !IsFile) output.Append('/');
            }
            // Print Path
            if ((format & PrintFormat.Path) != 0)
            {
                if (column++ > 0) output.Append(" ");
                // Print 
                output.Append(Path);
                // '/'
                if ((format & PrintFormat.DirectorySlash) != 0 && !IsFile) output.Append('/');
            }
            // Print Error
            if ((format & PrintFormat.Error) != 0 && this.Error != null)
            {
                if (column++ > 0) output.Append(" ");
                // Print 
                output.Append(Error);
            }
        }

        /// <summary>Print as string</summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(PrintFormat format)
        {
            StringBuilder sb = new StringBuilder();
            List<int> columns = new List<int>();
            AppendTo(sb, format, columns);
            return sb.ToString();
        }

        /// <summary>Print as string</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<int> columns = new List<int>();
            AppendTo(sb, PrintFormat.Default, columns);
            return sb.ToString();
        }
    }

    /// <summary>Print format flags</summary>
    [Flags]
    public enum PrintFormat : ulong
    {
        /// <summary>Print tree structure</summary>
        Tree = 1UL << 1,
        /// <summary>Name</summary>
        Name = 1UL << 3,
        /// <summary>Path</summary>
        Path = 1UL << 4,
        /// <summary>Print '/' at end of directory</summary>
        DirectorySlash = 1UL << 5,
        /// <summary>Path</summary>
        Error = 1UL << 8,
        /// <summary>NewLine</summary>
        NewLine = 1UL << 9,
        /// <summary>Default format</summary>
        DefaultLong = Tree | Path | Error,
        /// <summary>Default format</summary>
        Default = Tree | Name | Error,
    }

}


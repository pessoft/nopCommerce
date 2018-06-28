using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// A file provider abstraction
    /// </summary>
    public partial interface INopFileProvider : IFileProvider
    {
        /// <summary>
        /// Combines an array of strings into a path
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <returns>The combined paths</returns>
        string Combine(params string[] paths);

        /// <summary>
        /// Combines an array of strings into a path
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains combined paths</returns>
        Task<string> CombineAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist
        /// </summary>
        /// <param name="path">The directory to create</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Creates all directories and subdirectories in the specified path unless they already exist
        /// </summary>
        /// <param name="path">The directory to create</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that directory is created</returns>
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or overwrites a file in the specified path
        /// </summary>
        /// <param name="path">The path and name of the file to create</param>
        void CreateFile(string path);

        /// <summary>
        /// Creates or overwrites a file in the specified path
        /// </summary>
        /// <param name="path">The path and name of the file to create</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that file is created</returns>
        Task CreateFileAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
        /// </summary>
        /// <param name="path">Directory path</param>
        void DeleteDirectory(string path);

        /// <summary>
        ///  Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that directory is deleted</returns>
        Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="filePath">The name of the file to be deleted. Wildcard characters are not supported</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="filePath">The name of the file to be deleted. Wildcard characters are not supported</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that file is deleted</returns>
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>
        /// true if path refers to an existing directory; false if the directory does not exist or an error occurs when
        /// trying to determine if the specified file exists
        /// </returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Determines whether the given path refers to an existing directory on disk
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether directory exists</returns>
        Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file or a directory and its contents to a new location
        /// </summary>
        /// <param name="sourceDirName">The path of the file or directory to move</param>
        /// <param name="destDirName">
        /// The path to the new location for sourceDirName. If sourceDirName is a file, then destDirName
        /// must also be a file name
        /// </param>
        void DirectoryMove(string sourceDirName, string destDirName);

        /// <summary>
        /// Moves a file or a directory and its contents to a new location
        /// </summary>
        /// <param name="sourceDirName">The path of the file or directory from</param>
        /// <param name="destDirName">The path of the file or directory to move</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that directory is moved</returns>
        Task DirectoryMoveAsync(string sourceDirName, string destDirName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns an enumerable collection of file names that match a search pattern in
        /// a specified path, and optionally searches subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to search</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. This parameter
        /// can contain a combination of valid literal path and wildcard (* and ?) characters
        /// , but doesn't support regular expressions.
        /// </param>
        /// <param name="topDirectoryOnly">
        /// Specifies whether to search the current directory, or the current directory and all
        /// subdirectories
        /// </param>
        /// <returns>
        /// An enumerable collection of the full names (including paths) for the files in
        /// the directory specified by path and that match the specified search pattern
        /// </returns>
        IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern, bool topDirectoryOnly = true);

        /// <summary>
        /// Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to search</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. This parameter
        /// can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions./// </param>
        /// <param name="topDirectoryOnly">Specifies whether to search the current directory, or the current directory and all subdirectories</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains enumerated files</returns>
        Task<IEnumerable<string>> EnumerateFilesAsync(string directoryPath, string searchPattern,
            bool topDirectoryOnly = true, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed
        /// </summary>
        /// <param name="sourceFileName">The file to copy</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false</param>
        void FileCopy(string sourceFileName, string destFileName, bool overwrite = false);

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is allowed
        /// </summary>
        /// <param name="sourceFileName">The file to copy</param>
        /// <param name="destFileName">The name of the destination file. This cannot be a directory</param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that file is copied</returns>
        Task FileCopyAsync(string sourceFileName, string destFileName, bool overwrite = false,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Determines whether the specified file exists
        /// </summary>
        /// <param name="filePath">The file to check</param>
        /// <returns>
        /// True if the caller has the required permissions and path contains the name of an existing file; otherwise,
        /// false.
        /// </returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Determines whether the specified file exists
        /// </summary>
        /// <param name="filePath">The file to check</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether file exists</returns>
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the length of the file in bytes, or -1 for a directory or non-existing files
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The length of the file</returns>
        long FileLength(string path);

        /// <summary>
        /// Gets the length of the file in bytes, or -1 for a directory or non-existing files
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The length of the file</returns>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the length of the file</returns>
        Task<long> FileLengthAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name
        /// </summary>
        /// <param name="sourceFileName">The name of the file to move. Can include a relative or absolute path</param>
        /// <param name="destFileName">The new path and name for the file</param>
        void FileMove(string sourceFileName, string destFileName);

        /// <summary>
        /// Moves a specified file to a new location, providing the option to specify a new file name
        /// </summary>
        /// <param name="sourceFileName">The name of the file to move. Can include a relative or absolute path</param>
        /// <param name="destFileName">The new path and name for the file</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that file is moved</returns>
        Task FileMoveAsync(string sourceFileName, string destFileName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the absolute path to the directory
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <returns>The absolute path to the directory</returns>
        string GetAbsolutePath(params string[] paths);

        /// <summary>
        /// Returns the absolute path to the directory
        /// </summary>
        /// <param name="paths">An array of parts of the path</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the absolute path to the directory</returns>
        Task<string> GetAbsolutePathAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a System.Security.AccessControl.DirectorySecurity object that encapsulates the access control list (ACL) entries for a specified directory
        /// </summary>
        /// <param name="path">The path to a directory containing a System.Security.AccessControl.DirectorySecurity object that describes the file's access control list (ACL) information</param>
        /// <returns>An object that encapsulates the access control rules for the file described by the path parameter</returns>
        DirectorySecurity GetAccessControl(string path);

        /// <summary>
        /// Gets an object that encapsulates the access control list (ACL) entries for a specified directory
        /// </summary>
        /// <param name="path">The path to a directory containing a System.Security.AccessControl.DirectorySecurity object that describes the file's access control list (ACL) information</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the DirectorySecurity object</returns>
        Task<DirectorySecurity> GetAccessControlAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the creation date and time of the specified file or directory
        /// </summary>
        /// <param name="path">The file or directory for which to obtain creation date and time information</param>
        /// <returns>
        /// A System.DateTime structure set to the creation date and time for the specified file or directory. This value
        /// is expressed in local time
        /// </returns>
        DateTime GetCreationTime(string path);

        /// <summary>
        /// Returns the creation date and time of the specified file or directory
        /// </summary>
        /// <param name="path">The file or directory for which to obtain creation date and time information</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the creation date and time</returns>
        Task<DateTime> GetCreationTimeAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the names of the subdirectories (including their paths) that match the
        /// specified search pattern in the specified directory
        /// </summary>
        /// <param name="path">The path to the directory to search</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of subdirectories in path. This
        /// parameter can contain a combination of valid literal and wildcard characters
        /// , but doesn't support regular expressions.
        /// </param>
        /// <param name="topDirectoryOnly">
        /// Specifies whether to search the current directory, or the current directory and all
        /// subdirectories
        /// </param>
        /// <returns>
        /// An array of the full names (including paths) of the subdirectories that match
        /// the specified criteria, or an empty array if no directories are found
        /// </returns>
        string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true);

        /// <summary>
        /// Returns the names of the subdirectories (including their paths) that match the specified search pattern in the specified directory
        /// </summary>
        /// <param name="path">The path to the directory to search</param>
        /// <param name="searchPattern">The search string to match against the names of subdirectories in path. 
        /// This parameter can contain a combination of valid literal and wildcard characters, but doesn't support regular expressions.</param>
        /// <param name="topDirectoryOnly">Specifies whether to search the current directory, or the current directory and all subdirectories</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains collection of directory names</returns>
        Task<string[]> GetDirectoriesAsync(string path, string searchPattern = "", bool topDirectoryOnly = true,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the directory information for the specified path string
        /// </summary>
        /// <param name="path">The path of a file or directory</param>
        /// <returns>
        /// Directory information for path, or null if path denotes a root directory or is null. Returns
        /// System.String.Empty if path does not contain directory information
        /// </returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Returns the directory information for the specified path string
        /// </summary>
        /// <param name="path">The path of a file or directory</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the directory name</returns>
        Task<string> GetDirectoryNameAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the directory name only for the specified path string
        /// </summary>
        /// <param name="path">The path of directory</param>
        /// <returns>The directory name</returns>
        string GetDirectoryNameOnly(string path);

        /// <summary>
        /// Returns the directory name only for the specified path string
        /// </summary>
        /// <param name="path">The path of directory</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the directory name</returns>
        Task<string> GetDirectoryNameOnlyAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the extension of the specified path string
        /// </summary>
        /// <param name="filePath">The path string from which to get the extension</param>
        /// <returns>The extension of the specified path (including the period ".")</returns>
        string GetFileExtension(string filePath);

        /// <summary>
        /// Returns the extension of the specified path string
        /// </summary>
        /// <param name="filePath">The path string from which to get the extension</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the extension of the file</returns>
        Task<string> GetFileExtensionAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the file name and extension of the specified path string
        /// </summary>
        /// <param name="path">The path string from which to obtain the file name and extension</param>
        /// <returns>The characters after the last directory character in path</returns>
        string GetFileName(string path);

        /// <summary>
        /// Returns the file name and extension of the specified path string
        /// </summary>
        /// <param name="path">The path string from which to obtain the file name and extension</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the file name and extension</returns>
        Task<string> GetFileNameAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the file name of the specified path string without the extension
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        /// <returns>The file name, minus the last period (.) and all characters following it</returns>
        string GetFileNameWithoutExtension(string filePath);

        /// <summary>
        /// Returns the file name of the specified path string without the extension
        /// </summary>
        /// <param name="filePath">The path of the file</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the file name without the extension</returns>
        Task<string> GetFileNameWithoutExtensionAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search
        /// pattern in the specified directory, using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to search</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. This parameter
        /// can contain a combination of valid literal path and wildcard (* and ?) characters
        /// , but doesn't support regular expressions.
        /// </param>
        /// <param name="topDirectoryOnly">
        /// Specifies whether to search the current directory, or the current directory and all
        /// subdirectories
        /// </param>
        /// <returns>
        /// An array of the full names (including paths) for the files in the specified directory
        /// that match the specified search pattern, or an empty array if no files are found.
        /// </returns>
        string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true);

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory, using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to search</param>
        /// <param name="searchPattern">The search string to match against the names of files in path. 
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
        /// <param name="topDirectoryOnly">Specifies whether to search the current directory, or the current directory and all subdirectories</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the names of files</returns>
        Task<string[]> GetFilesAsync(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed
        /// </summary>
        /// <param name="path">The file or directory for which to obtain access date and time information</param>
        /// <returns>A System.DateTime structure set to the date and time that the specified file</returns>
        DateTime GetLastAccessTime(string path);

        /// <summary>
        /// Returns the date and time the specified file or directory was last accessed
        /// </summary>
        /// <param name="path">The file or directory for which to obtain access date and time information</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the date and time the specified file or directory was last accessed</returns>
        Task<DateTime> GetLastAccessTimeAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information</param>
        /// <returns>
        /// A System.DateTime structure set to the date and time that the specified file or directory was last written to.
        /// This value is expressed in local time
        /// </returns>
        DateTime GetLastWriteTime(string path);

        /// <summary>
        /// Returns the date and time the specified file or directory was last written to
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the date and time the specified file or directory was last written to</returns>
        Task<DateTime> GetLastWriteTimeAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the date and time, in coordinated universal time (UTC), that the specified file or directory was last
        /// written to
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information</param>
        /// <returns>
        /// A System.DateTime structure set to the date and time that the specified file or directory was last written to.
        /// This value is expressed in UTC time
        /// </returns>
        DateTime GetLastWriteTimeUtc(string path);

        /// <summary>
        /// Returns the date and time, in coordinated universal time (UTC), that the specified file or directory was last written to
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the date and time (UTC) the specified file or directory was last written to</returns>
        Task<DateTime> GetLastWriteTimeUtcAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the parent directory of the specified path
        /// </summary>
        /// <param name="directoryPath">The path for which to retrieve the parent directory</param>
        /// <returns>The parent directory, or null if path is the root directory, including the root of a UNC server or share name</returns>
        string GetParentDirectory(string directoryPath);

        /// <summary>
        /// Retrieves the parent directory of the specified path
        /// </summary>
        /// <param name="directoryPath">The path for which to retrieve the parent directory</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the parent directory</returns>
        Task<string> GetParentDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks if the path is directory
        /// </summary>
        /// <param name="path">Path for check</param>
        /// <returns>True, if the path is a directory, otherwise false</returns>
        bool IsDirectory(string path);

        /// <summary>
        /// Checks if the path is directory
        /// </summary>
        /// <param name="path">Path for check</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines whether passed path is a directory</returns>
        Task<bool> IsDirectoryAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the path</returns>
        Task<string> MapPathAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Reads the contents of the file into a byte array
        /// </summary>
        /// <param name="filePath">The file for reading</param>
        /// <returns>A byte array containing the contents of the file</returns>
        byte[] ReadAllBytes(string filePath);

        /// <summary>
        /// Reads the contents of the file into a byte array
        /// </summary>
        /// <param name="filePath">The file for reading</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains a byte array containing the contents of the file</returns>
        Task<byte[]> ReadAllBytesAsync(string filePath, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <param name="encoding">The encoding applied to the contents of the file</param>
        /// <returns>A string containing all lines of the file</returns>
        string ReadAllText(string path, Encoding encoding);

        /// <summary>
        /// Opens a file, reads all lines of the file with the specified encoding, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading</param>
        /// <param name="encoding">The encoding applied to the contents of the file</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result contains the read text from a file by the passed path</returns>
        Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the date and time, in coordinated universal time (UTC), that the specified file was last written to
        /// </summary>
        /// <param name="path">The file for which to set the date and time information</param>
        /// <param name="lastWriteTimeUtc">
        /// A System.DateTime containing the value to set for the last write date and time of path.
        /// This value is expressed in UTC time
        /// </param>
        void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

        /// <summary>
        /// Sets the date and time, in coordinated universal time (UTC), that the specified file was last written to
        /// </summary>
        /// <param name="path">The file for which to set the date and time information</param>
        /// <param name="lastWriteTimeUtc">A System.DateTime containing the value to set for the last write date and time of path. This value is expressed in UTC time</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that the date and time is set</returns>
        Task SetLastWriteTimeUtcAsync(string path, DateTime lastWriteTimeUtc, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Writes the specified byte array to the file
        /// </summary>
        /// <param name="filePath">The file to write to</param>
        /// <param name="bytes">The bytes to write to the file</param>
        void WriteAllBytes(string filePath, byte[] bytes);

        /// <summary>
        /// Writes the specified byte array to the file
        /// </summary>
        /// <param name="filePath">The file to write to</param>
        /// <param name="bytes">The bytes to write to the file</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that byte array is written to the file</returns>
        Task WriteAllBytesAsync(string filePath, byte[] bytes, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding,
        /// and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to</param>
        /// <param name="contents">The string to write to the file</param>
        /// <param name="encoding">The encoding to apply to the string</param>
        void WriteAllText(string path, string contents, Encoding encoding);

        /// <summary>
        /// Creates a new file, writes the specified string to the file using the specified encoding,
        /// and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="path">The file to write to</param>
        /// <param name="contents">The string to write to the file</param>
        /// <param name="encoding">The encoding to apply to the string</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete</param>
        /// <returns>The asynchronous task whose result determines that passed text is written to a file by the passed path</returns>
        Task WriteAllTextAsync(string path, string contents, Encoding encoding,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
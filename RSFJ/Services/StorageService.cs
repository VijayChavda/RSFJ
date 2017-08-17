using System;
using System.IO;

namespace RSFJ.Services
{
    /// <summary>
    /// Service to handle storage related business.
    /// </summary>
    public class StorageService
    {
        /// <summary>
        /// The only instance of StorageService.
        /// </summary>
        public static StorageService Instance => _Instance ?? (_Instance = new StorageService());
        private static StorageService _Instance;

        /// <summary>
        /// Path to the reports directory.
        /// </summary>
        public static readonly string ReportsDirectory;

        /// <summary>
        /// Path to the backup directory.
        /// </summary>
        public static readonly string BackupsDirectory;

        /// <summary>
        /// Path to the database file.
        /// </summary>
        public static readonly string DatabaseFile;

        /// <summary>
        /// Initializes StorageService.
        /// </summary>
        private StorageService()
        {
            InitializeStorage();
        }

        /// <summary>
        /// Initializes static components of StorageService.
        /// </summary>
        static StorageService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var rootDirectory = Path.Combine(appData, "RSFJ");
            ReportsDirectory = Path.Combine(rootDirectory, "Reports");
            BackupsDirectory = Path.Combine(rootDirectory, "Backups");
            DatabaseFile = Path.Combine(rootDirectory, "database.rsfj");
        }

        /// <summary>
        /// Initializes the storage structure of RSFJ.
        /// </summary>
        private void InitializeStorage()
        {
            Directory.CreateDirectory(ReportsDirectory);
            Directory.CreateDirectory(BackupsDirectory);
        }

        /// <summary>
        /// Creates a file, and writes content into it.
        /// </summary>
        /// <param name="File">The path to the file to create.</param>
        /// <param name="Content">The content to write in the file.</param>
        public void CreateFile(string File, string Content)
        {
            using (var writer = new StreamWriter(File))
            {
                writer.Write(Content);
            }
        }

        public void DeleteFile(string File)
        {
            new FileInfo(File).Delete();
        }

        /// <summary>
        /// Creates a file which has time-stamp suffixed in it's name, and writes content into it.
        /// </summary>
        /// <param name="Directory">The directory inside which to create this file.</param>
        /// <param name="FileName">The name of the file to create.</param>
        /// <param name="FileExtension">The extension of the file to create.</param>
        /// <param name="Content">The content to write in the file.</param>
        /// <returns>Returns the path to the file that was created.</returns>
        public string CreateTimeStampFile(string Directory, string FileName, string FileExtension, string Content)
        {
            var file = Path.Combine(Directory,
                string.Format("{0}-{1}.{2}", FileName, DateTime.Now.ToString("yyyyMMddHHmmss"), FileExtension));

            CreateFile(file, Content);

            return file;
        }

        public void CopyFile(string file, string location)
        {
            new FileInfo(file).CopyTo(location);
        }
    }
}

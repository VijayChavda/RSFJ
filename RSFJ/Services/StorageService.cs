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
        public static StorageService Instance => new StorageService();

        /// <summary>
        /// Path to the reports directory.
        /// </summary>
        public string ReportsDirectory { get; private set; }

        /// <summary>
        /// Path to the backup directory.
        /// </summary>
        public string BackupsDirectory { get; private set; }

        /// <summary>
        /// Path to the database file.
        /// </summary>
        public string DatabaseFile { get; private set; }

        /// <summary>
        /// Initializes StorageService.
        /// </summary>
        private StorageService()
        {
            InitializeStorage();
        }

        /// <summary>
        /// Initializes the storage structure of RSFJ.
        /// </summary>
        private void InitializeStorage()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var rootDirectory = Path.Combine(appData, "RSFJ");
            ReportsDirectory = Path.Combine(rootDirectory, "Reports");
            BackupsDirectory = Path.Combine(rootDirectory, "Backups");

            Directory.CreateDirectory(ReportsDirectory);
            Directory.CreateDirectory(BackupsDirectory);

            DatabaseFile = Path.Combine(rootDirectory, "database.rsfj");
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

        /// <summary>
        /// Creates a file which has time-stamp suffixed in it's name, and writes content into it.
        /// </summary>
        /// <param name="Directory">The directory inside which to create this file.</param>
        /// <param name="FileName">The name of the file to create.</param>
        /// <param name="FileExtension">The extension of the file to create.</param>
        /// <param name="Content">The content to write in the file.</param>
        public void CreateTimeStampFile(string Directory, string FileName, string FileExtension, string Content)
        {
            var file = Path.Combine(Directory,
                string.Format("{0}-{1}.{2}", FileName, DateTime.Now.ToString("yyyyMMddHHmmss"), FileExtension));

            CreateFile(file, Content);
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RSFJ.Services
{
    /// <summary>
    /// Service to handle data context related business.
    /// </summary>
    public class DataContextService
    {
        /// <summary>
        /// The only instance of DataContextService.
        /// </summary>
        public static DataContextService Instance => _Instance ?? (_Instance = new DataContextService());
        private static DataContextService _Instance;

        /// <summary>
        /// Current application's data context.
        /// </summary>
        public Model.DataContext DataContext { get; set; }

        /// <summary>
        /// Saves the current data context object in database file.
        /// </summary>
        public void Save()
        {
            var sBuilder = new StringBuilder();

            using (var stream = new StringWriter(sBuilder))
            {
                var serializer = new XmlSerializer(typeof(Model.DataContext));
                serializer.Serialize(stream, DataContext);
            }

            StorageService.Instance.CreateFile(StorageService.DatabaseFile, sBuilder.ToString());
        }

        /// <summary>
        /// Loads the saved application data context from the database file.
        /// </summary>
        public void Load()
        {
            if (File.Exists(StorageService.DatabaseFile) == false)
            {
                DataContext = new Model.DataContext();
                DataContext.Load();
                return;
            }

            string data;
            using (var reader = new StreamReader(StorageService.DatabaseFile))
            {
                data = reader.ReadToEnd();
            }

            using (var stream = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(Model.DataContext));
                DataContext = (Model.DataContext)serializer.Deserialize(stream);
            }

            DataContext.Load();
        }

        public void StartOver()
        {
            StorageService.Instance.DeleteFile(StorageService.DatabaseFile);

            RegistoryService.Instance.Reset();
        }

        /// <summary>
        /// Creates a backup file of the last saved application data context. Caller should take care that the
        /// latest application data context changes are saved before backing up.
        /// </summary>
        public void Backup()
        {
            var backupFile = StorageService.Instance.CreateTimeStampFile(StorageService.BackupsDirectory, "Backup", "rsfj", string.Empty);
            new FileInfo(StorageService.DatabaseFile).CopyTo(backupFile, true);
        }

        /// <summary>
        /// Restores a previously backed up data context from a backup file.
        /// </summary>
        /// <param name="BackupFile">Path to the backup file to restore.</param>
        public void Restore(string BackupFile)
        {
            File.Delete(StorageService.DatabaseFile);
            new FileInfo(BackupFile).CopyTo(StorageService.DatabaseFile);
            Load();
        }
    }
}

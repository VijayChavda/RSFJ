using System;
namespace RSFJ.Services
{
    /// <summary>
    /// Service to handle reporting related business.
    /// </summary>
    public class ReportingService
    {
        /// <summary>
        /// The only instance of ReportingService.
        /// </summary>
        public static ReportingService Instance => new ReportingService();

        /// <summary>
        /// Initializes ReportingService.
        /// </summary>
        private ReportingService() { }

        /// <summary>
        /// Creates a report with the given message and exception inside the reports directory.
        /// </summary>
        /// <param name="Message">A message provided by user providing hopeful explainations.</param>
        /// <param name="ServiceException">The exception that was thrown.</param>
        public void CreateReport(string Message, Exception ServiceException)
        {
            var headingLine = string.Concat(Environment.NewLine, "********************", Environment.NewLine);

            string message = string.Concat(
                DateTime.Now, Environment.NewLine,
                headingLine, "USER_MESSAGE", headingLine, Message, Environment.NewLine,
                headingLine, "SERVICE_EXCEPTION", headingLine, ServiceException, Environment.NewLine,
                headingLine, "INNER_EXCEPTION", headingLine, ServiceException.InnerException
                );

            StorageService.Instance.CreateTimeStampFile(StorageService.Instance.ReportsDirectory, "Log", "log", message);
        }

        /// <summary>
        /// Creates a report with the given message inside the reports directory.
        /// </summary>
        /// <param name="Message"></param>
        public void CreateReport(string Message)
        {
            string message = string.Concat(DateTime.Now, Environment.NewLine, Message);

            StorageService.Instance.CreateTimeStampFile(StorageService.Instance.ReportsDirectory, "Message", "msg", message);
        }
    }
}

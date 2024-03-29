﻿using System;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.Services
{
    /// <summary>
    /// Service to handle app verification.
    /// </summary>
    public class AppVerificationService
    {
        public const string ERR_NO_INTERNET = "ERR_NO_INTERNET";   //Represents no connectivity with internet.
        public const string ERR_NO_SERVER = "ERR_NO_SERVER";   //Represents no connectivity with verification server.
        public const string ERR_REQUEST = "ERR_REQUEST";   //Represents other local connectivity error.

        /// <summary>
        /// The only instance of StorageService.
        /// </summary>
        public static AppVerificationService Instance => _Instance ?? (_Instance = new AppVerificationService());
        private static AppVerificationService _Instance;

        /// <summary>
        /// The machine ID of the machine where the app is installed.
        /// </summary>
        private readonly string MID;

        /// <summary>
        /// Initializes AppVerificationService.
        /// </summary>
        private AppVerificationService()
        {
            MID = GetMachineID();
        }

        /// <summary>
        /// Sends an activation request to the server.
        /// </summary>
        /// <param name="key">They key with which the app will be activated.</param>
        /// <returns>Response from the server, or one of the following:
        /// 'ERR_NO_INTERNET' if Google server could not be reached.
        /// 'ERR_REQUEST' if RSFJ Authentication server could not be reached.
        /// 'ERR_REQUEST' if the request failed due to some other reason.</returns>
        public async Task<string> RequestActivationAsync(string key)
        {
            if (CheckForServerConnection() == false)
            {
                if (CheckForInternetConnection() == false)
                {
                    return "ERR_NO_INTERNET";
                }

                return "ERR_NO_SERVER";
            }

            try
            {
                var url = key == null ?
                    string.Format("{0}?uid={1}&app=rsfj", Properties.Settings.Default.VerificationURL, MID) :
                    string.Format("{0}?key={1}&uid={2}&app=rsfj", Properties.Settings.Default.VerificationURL, key, MID);

                var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };

                var response = await httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.GetType() == typeof(TaskCanceledException) ? "ERR_TIMEOUT" : "ERR_REQUEST";
            }
        }

        /// <summary>
        /// Sends an verification request to the server.
        /// </summary>
        /// <returns>Response from the server, or one of the following:
        /// 'ERR_NO_INTERNET' if Google server could not be reached.
        /// 'ERR_REQUEST' if RSFJ Authentication server could not be reached.
        /// 'ERR_REQUEST' if the request failed due to some other reason.</returns>
        public async Task<string> RequestVerificationAsync()
        {
            return await RequestActivationAsync(null);
        }

        /// <summary>
        /// Gets a hash value that is unique for the current machine. 
        /// Motherboard and CPU serial numbers are used to compute the hash.
        /// </summary>
        /// <returns>A hash value unique for the current machine.</returns>
        private string GetMachineID()
        {
            var ss = SecurityService.Instance;
            return ss.Hash(string.Concat(ss.Hash(GetCPUId()), ss.Hash(GetMotherboardId())));
        }

        /// <summary>
        /// Gets the CPU ID.
        /// </summary>
        /// <returns></returns>
        private string GetCPUId()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorID"].ToString();
            }
            return id;
        }

        /// <summary>
        /// Gets the Motherboard ID.
        /// </summary>
        /// <returns></returns>
        private string GetMotherboardId()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string serial = "";
            foreach (ManagementObject mo in moc)
            {
                serial = (string)mo["SerialNumber"];
            }
            return serial;
        }

        /// <summary>
        /// Attempts to connect to the internet.
        /// </summary>
        /// <returns></returns>
        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("https://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to connect to the verification server.
        /// </summary>
        /// <returns></returns>
        private bool CheckForServerConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(Properties.Settings.Default.VerificationURL))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Represents various server responses.
    /// </summary>
    public enum AppVerificationServerResponses
    {
        /// <summary>
        /// The application was verified.
        /// </summary>
        OK_VERIFIED,

        /// <summary>
        /// The application was activated.
        /// </summary>
        OK_ACTIVATED,

        /// <summary>
        /// User-Id was not provided.
        /// </summary>
        ERR_NO_ID,

        /// <summary>
        /// Application name was not provided.
        /// </summary>
        ERR_NO_APP,

        /// <summary>
        /// Key was not provided.
        /// </summary>
        ERR_NO_KEY,

        /// <summary>
        /// The provided key is already in use.
        /// </summary>
        ERR_USED,

        /// <summary>
        /// The provided key was incorrect.
        /// </summary>
        ERR_WRONG_KEY,

        /// <summary>
        /// One of the SQL query generated error.
        /// </summary>
        ERR_DB_1,

        /// <summary>
        /// One of the SQL query generated error.
        /// </summary>
        ERR_DB_2,

        /// <summary>
        /// One of the SQL query generated error.
        /// </summary>
        ERR_DB_3,

        /// <summary>
        /// Connection with the database server could not be established.
        /// </summary>
        ERR_CON,

        /// <summary>
        /// Some unknown error during activation occured.
        /// </summary>
        ERR_ACTIVATION
    }
}

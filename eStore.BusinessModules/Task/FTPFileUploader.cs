using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace eStore.BusinessModules.Task
{
    public class FTPFileUploader : TaskBase
    {
        private string _localFullPath;
        private string _remotePath;
        private string _remoteFilename;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localFullPath">Full path of uploading file including file name</param>
        /// <param name="remotePath">Targeting folder at FTP site</param>
        /// <param name="remoteFilename">Targeting file name at FTP site. If empty, remote file name will be same as original file name</param>
        public FTPFileUploader(String localFullPath, string remotePath = "", string remoteFilename = "") 
        {
            _localFullPath = localFullPath;
            _remotePath = remotePath;
            _remoteFilename = remoteFilename;
        }

        #region Task_Execution method

        /// <summary>
        /// This method check the existance of uploading file
        /// </summary>
        /// <returns></returns>
        public override bool PreProcess()
        {
            bool status = base.PreProcess();
            //check the existance of local file
            if (String.IsNullOrEmpty(_localFullPath) || File.Exists(_localFullPath) == false)
                status = false;

            return status;
        }


        /// <summary>
        /// This method will be executed to publish event
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object execute(object obj)
        {
                try
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.Get("syncFile2LimeLightServer").ToLower() == "true")
                    {
                        FtpLib.FtpHelper uploader = new FtpLib.FtpHelper(_remotePath);
                        uploader.upload(_localFullPath, _remoteFilename, _remoteFilename);
                    }
                    OnCompleted();
                    return true;
                }
                catch (Exception)
                {
                    OnFailed();
                    return null;
                }
        }

        #endregion //Task_Execution method
    }
}

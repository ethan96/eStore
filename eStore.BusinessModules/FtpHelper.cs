using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;

namespace FtpLib
{

    public class FtpHelper
    {
        private string _ftpServerIP = "ftp://advantech.upload.llnw.net/eStore";        
        private string _ftpUserID = "advantech-ht";
        private string _ftpPassword = "dcy54p";
        private string _ftpRootURI 
        {
            get
            {
                if (!string.IsNullOrEmpty(_targetPath))
                    return string.Format("{0}/{1}/", _ftpServerIP, _targetPath);
                else
                    return (_ftpServerIP + "/").Replace("//", "/");
            }
        } //Ftp目标Full 地址
        private string _targetPath; //目标文件夹路径

        public FtpHelper(string targetPath = "")
        {
            _targetPath = targetPath;
        }

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        public FtpHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            _ftpServerIP = FtpServerIP;
            _ftpUserID = FtpUserID;
            _ftpPassword = FtpPassword;
            _targetPath = FtpRemotePath;
        }

        /// <summary>
        /// This method will upload specified file to FTP server
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="remoteFilename"></param>
        public bool upload(String localFilePath, string remoteFilename = "", string remoteFilePath = "")
        {
            WebClient request = null;
            try
            {
                var path = localFilePath.Replace(@"\", "/").Replace("//", "/").Replace(@"C:/eStoreResources3C/", ""); //eStore文件目前都存在C:\eStoreResources3C下

                //check if remote file name is different from local file name, if not, use local file.
                if (String.IsNullOrEmpty(remoteFilename))
                    remoteFilename = path.Substring(path.LastIndexOf("/") + 1);
                if (!string.IsNullOrEmpty(remoteFilePath))
                    _targetPath = remoteFilePath;
                if (string.IsNullOrEmpty(_targetPath))
                    _targetPath = path.Substring(0, path.LastIndexOf("/"));

                MakeDir(); //同步文件夹并创建新文件夹

                string remoteFullURI = _ftpRootURI + remoteFilename;
                request = new WebClient();
                request.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                request.UploadFile(remoteFullURI, localFilePath);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                request.Credentials = null;
                request.Dispose();
                request = null;
            }
        }

        /// <summary>
        /// 下载Ftp
        /// </summary>
        public void downloadAllFile(string localFilePath, bool isDeleteRemoteFile = false)
        {
            var files = GetFileList("*.xml");
            foreach (var fileName in files)
            {
                if (!string.IsNullOrEmpty(fileName))
                    downloadFile(fileName, localFilePath, isDeleteRemoteFile);
            }
        }

        /// <summary>
        /// download file
        /// </summary>
        /// <param name="remoteFileName">ftp server file name only name</param>
        /// <param name="localFileFullPath">local file full path</param>
        /// <param name="isDeleteRemoteFile">is delete ftp sever file</param>
        public void downloadFile(string remoteFileName,string localFileFullPath,bool isDeleteRemoteFile = false)
        {
            FtpWebRequest reqFTP = null;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            FileStream outputStream = null;

            try
            {
                if (!Directory.Exists(localFileFullPath))
                    Directory.CreateDirectory(localFileFullPath);
                if (!File.Exists(localFileFullPath + "/" + remoteFileName))
                {

                    outputStream = new FileStream((localFileFullPath + "/" + remoteFileName).Replace("//", "/"), FileMode.Create);
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpRootURI + remoteFileName));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.KeepAlive = false;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                    response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                    if (isDeleteRemoteFile)
                        delete(remoteFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reqFTP != null)
                    reqFTP = null;
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                    ftpStream = null;
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream.Dispose();
                    outputStream = null;
                }
            }
        }

        /// <summary>
        /// 判断目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string path, string RemoteDirectoryName)
        {
            string[] dirList = GetDirectoryList(path);
            if (dirList != null)
            {
                foreach (string str in dirList)
                {
                    if (str.Trim() == RemoteDirectoryName.Trim())
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dirName"></param>
        public void MakeDir()
        {
            string remotePath = _targetPath.Replace(@"\", "/").Replace("//", "/");
            var paths = remotePath.Split('/');
            string _path = _ftpServerIP;
            foreach (var s in paths)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    MakeDir(_path, s);
                    _path += "/" + s;
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">ftp server file name only name</param>
        public void delete(string fileName)
        {
            FtpWebRequest reqFTP = null;
            FtpWebResponse response = null;
            Stream datastream = null;
            StreamReader sr = null;

            try
            {
                string uri = _ftpRootURI + fileName;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                datastream = response.GetResponseStream();
                sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reqFTP != null)
                    reqFTP = null;
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (datastream != null)
                {
                    datastream.Close();
                    datastream.Dispose();
                    datastream = null;
                }
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
        }

        /// <summary>
        /// create ftp 文件夹
        /// </summary>
        /// <param name="_path"></param>
        void MakeDir(string _path, string DirectoryName)
        {
            if (!DirectoryExist(_path, DirectoryName))
            {
                FtpWebRequest reqFTP = null;
                FtpWebResponse response = null;
                Stream ftpStream = null;

                try
                {
                    // dirName = name of the directory to create.
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_path + "/" + DirectoryName));
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                    response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (reqFTP != null)
                        reqFTP = null;
                    if (response != null)
                    {
                        response.Close();
                        response = null;
                    }
                    if (ftpStream != null)
                    {
                        ftpStream.Close();
                        ftpStream.Dispose();
                        ftpStream = null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前目录下所有的文件夹列表(仅文件夹)
        /// </summary>
        /// <returns></returns>
        public string[] GetDirectoryList(string path)
        {
            string[] drectory = GetFilesDetailList(path);
            string m = string.Empty;
            if (drectory != null)
            {
                foreach (string str in drectory)
                {
                    int dirPos = str.IndexOf("<DIR>");
                    if (dirPos > 0)
                    {
                        m += str.Substring(dirPos + 5).Trim() + "\n";
                    }
                    else if (str.Trim().Substring(0, 1).ToUpper() == "D")
                    {
                        string dir = str.Substring(59).Trim();
                        if (dir != "." && dir != "..")
                        {
                            m += dir + "\n";
                        }
                    }
                }
            }
            char[] n = new char[] { '\n' };
            return m.Split(n);
        }

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDetailList(string path)
        {
            FtpWebRequest ftp = null;
            WebResponse response = null;
            StreamReader reader = null;
            StringBuilder result = null;

            try
            {
                result = new StringBuilder();

                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
                ftp.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                response = ftp.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                var strr = result.ToString().Split('\n');
                return strr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ftp != null)
                    ftp = null;
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
                if (result != null)
                {
                    result.Clear();
                    result = null;
                }
            }
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public string[] GetFileList(string mask)
        {
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP = null;
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpRootURI));
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(_ftpUserID, _ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (mask.Trim() != string.Empty && mask.Trim() != "*.*")
                    {
                        if (mask.LastIndexOf('.') >= 0 && line.LastIndexOf('.') >= 0 &&
                            mask.Substring(mask.LastIndexOf('.')) == line.Substring(line.LastIndexOf('.')))
                        {
                            result.Append(line);
                            result.Append("\n");
                        }
                    }
                    else
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                if (result.ToString().LastIndexOf('\n') != -1)
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reqFTP != null)
                    reqFTP = null;
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
            }
        }
    }

}
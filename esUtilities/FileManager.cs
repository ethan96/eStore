using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace esUtilities
{
    public class FileManager
    {
        public enum eStoreFileType { NetTerm, Reseller, PurchaseOrder }
        public enum eStoreFileExtDocType { AllExt, ImageAndDocument, Image, Document }

        public static string save(eStoreFileType type, FileUpload fileUpload)
        {
            string rlt = string.Empty;
            if (fileUpload.HasFile)
            {
                try
                {
                    string fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
                    string path = System.Configuration.ConfigurationManager.AppSettings["LocalPicture_path"];
                    if (string.IsNullOrEmpty(path))
                    {
                        eStore.Utilities.eStoreLoger.Fatal("UserCertificateFiles missing in web.config");
                    }
                    else
                    {
                        path = string.Format("{0}\\{1}", path, type.ToString());
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                        string fileName = System.Guid.NewGuid().ToString() + fileExt;
                        path = string.Format("{0}\\{1}", path, fileName);
                        fileUpload.SaveAs(path);
                        rlt = fileName;
                    }
                }
                catch (Exception ex)
                {
                    eStore.Utilities.eStoreLoger.Error("eStoreFileManager save error. type:" + type.ToString(), "", "", "", ex);
                }

            }

            return rlt;
        }
        public static string downloadURL(eStoreFileType type, string fileName)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["LocalPicture_path"];
            path = string.Format("{0}\\{1}\\{2}", path, type.ToString(), fileName);
            try
            {
                if (System.IO.File.Exists(path))
                {
                    return string.Format("/resource/{0}/{1}", type.ToString(), fileName);
                }
            }
            catch (Exception ex)
            {

                eStore.Utilities.eStoreLoger.Error("eStoreFileManager downloadURL error", "", "", "", ex);
            }

            return string.Empty;
        }

        public static Boolean isDocTypeSupported(eStoreFileExtDocType supportedDocType, string ext)
        {
            if (supportedDocType == eStoreFileExtDocType.ImageAndDocument)
            {
                string[] extFile = { ".bmp", ".jpg", ".ipg", ".gif", ".png", ".tif", ".tiff", ".pdf", ".doc", ".docx", ".rtf" };
                return extFile.Contains(ext);
            }
            else if (supportedDocType == eStoreFileExtDocType.Image)
            {
                string[] extFile = { ".bmp", ".jpg", ".ipg", ".gif", ".png", ".tif", ".tiff" };
                return extFile.Contains(ext);
            }
            else if (supportedDocType == eStoreFileExtDocType.Document)
            {
                string[] extFile = { ".pdf", ".doc", ".docx", ".rtf" };
                return extFile.Contains(ext);
            }
            else
                return true;
        }

        /// <summary>
        /// 判断文件的大小
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public static bool isContentLengthSupported(FileUpload fileUpload)
        {
            bool rlt = false;
            if (fileUpload.HasFile)
            {
                if (fileUpload.PostedFile.ContentLength > 0 && fileUpload.PostedFile.ContentLength < 4 * 1024 * 1024)
                    rlt = true;
            }
            return rlt;
        }
    }
}

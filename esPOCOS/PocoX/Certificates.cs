using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;


namespace eStore.POCOS
{
    public partial class Certificates
    {
        private string _certificateName;
        public string CertificateName
        {
            get { return _certificateName; }
            set { _certificateName = value; }
        }

        private string _certificateImagePath;
        public string CertificateImagePath
        {
            get { return _certificateImagePath; }
            set { _certificateImagePath = value; }
        }
    }
}

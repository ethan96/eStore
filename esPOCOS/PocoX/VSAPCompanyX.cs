using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class VSAPCompany
    {
        private String _companyNameX = null;
        public String companyNameX
        {
            get
            {
                if (_companyNameX == null)
                {
                    _companyNameX = CompanyName.Replace(@"<br />",  ",");
                    _companyNameX = _companyNameX.Replace(@"<br/>",  ",");
                    _companyNameX = _companyNameX.Replace(@"<",  " ");
                    _companyNameX = _companyNameX.Replace(@">",  " ");
                }

                return _companyNameX;
            }
        }
    }
}

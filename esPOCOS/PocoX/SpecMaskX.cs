using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS{ 

    public partial class SpecMask{
        
        //�Ƿ�ɾ��
        private bool _IsDelete = false;
        public bool IsDelete
        {
            get { return _IsDelete; }
            set { _IsDelete = value; }
        }
    
    } 

}
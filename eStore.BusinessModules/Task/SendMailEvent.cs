using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using esUtilities;

namespace eStore.BusinessModules.Task
{
    public class SendMailEvent : TaskBase
    {
        private EMailOrder _emailTemp = null;
        private EMailNoticeTemplate _noticeTemplate = null;

        public SendMailEvent(EMailOrder emailTemp, EMailNoticeTemplate noticeTemplate)
        {
            this._emailTemp = emailTemp;
            this._noticeTemplate = noticeTemplate;
        }

        public override bool PreProcess()
        {
            bool status = base.PreProcess();
            if (_emailTemp == null || _noticeTemplate == null)
                status = false;
            return status;
        }

        public override object execute(object obj)
        {
            try
            {
                EMailReponse orderDeptEmailResponse = new EMailReponse();
                EMailReponse customerEmailResponse = new EMailReponse();

                if (testMode() == true)
                {
                    orderDeptEmailResponse = _noticeTemplate.sendMail(_noticeTemplate.TestingOrderDeptEmail, _emailTemp.CustomerEmail, _emailTemp.MailFromName, _emailTemp.MailSubject, _emailTemp.InternalResult, _noticeTemplate.Store.storeID.ToUpper());

                    customerEmailResponse = _noticeTemplate.sendMail(_emailTemp.CustomerEmail, _emailTemp.EmailGroup.Split(';')[0], _emailTemp.MailFromName, _emailTemp.MailSubject, _emailTemp.CustomerResult, _noticeTemplate.Store.storeID.ToUpper(), "", _noticeTemplate.TestingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    orderDeptEmailResponse = _noticeTemplate.sendMail(_emailTemp.EmailGroup, _emailTemp.CustomerEmail, _emailTemp.MailFromName, _emailTemp.MailSubject, _emailTemp.InternalResult, _noticeTemplate.Store.storeID.ToUpper(), "", _emailTemp.EmailGroup);

                    customerEmailResponse = _noticeTemplate.sendMail(_emailTemp.CustomerEmail, _emailTemp.EmailGroup.Split(';')[0], _emailTemp.MailFromName, _emailTemp.MailSubject, _emailTemp.CustomerResult, _noticeTemplate.Store.storeID.ToUpper(), "", _emailTemp.EmailGroup);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
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

    }
}

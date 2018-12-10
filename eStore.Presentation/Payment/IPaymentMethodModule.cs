using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.Payment
{
    public interface IPaymentMethodModule
    {
        POCOS.Payment GetPaymentInfo();
        bool ValidateForm();
        string ClientValidataionFunction();
        bool PreLoad();
    }
}

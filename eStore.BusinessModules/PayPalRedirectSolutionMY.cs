using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    class PayPalRedirectSolutionMY : PayPalRedirectSolution
    {
        private PPConfiguration _config;
        protected override PPConfiguration config
        {
            get
            {
                if (_config == null)
                {
                    _config = new PPConfiguration();
                    if (_testing)
                    {
                        _config.STANDARD_IDENTITY_TOKEN = "lPcNRZWkFzMX-zewD-yhUZtDH-nxEwtlHLlEv2MM6vrqrJU3-YH88zXpZJa";
                        _config.STANDARD_EMAIL_ADDRESS = "cl.ong@advantech.com";
                        _config.PAYPAL_WEBSCR_URL = "https://www.paypal.com/cgi-bin/webscr";
                        _config.CERT_ID = "GEFPQGZWLTGQL";
                        _config.signerPfxPath = configPath + @"\SAP\SAPcertMY\mycert.p12";
                        _config.signerPfxPassword = "88888888";
                        _config.paypalCertPath = configPath + @"\SAP\SAPcertMY\paypal_cert_pem.txt";

                    }
                    else
                    {
                        _config.STANDARD_IDENTITY_TOKEN = "lPcNRZWkFzMX-zewD-yhUZtDH-nxEwtlHLlEv2MM6vrqrJU3-YH88zXpZJa";
                        _config.STANDARD_EMAIL_ADDRESS = "cl.ong@advantech.com";
                        _config.PAYPAL_WEBSCR_URL = "https://www.paypal.com/cgi-bin/webscr";
                        _config.CERT_ID = "M2GHCVCHSFM6W";
                        _config.signerPfxPath = configPath + @"\SAP\SAPcertMY20170413\mycert.p12";
                        _config.signerPfxPassword = "88888888";
                        _config.paypalCertPath = configPath + @"\SAP\SAPcertMY20170413\paypal_cert_pem.txt";
                    }
                }
                return _config;
            }

        }

    }
}

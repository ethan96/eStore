using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.BusinessModules
{
    public class VATvalidation
    {
        private User _user = null;
        
        private string _vatNumber;
        public string VATnumber
        {
            get { return _vatNumber; }
        }

        private string _vatStatus;
        public string VATStatus
        {
            get { return _vatStatus; }
            set { _vatStatus = value; }
        }

        private string _vatProcessBy;
        public string VATProcessBy
        {
            get { return _vatProcessBy; }
            set { _vatProcessBy = value; }
        }

        private DateTime _resultDate;
        public DateTime ResultDate
        {
            get { return _resultDate; }
            set { _resultDate = value; }
        }

        private string _vatResult;
        public string VATResult
        {
            get { return _vatResult; }
            set { _vatResult = value; }
        }

        /// <summary>
        /// Existence validation will validate vat number via EC web service(http://ec.europa.eu/taxation_customs/vies/checkVatService.wsdl)
        /// Format validation will only validate the vat number structure.
        /// </summary>
        public enum Method { ExistenceValidation, FormatValidation };
        public Method Type
        {
            get;
            set;
        }

        //Validation result
        private bool _valid;
        public bool Valid
        {
            get { return _valid; }
        }

        //Constructor
        public VATvalidation(string vatNum, Method type, User user)
        {
            _user = user;
            // Trim and convert to upcase
            _vatNumber = vatNum.Trim().ToUpper() ;
            Type = type;
        }

        public VATvalidation(string vatNum, Method type)
        {
            // Trim and convert to upcase
            _vatNumber = vatNum.Trim().ToUpper();
            Type = type;
        }

        public bool checkVAT()
        {
            // Correct VAT Number
            correctVATnumber();
            if (Type == Method.ExistenceValidation)
            { 
                // Wait to compose
            }
            else if (Type == Method.FormatValidation)
            {
                validateVATformat();
            }

            return this._valid;
        }

        /// <summary>
        /// Correct VAT Number.
        /// 1. replace "-"
        /// 2. If number doesn't contain prefix of country code, will add it.
        /// </summary>
        private void correctVATnumber()
        {
            // replace " " and "-","."
            _vatNumber = _vatNumber.Replace(" ", "");
            _vatNumber = _vatNumber.Replace("-", "");
            _vatNumber = _vatNumber.Replace(".", "");

            string prefix = _vatNumber.Substring(0,2);
            Regex rgx = new Regex(@"^[A-Z]");
            try
            {
                if (rgx.IsMatch(_vatNumber) == false)
                {
                    string addPrefix = _user.mainContact.countryCodeX;
                    _vatNumber = addPrefix + _vatNumber;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("VAT validation failed due to missing user object", "", "","", ex);
            }
        }


        private void validateVATformat()
        {
            string countryCode = _vatNumber.Substring(0,2);
            string cNumber = _vatNumber.Substring(2) ;
            // Now look at the check digits for those countries we know about.
            switch (countryCode)
            {
                case "AT":
                    _valid = ATVATCheckDigit(cNumber);
                    break;

                case "BE":
                    _valid = BEVATCheckDigit(cNumber);
                    break;

                case "BG":
                    // The SIMA _validation rules are incorrect for Bulgarian numbers.
                    _valid = BGVATCheckDigt(cNumber);
                    break;

                case "CY":
                    _valid = CYVATCheckDigit(cNumber);
                    break;

                case "CZ":
                    _valid = CZVATCheckDigit(cNumber);
                    break;
                case "DE":

                    _valid = DEVATCheckDigit(cNumber);
                    break;

                case "DK":
                    _valid = DKVATCheckDigit(cNumber);
                    break;

                case "EE":
                    _valid = EEVATCheckDigit(cNumber);
                    break;

                case "GR":
                case "EL":
                    _valid = ELVATCheckDigit(cNumber);
                    break;

                case "ES":
                    _valid = ESVATCheckDigit(cNumber);
                    break;

                case "EU":
                    _valid = EUVATCheckDigit(cNumber);
                    break;

                case "FI":
                    _valid = FIVATCheckDigit(cNumber);
                    break;

                case "FR":
                    _valid = FRVATCheckDigit(cNumber);
                    break;

                case "GB":
                    _valid = UKVATCheckDigit(cNumber);
                    break;

                case "HU":
                    _valid = HUVATCheckDigit(cNumber);
                    break;

                case "IE":
                    _valid = IEVATCheckDigit(cNumber);
                    break;

                case "IT":
                    _valid = ITVATCheckDigit(cNumber);
                    break;

                case "LT":
                    _valid = LTVATCheckDigit(cNumber);
                    break;

                case "LU":
                    _valid = LUVATCheckDigit(cNumber);
                    break;

                case "LV":
                    _valid = LVVATCheckDigit(cNumber);
                    break;

                case "MT":
                    _valid = MTVATCheckDigit(cNumber);
                    break;

                case "NL":
                    _valid = NLVATCheckDigit(cNumber);
                    break;

                case "PL":
                    _valid = PLVATCheckDigit(cNumber);
                    break;

                case "PT":
                    _valid = PTVATCheckDigit(cNumber);
                    break;

                case "RO":
                    _valid = ROVATCheckDigit(cNumber);
                    break;

                case "SE":
                    _valid = SEVATCheckDigit(cNumber);
                    break;

                case "SI":
                    _valid = SIVATCheckDigit(cNumber);
                    break;

                case "SK":
                    _valid = SKVATCheckDigit(cNumber);
                    break;

                default:
                    _valid = false;
                    break;
            }
        }

        /// <summary>
        /// Checks the check digits of an Austrian VAT number.
        /// </summary>
        /// <returns></returns>
        private bool ATVATCheckDigit(string cNumber)
        {
            //cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;

            string cNum = cNumber.Substring(1);
            double total = 0;
            int[] multipliers = new int[7] {1,2,1,2,1,2,1};
            double temp = 0;
            
            // Extract the next digit and multiply by the appropriate multiplier. 
            for (int i = 0; i < 7; i++)
            {
                string c = cNum[i].ToString();
                temp = Double.Parse(c) * multipliers[i];
                if (temp > 9)
                    total = total + Math.Floor(temp / 10) + temp % 10;
                else
                    total = total + temp;
            }

            // Establish check digit.
            total = 10 - (total + 4) % 10;
            if (total == 10)
                total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            string n = cNum.Substring(6,2);
            if (total == Double.Parse(n))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of an Belgium VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool BEVATCheckDigit(string cNumber)
        {
            //cNumber should be 10 digits
            if (cNumber.Length == 10 && cNumber.Substring(0, 1) != "0")
                return false;

            // Nine digit numbers have a prefix 0 inserted at the front.
            if (cNumber.Length == 9)
                cNumber = "0" + cNumber;

            // Moudle 97 check on last nine digits
            if(97 - Double.Parse(cNumber.Substring(0,8))% 97 == Double.Parse(cNumber.Substring(8,2)))
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Checks the check digits of an Bulgaria VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool BGVATCheckDigt(string cNumber)
        {
            // due to refered javascrip doesn't contain Bulgaria, need to find out check algorithm. Here just validate the string length.
            //cNumber should be 9 to 10 digits
            if (cNumber.Length == 9 || cNumber.Length == 10)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of an Cyprus(Cypriot) VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool CYVATCheckDigit(string cNumber)
        {
            //cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;
            else
            {
                // Extract the next digit and multiply by hte counter.
                double total = 0;
                for (int i = 0; i < 8; i++)
                {
                    int temp = int.Parse(cNumber[i].ToString());
                    if (i % 2 == 0)
                    {
                        switch (temp)
                        {
                            case 0:
                                temp = 1;
                                break;

                            case 1:
                                temp = 0;
                                break;

                            case 2:
                                temp = 5;
                                break;

                            case 3:
                                temp = 7;
                                break;

                            case 4:
                                temp = 9;
                                break;

                            default:
                                temp = temp * 2 + 3;
                                break;
                        }
                    }

                        total += temp;
                    }

                    // Establish check digit using modules 26, and translate to char. equivalent.
                    total %= 26;
                    total += 65;
                    string strTotal = "";
                    strTotal = total.ToString();

                    // Check to see if the check digit given is correct, acsii
                    Int32 checkCode = System.Convert.ToInt32(char.Parse(cNumber.Substring(8, 1)));
                    if (strTotal == checkCode.ToString())
                        return true;
                    else
                        return false;
                }
        }

        /// <summary>
        ///  Checks the check digits of an Czech Republic VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool CZVATCheckDigit(string cNumber)
        {
            //cNumber should be 9 to 10 digits
            if (cNumber.Length < 8 || cNumber.Length > 10)
                return false;

            double total = 0;
            int[] multipliers = new int[7] {8,7,6,5,4,3,2 };

            // Extract the next digit and multiply by the counter.
            for (int i = 0; i < 7; i++)
            {
                total += Double.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            // Establish check digit
            total = 11 - total % 11;
            if (total == 10) 
                total = 0;
            if (total == 11)
                total = 1;
            
            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            if (total == Double.Parse(cNumber.Substring(7,1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of an German VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool DEVATCheckDigit(string cNumber)
        {
            //cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;

            int product = 10;
            int sum = 0;
            int checkdigit = 0;
            for (int i = 0; i < 8; i++)
            { 
                // Extract the next digit and implement perculiar algorithm.
                sum = (int.Parse(cNumber[i].ToString()) + product) % 10;
                if (sum == 0)
                    sum = 10;
                product = (2 * sum) % 11;
            }

            // Establish check digit.
            if (11 - product == 10)
                checkdigit = 0;
            else
                checkdigit = 11 - product;

              // Compare it with the last two characters of the VAT number. If the same, 
              // then it is a valid check digit.
            if (checkdigit == int.Parse(cNumber.Substring(8, 1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of an Denmark VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool DKVATCheckDigit(string cNumber)
        {
            //cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[8] { 2, 7, 6, 5, 4, 3, 2, 1 };

            // Extract the next digit and multiply by the counter.
            for (int i = 0; i < 8; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digit
            total %= 11;
            
            //The remainder should be 0 for it to be valid.
            return true;
        }

        /// <summary>
        ///  Checks the check digits of an Estonia VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool EEVATCheckDigit(string cNumber)
        {
            // cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;
            
            int total = 0;
            int[] multipliers = new int[8] { 3,7,1,3,7,1,3,7};

            for (int i = 0; i < 8; i++)
            {
                total+= int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits using modules 10.
            total = 10 - total % 10;

            if (total == 10)
                total = 0;

              // Compare it with the last character of the VAT number. If it is the same, 
              // then it's a valid check digit.
              if (total == int.Parse(cNumber.Substring(8,1)))
                return true;
              else 
                return false;
        }

        /// <summary>
        /// Checks the check digits of an Greece VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool ELVATCheckDigit(string cNumber)
        {
            // cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;

            int total = 0;
            int[] multipliers = new int[8] { 256,128,64,32,16,8,4,2 };

            //8 character numbers should be prefixed with an 0.
            if (cNumber.Length == 8)
                cNumber += "0";

            //Extract the next digit and multiply by the counter.
            for (int i = 0; i < 8; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digit.
            total %= 11;
            if (total > 9)
                total = 0;

            //Compare it with the last character of the VAT number. If it is the same,
            //then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(8, 1)))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Checks the check digits of an Spain VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool ESVATCheckDigit(string cNumber)
        {
            // cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;

            int total = 0;
            int temp = 0;
            decimal tempD = 0;
            int[] multipliers = new int[7] { 2, 1, 2, 1, 2, 1, 2 };

            Regex pattern1 = new Regex(@"^[A-H]\d{8}$");
            Regex pattern2 = new Regex(@"^[N|P|Q|S]\d{7}[A-Z]$");
            Regex pattern3 = new Regex(@"^[0-9]{8}[A-Z]$");

            // With profit companies
            if (pattern1.IsMatch(cNumber))
            { 
                //Extract the next digit and multiply by the counter.
                for (int i = 0; i < 7; i++)
                {
                    temp = int.Parse(cNumber[i + 1].ToString()) * multipliers[i];
                    if (temp > 9)
                    {
                        tempD = System.Convert.ToDecimal(temp / 10);
                        total += System.Convert.ToInt32(Math.Floor(tempD)) + temp % 10;
                    }
                    else
                    {
                        total += temp;
                    }
                }

                // Now calculate the check digit itself.
                total = 10 - total % 10;
                if (total == 10)
                    total = 0;

                //Compare it with the last character of the VAT number. If it is the same, 
                //then it's a valid check digit.
                if (total == int.Parse(cNumber.Substring(8, 1)))
                    return true;
                else
                    return false;
            }

            //Non-profit companies
            else if (pattern2.IsMatch(cNumber))
            { 
                //Extract the next digit and multiply by the counter.
                for (int i = 0; i < 7; i++)
                {
                    temp = int.Parse(cNumber[i + 1].ToString()) * multipliers[i];
                    if (temp > 9)
                    {
                        tempD = System.Convert.ToDecimal(temp / 10);
                        total += System.Convert.ToInt32(Math.Floor(tempD)) + temp % 10;
                    }
                    else
                    {
                        total += temp;
                    }
                }

                // Now caculate the check digit itself.
                total = 10 - total % 10;
                total += 64;
                char checkChar = System.Convert.ToChar(total);

                //Compare it with the last character of the VAT number. If it is the same,
                //then it's  a valid check digit.
                if (checkChar.ToString() == cNumber.Substring(8, 1).ToString())
                    return true;
                else
                    return false;
            }

            //Personal number (NIF)
            else if (pattern3.IsMatch(cNumber))
            {
                //'TRWAGMYFPDXBNJZSQVHLCKE'.charAt(Number(vatnumber.substring(0, 8)) % 23);
                string validateString = "TRWAGMYFPDXBNJZSQVHLCKE";
                if (cNumber[8] == validateString[int.Parse(cNumber.Substring(0, 8)) % 23])
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        /// <summary>
        ///  Checks the check digits of an EU VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool EUVATCheckDigit(string cNumber)
        {
            // We know litle about EU numbers apart from the fact that the first 3 digits 
            // represent the country, and that there are nine digits in total.
            return true;
        }

        /// <summary>
        ///  Checks the check digits of a Finnish VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool FIVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[7] { 7, 9, 10, 5, 8, 4, 2 };

            //Extract the next digit and multiply by the counter
            for (int i = 0; i < 7; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            // Establish check digit.
            total = 11 - total % 11;
            if (total > 9)
                total = 0;

            //Compare it with the last character of the VAT number. It it is the same,
            // then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(7, 1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a French VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool FRVATCheckDigit(string cNumber)
        {
            // cNumber should be 11 digits
            if (cNumber.Length != 11)
                return false;

            // Extract the last 9 digits as an integer.
            int total = 0;
            total =int.Parse(cNumber.Substring(2,9));

            //Establish check digit.
            total = (total * 100 + 12) % 97;

            //Compare it with the last character of the VAT number. If it is the same,
            //then it's a valid check digit.
            if (total.ToString() == cNumber.Substring(0, 2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a UK VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool UKVATCheckDigit(string cNumber)
        {
            // cNumber format should be 999 9999 99
            // or 999 9999 99 999 
            // or GD999 
            // or HA999
            int cNumberLength = int.Parse(cNumber.Length.ToString());
            if (cNumberLength == 9 || cNumberLength == 12 || cNumberLength == 5)
            { }
            else
                return false;

            int[] multipliers = new int[7] {8,7,6,5,4,3,2 };
            //Goverment departments
            if (cNumber.Substring(0, 2) == "GD")
            {
                if (int.Parse(cNumber.Substring(2, 3)) < 500)
                    return true;
                else
                    return false;
            }
            else if (cNumber.Substring(0, 2) == "HA")
            {
                if (int.Parse(cNumber.Substring(2, 3)) < 499)
                    return true;
                else
                    return false;
            }
            else if (cNumberLength > 8 && cNumberLength < 11)
            {
                int total = 0;
                if (cNumberLength == 10 && cNumber.Substring(9, 1) == "3")
                    return false;

                // Extract the next digit and multiply by the counter.
                for (int i = 0; i < 7; i++)
                {
                    total += int.Parse(cNumber[i].ToString()) * multipliers[i];
                }

                // Establish check digits by subtracting 97 from total until negative.
                while (total > 0) {
                    total = total - 97;
                }    

                // Get the absolute value and compare it with the last two characters of the
                // VAT number. If the same, then it is a valid check digit.
                total = Math.Abs(total);
                if (total.ToString() == cNumber.Substring(7,2))
                    return true;
                else
                    return false;

            }
            else
            {
                return true;
            }
        }

        /// <summary>
        ///  Checks the check digits of a Hungary VAT number
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool HUVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[7] {9,7,3,1,9,7,3 };

            for (int i = 0; i < 7; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];            
            }

            //Establish check digit.
            total = 10 - total % 10;
            if (total == 10)
                total = 0;

              // Compare it with the last character of the VAT number. If it is the same, 
              // then it's a valid check digit.
            if (total.ToString() == cNumber.Substring(7,1))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Irish VAT number
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool IEVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[7] {8,7,6,5,4,3,2 };

            //If the code is in the old format, we need to convert it to the new
            Regex reg = new Regex(@"^[A-Z\*\+]");
            if(reg.IsMatch(cNumber))
            {
                cNumber = "0" + cNumber.Substring(1,6) + cNumber.Substring(0,1)+cNumber.Substring(6,1);
            }

            // Extract the next digit and multiply by the counter.
            for (int i = 0; i < 7; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digit using module 23, and translate to char. equivalent.
            string checkStr = "";
            total %= 23;
            if (total == 0)
                checkStr = "W";
            else
                total += 64;

          checkStr = System.Convert.ToChar(total).ToString();
          // Compare it with the last character of the VAT number. If it is the same, 
          // then it's a valid check digit.
            if (checkStr == cNumber.Substring(7, 1))
                return true;
            else
                return false;
        }

        private bool ITVATCheckDigit(string cNumber)
        {
            // cNumber should be 11 digits
            if (cNumber.Length != 11)
                return false;

            int total = 0;
            int[] multipliers = new int[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int temp;

            // The last three digits are the issuing office, and cannot exceed more 201
            temp = int.Parse(cNumber.Substring(0, 6));
            if (temp == 0)
                return false;

            temp = int.Parse(cNumber.Substring(7, 3));
            if((temp < 1) || (temp > 201))
                return false;

            double tempD;
            // Extract the next digit and multiply by the appropriate
            for (int i = 0; i < 10; i++)
            {
                string c = cNumber[i].ToString();
                temp = int.Parse(c) * multipliers[i];
                tempD = temp / 10;
                if (temp > 9)
                    total = total + System.Convert.ToInt16(Math.Floor(tempD)) + temp % 10;
                else
                    total = total + temp;
            }

                //Establish check digit.
                total = 10 - total % 10;
                if (total > 9)
                    total = 0;

                 // Compare it with the last character of the VAT number. If it is the same, 
                 // then it's a valid check digit.
                if (total.ToString() == cNumber.Substring(10,1))
                    return true;
                else
                    return false;
        }

        /// <summary>
        ///  Checks the check digits of a Lithuania VAT number
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool LTVATCheckDigit(string cNumber)
        {
            // cNumber should be 9 or 12 digits
            if (cNumber.Length != 9)
            {
                return true;
            }
            else if (cNumber.Length == 12)
            {
                // Extract the next digit and multiply by the counter+1.
                int total = 0;
                for (int i = 0; i < 8; i++)
                {
                    total += int.Parse(cNumber[i].ToString()) * (i + 1);
                }

                // Can have a double check digit calculation!
                if (total % 11 == 10)
                {
                    int[] multipliers = new int[8] { 3, 4, 5, 6, 7, 8, 9, 1 };
                    total = 0;

                    for (int i = 0; i < 8; i++)
                    {
                        total += int.Parse(cNumber[i].ToString()) * multipliers[i];
                    }
                }

                //Establish check digit.
                total %= 11;
                if (total == 10)
                    total = 0;

                //Compare it with the last character of the VAT number. If it is the same, 
                //then it's a valid check digit.
                if (total == int.Parse(cNumber.Substring(7, 1)))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Luxembourg VAT number
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool LUVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            if(int.Parse(cNumber.Substring(0,6)) % 89 == int.Parse(cNumber.Substring(6,2)))
                return true;
            else
                return false;
        }

        /// <summary>
        ///  Checks the check digits of a Latvian VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool LVVATCheckDigit(string cNumber)
        {
            // cNumber should be 11 digits
            if (cNumber.Length != 11)
                return false;

            // Only check the legal bodies
            Regex reg = new Regex(@"^[0-3]");
            if (reg.IsMatch(cNumber))
                return true;

            int total = 0;
            int[] multipliers = new int[10] { 9,1,4,8,3,10,2,5,7,6};

            //Extract the next digit and multiply by the counter
            for (int i = 0; i < 10; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits by getting modules 11.
            if (total % 11 == 4 && cNumber[0] == '9')
                total -= 45;
            if (total % 11 == 4)
                total = 4 - total % 11;
            else if (total % 11 > 4)
                total = 14 - total % 11;
            else if (total % 11 < 4)
                total = 3 - total % 11;


            //Compare it with the last character of the VAT number. If it is the same,
            // then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(10, 1)))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Checks the check digits of a Maltese VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool MTVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[6] { 3, 4, 6, 7, 8, 9 };

            // eXTRACT THE NEXT DIGIT AND MULTIPLY BY THE COUNTER.
            for(int i= 0;i < 6; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits by getting modules 37.
            total = 37 - total % 37;

            //Compare it with the last character of the VAT number. If it is the same,
            //then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(6, 2)) * 1)
                return true;
            else
                return false;
            
        }

        // Checks the check digits of a Dutch(Netherlands) VAT number.
        private bool NLVATCheckDigit(string cNumber)
        {
            // cNumber should be 12 digits
            if (cNumber.Length != 12)
                return false;

            int total = 0;
            int[] multipliers = new int[8] { 9, 8, 7, 6, 5, 4, 3, 2 };

            //Extract the next digit and multiply by the counter.
            for (int i = 0; i < 8; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits by getting modules 11.
            total %= 11;
            if (total > 9)
                total = 0;

            //Compare it with the last character of the VAT number. It it is the same,
            //then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(8, 1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Polish VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool PLVATCheckDigit(string cNumber)
        {
            // cNumber should be 10 digits
            if (cNumber.Length != 10)
                return false;

            int total = 0;
            int[] multipliers = new int[9] { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

            // Extract the next digit and multiply by the counter.
            for (int i = 0; i < 9; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            // Establish check digits subtracting modules 11 from 11.
            total %= 11;
            if (total > 9)
                total = 0;

            //Compare it with the last character of the VAT number. If it is the same,
            //then it's a valid check digit.
            if(total == int.Parse(cNumber.Substring(9,1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Portugese VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool PTVATCheckDigit(string cNumber)
        {
            // cNumber should be 9 digits
            if (cNumber.Length != 9)
                return false;

            int total = 0;
            int[] multipliers = new int[8] { 9, 8, 7, 6, 5, 4, 3, 2 };

            //Extract the next digit and multiply by the counter.
            for (int i = 0; i < 8; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits subtracting modules 11 from 11.
            total = 11 - total % 11;
            if (total > 9)
                total = 0;

            //Compare it with the last character of the VAT number. If it is the same, 
            //then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(8, 1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Romanian VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool ROVATCheckDigit(string cNumber)
        {
            // cNumber should be 2-10 digits
         if (cNumber.Length < 2 || cNumber.Length > 10)
                return false;

            int[] multipliers = new int[10] { 7, 5, 3, 2, 1, 7, 5, 3, 2, 1 };

            //Extract the next digit and multiply by the counter.
            int VATlen = cNumber.Length;
            for (int i = 0; i < multipliers.Length - VATlen; i++)
            {
                multipliers[i] = 0;
            }

            int arrayStart = multipliers.Length - VATlen;
            int total = 0;
            for (int i = 0; i < cNumber.Length - 1; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[arrayStart++];
            }

            //Establish check digits by getting modules 11
            total = (10 * total) % 11;
            if (total == 10)
                total = 0;

             // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            if (total == int.Parse(cNumber.Substring(cNumber.Length - 1, 1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Swedish VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool SEVATCheckDigit(string cNumber)
        {
            // cNumber should be 12 digits
            if (cNumber.Length != 12)
                return false;

            int total = 0;
            int[] multipliers = new int[9] { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int temp = 0;
            double tempD = 0;

            // Extract the next digit and multiply by the appropriate multiplier.
            for (int i = 0; i < 9; i++)
            {
                temp = int.Parse(cNumber[i].ToString()) * multipliers[i];
                tempD = temp / 10;
                if (temp > 9)
                    total += int.Parse(Math.Floor(tempD).ToString()) + temp % 10;
                else
                    total += temp;
            }

            //Establish check digits by subtracting mod 10 of total from 10.
            total = 10 - (total % 10);
            if (total == 10)
                total = 0;

            // Compare it with the 10th character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            if (total.ToString() == cNumber.Substring(9,1))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Checks the check digits of a Slovenian VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private bool SIVATCheckDigit(string cNumber)
        {
            // cNumber should be 8 digits
            if (cNumber.Length != 8)
                return false;

            int total = 0;
            int[] multipliers = new int[7] { 8, 7, 6, 5, 4, 3, 2 };

            //Extract the next digit and multiply by the counter.
            for (int i = 0; i < 7; i++)
            {
                total += int.Parse(cNumber[i].ToString()) * multipliers[i];
            }

            //Establish check digits by subtracting 97 from total until negative.
            total = 11 - total % 11;
            if (total > 9)
                total = 0;

             // Compare the number with the last character of the VAT number. If it is the 
            // same, then it's a valid check digit.
            if (total.ToString() == cNumber.Substring(7,1))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks the check digits of a Slovak VAT number.
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
         private bool SKVATCheckDigit(string cNumber)
         {
             //There is no validation rule could be find out.
             // cNumber should be 10 digits
             if (cNumber.Length != 10)
                 return false;
             else
                 return true;
         }





    }
}

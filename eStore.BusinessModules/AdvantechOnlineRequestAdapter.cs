using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.SSO.Advantech;
namespace eStore.BusinessModules
{
    public class AdvantechOnlineRequestV2Adapter
    {
        public OnlineRequestV2 convert2OnlineRequest(POCOS.PocoX.FollowUpable followUpableobj)
        {
            if (followUpableobj is POCOS.Order)
                return convert2OnlineRequest((POCOS.Order)followUpableobj);
            else if (followUpableobj is POCOS.Quotation)
                return convert2OnlineRequest((POCOS.Quotation)followUpableobj);
            else if (followUpableobj is POCOS.UserRequest)
                return convert2OnlineRequest((POCOS.UserRequest)followUpableobj);
            else
                return null;
        }
        public OnlineRequestV2 convert2OnlineRequest(POCOS.Order order)
        {
            OnlineRequestV2 generalRequest = new OnlineRequestV2();
            generalRequest.firstName = order.Cart.ShipToContact.FirstName;
            generalRequest.lastName = order.Cart.ShipToContact.LastName;
            generalRequest.company = order.Cart.ShipToContact.AttCompanyName;
            generalRequest.email = order.UserID;
            generalRequest.phone = order.Cart.ShipToContact.Mobile;
            generalRequest.zipCode = order.Cart.ShipToContact.ZipCode;
            generalRequest.address = order.Cart.ShipToContact.Address1;
            generalRequest.city = order.Cart.ShipToContact.City;
            generalRequest.state = order.Cart.ShipToContact.State;
            generalRequest.country = order.Cart.ShipToContact.Country;
            generalRequest.product = order.Cart.cartItemsX.Select(x => x.productNameX).FirstOrDefault();

            generalRequest.subject = "eStore Order " + order.OrderNo;
            generalRequest.requestType = 0;
            generalRequest.requestTypeEnum = EnumRequestType.Activity;

            generalRequest.description = !string.IsNullOrEmpty(order.CustomerComment) ? order.CustomerComment : "";
            generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound
            generalRequest.activityTypeEnum = EnumActivityType.eStoreCheckout;
            generalRequest.activitySource = 4;
            generalRequest.activitySourceEnum = EnumActivitySource.eStore;
            generalRequest.userType = 4;
            generalRequest.userTypeEnum = EnumUserType.eStore;
            return generalRequest;
        }

        public OnlineRequestV2 convert2OnlineRequest(POCOS.Quotation quotation)
        {
            OnlineRequestV2 generalRequest = new OnlineRequestV2();
            generalRequest.firstName = quotation.Cart.ShipToContact.FirstName;
            generalRequest.lastName = quotation.Cart.ShipToContact.LastName;
            generalRequest.company = quotation.Cart.ShipToContact.AttCompanyName;
            generalRequest.email = quotation.UserID;
            generalRequest.phone = quotation.Cart.ShipToContact.Mobile;
            generalRequest.zipCode = quotation.Cart.ShipToContact.ZipCode;
            generalRequest.address = quotation.Cart.ShipToContact.Address1;
            generalRequest.city = quotation.Cart.ShipToContact.City;
            generalRequest.state = quotation.Cart.ShipToContact.State;
            generalRequest.country = quotation.Cart.ShipToContact.Country;
            generalRequest.subject = "eStore quotation " + quotation.QuotationNumber;
            generalRequest.product = quotation.Cart.cartItemsX.Select(x => x.productNameX).FirstOrDefault();
            generalRequest.requestType = 0;
            generalRequest.requestTypeEnum = EnumRequestType.Activity;

            generalRequest.description = !string.IsNullOrEmpty(quotation.Comments) ? quotation.Comments : "";
            generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound
            generalRequest.activityTypeEnum = EnumActivityType.eStoreQuotes;
            generalRequest.activitySource = 4;
            generalRequest.activitySourceEnum = EnumActivitySource.eStore;
            generalRequest.userType = 4;
            generalRequest.userTypeEnum = EnumUserType.eStore;


            return generalRequest;
        }

        public OnlineRequestV2 convert2OnlineRequest(POCOS.UserRequest requestDiscount)
        {
            OnlineRequestV2 generalRequest = new OnlineRequestV2();
            generalRequest.firstName = requestDiscount.FirstName;
            generalRequest.lastName = requestDiscount.LastName;
            generalRequest.company = requestDiscount.Company;
            generalRequest.email = requestDiscount.Email;
            generalRequest.phone = requestDiscount.Telephone;
            generalRequest.zipCode = "";
            generalRequest.address = requestDiscount.Address;
            generalRequest.city = "";
            generalRequest.state = requestDiscount.State;
            generalRequest.country = requestDiscount.Country;
            generalRequest.product = requestDiscount.ProductName;

            generalRequest.requestTypeEnum = EnumRequestType.Activity;
            switch (requestDiscount.RequestType)
            {
                case "TechnicalSupport":
                    generalRequest.subject = "Technical Support";
                    generalRequest.requestTypeEnum = EnumRequestType.ServiceRequest;
                    generalRequest.activityTypeEnum = EnumActivityType.WebInboundService;
                    break;
                case "GeneralInquiries":
                    generalRequest.subject = "eStore General Inquiry";
                    generalRequest.activityTypeEnum = EnumActivityType.WebInboundService;
                    break;
                case "Sales":
                    generalRequest.subject = "eStore Sales Inquiry";
                    generalRequest.activityTypeEnum = EnumActivityType.WebInboundSales;
                    break;
                case "RequestDiscount":
                    generalRequest.subject = "Quantity Discount Request";
                    generalRequest.activityTypeEnum = EnumActivityType.WebInboundSales;
                    break;
            }
            generalRequest.productCategory = requestDiscount.ProductCategory;
            generalRequest.requestType = 0;


            generalRequest.product = requestDiscount.ProductX != null ? requestDiscount.ProductX.ModelNo : "";

            generalRequest.description = !string.IsNullOrEmpty(requestDiscount.Comment) ? requestDiscount.Comment : "";
            //generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound



            generalRequest.activitySource = 4;
            generalRequest.activitySourceEnum = EnumActivitySource.eStore;
            generalRequest.userType = 4;
            generalRequest.userTypeEnum = EnumUserType.eStore;
            return generalRequest;
        }
    }

    public class AdvantechOnlineRequestAdapter
    {
        public OnlineRequest convert2OnlineRequest(POCOS.PocoX.FollowUpable followUpableobj)
        {
            if (followUpableobj is POCOS.Order)
                return convert2OnlineRequest((POCOS.Order)followUpableobj);
            else if (followUpableobj is POCOS.Quotation)
                return convert2OnlineRequest((POCOS.Quotation)followUpableobj);
            else if (followUpableobj is POCOS.UserRequest)
                return convert2OnlineRequest((POCOS.UserRequest)followUpableobj);
            else
                return null;
        }
        public OnlineRequest convert2OnlineRequest(POCOS.Order order)
        {
            OnlineRequest generalRequest = new OnlineRequest();
            generalRequest.firstName = order.Cart.ShipToContact.FirstName;
            generalRequest.lastName = order.Cart.ShipToContact.LastName;
            generalRequest.company = order.Cart.ShipToContact.AttCompanyName;
            generalRequest.email = order.UserID;
            generalRequest.phone = order.Cart.ShipToContact.Mobile;
            generalRequest.zipCode = order.Cart.ShipToContact.ZipCode;
            generalRequest.address = order.Cart.ShipToContact.Address1;
            generalRequest.city = order.Cart.ShipToContact.City;
            generalRequest.state = order.Cart.ShipToContact.State;
            generalRequest.country = order.Cart.ShipToContact.Country;
            generalRequest.product = order.Cart.cartItemsX.Select(x => x.productNameX).FirstOrDefault();

            generalRequest.subject = "eStore Order " + order.OrderNo;
            generalRequest.requestType = 0;

            generalRequest.description = !string.IsNullOrEmpty(order.CustomerComment) ? order.CustomerComment : "";
            generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound

            generalRequest.activitySource = 4;

            generalRequest.userType = 4;

            return generalRequest;
        }

        public OnlineRequest convert2OnlineRequest(POCOS.Quotation quotation)
        {
            OnlineRequest generalRequest = new OnlineRequest();
            generalRequest.firstName = quotation.Cart.ShipToContact.FirstName;
            generalRequest.lastName = quotation.Cart.ShipToContact.LastName;
            generalRequest.company = quotation.Cart.ShipToContact.AttCompanyName;
            generalRequest.email = quotation.UserID;
            generalRequest.phone = quotation.Cart.ShipToContact.Mobile;
            generalRequest.zipCode = quotation.Cart.ShipToContact.ZipCode;
            generalRequest.address = quotation.Cart.ShipToContact.Address1;
            generalRequest.city = quotation.Cart.ShipToContact.City;
            generalRequest.state = quotation.Cart.ShipToContact.State;
            generalRequest.country = quotation.Cart.ShipToContact.Country;
            generalRequest.subject = "eStore quotation " + quotation.QuotationNumber;
            generalRequest.product = quotation.Cart.cartItemsX.Select(x => x.productNameX).FirstOrDefault();
            generalRequest.requestType = 0;


            generalRequest.description = !string.IsNullOrEmpty(quotation.Comments) ? quotation.Comments : "";
            generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound

            generalRequest.activitySource = 4;

            generalRequest.userType = 4;



            return generalRequest;
        }

        public OnlineRequest convert2OnlineRequest(POCOS.UserRequest requestDiscount)
        {
            OnlineRequest generalRequest = new OnlineRequest();
            generalRequest.firstName = requestDiscount.FirstName;
            generalRequest.lastName = requestDiscount.LastName;
            generalRequest.company = requestDiscount.Company;
            generalRequest.email = requestDiscount.Email;
            generalRequest.phone = requestDiscount.Telephone;
            generalRequest.zipCode = "";
            generalRequest.address = requestDiscount.Address;
            generalRequest.city = "";
            generalRequest.state = requestDiscount.State;
            generalRequest.country = requestDiscount.Country;
            generalRequest.product = requestDiscount.ProductName;


            switch (requestDiscount.RequestType)
            {
                case "TechnicalSupport":
                    generalRequest.subject = "Technical Support";

                    break;
                case "GeneralInquiries":
                    generalRequest.subject = "eStore General Inquiry";
                    break;
                case "Sales":
                    generalRequest.subject = "eStore Sales Inquiry";
                    break;
                case "RequestDiscount":
                    generalRequest.subject = "Quantity Discount Request";

                    break;
            }
            generalRequest.productCategory = requestDiscount.ProductCategory;
            generalRequest.requestType = 0;


            generalRequest.product = requestDiscount.ProductX != null ? requestDiscount.ProductX.ModelNo : "";

            generalRequest.description = !string.IsNullOrEmpty(requestDiscount.Comment) ? requestDiscount.Comment : "";
            //generalRequest.ownerEmail = "";
            generalRequest.activityType = 0;//6 //WebInbound



            generalRequest.activitySource = 4;

            generalRequest.userType = 4;

            return generalRequest;
        }
    }
}

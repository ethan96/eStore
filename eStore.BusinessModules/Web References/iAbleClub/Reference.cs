﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace eStore.BusinessModules.iAbleClub {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="IMartPointServiceSoap", Namespace="http://tempuri.org/")]
    public partial class IMartPointService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CheckLoginOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetUserRecordsOperationCompleted;
        
        private System.Threading.SendOrPostCallback eStoreSSOLoginAndCalPointOperationCompleted;
        
        private System.Threading.SendOrPostCallback eStorePlaceOrderOperationCompleted;
        
        private System.Threading.SendOrPostCallback eStoreRegisteredOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailForInternalOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted;
        
        private System.Threading.SendOrPostCallback SendMailForCorpEvery15thEachMonthOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public IMartPointService() {
            this.Url = global::eStore.BusinessModules.Properties.Settings.Default.eStore_BusinessModules_iAbleClub_IMartPointService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event CheckLoginCompletedEventHandler CheckLoginCompleted;
        
        /// <remarks/>
        public event GetUserRecordsCompletedEventHandler GetUserRecordsCompleted;
        
        /// <remarks/>
        public event eStoreSSOLoginAndCalPointCompletedEventHandler eStoreSSOLoginAndCalPointCompleted;
        
        /// <remarks/>
        public event eStorePlaceOrderCompletedEventHandler eStorePlaceOrderCompleted;
        
        /// <remarks/>
        public event eStoreRegisteredCompletedEventHandler eStoreRegisteredCompleted;
        
        /// <remarks/>
        public event SendMailForInternalCompletedEventHandler SendMailForInternalCompleted;
        
        /// <remarks/>
        public event SendMailForSalesAndCorpAfterSAPorderSuccessCompletedEventHandler SendMailForSalesAndCorpAfterSAPorderSuccessCompleted;
        
        /// <remarks/>
        public event SendMailForCorpEvery15thEachMonthCompletedEventHandler SendMailForCorpEvery15thEachMonthCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckLogin", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void CheckLogin() {
            this.Invoke("CheckLogin", new object[0]);
        }
        
        /// <remarks/>
        public void CheckLoginAsync() {
            this.CheckLoginAsync(null);
        }
        
        /// <remarks/>
        public void CheckLoginAsync(object userState) {
            if ((this.CheckLoginOperationCompleted == null)) {
                this.CheckLoginOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckLoginOperationCompleted);
            }
            this.InvokeAsync("CheckLogin", new object[0], this.CheckLoginOperationCompleted, userState);
        }
        
        private void OnCheckLoginOperationCompleted(object arg) {
            if ((this.CheckLoginCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckLoginCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUserRecords", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void GetUserRecords() {
            this.Invoke("GetUserRecords", new object[0]);
        }
        
        /// <remarks/>
        public void GetUserRecordsAsync() {
            this.GetUserRecordsAsync(null);
        }
        
        /// <remarks/>
        public void GetUserRecordsAsync(object userState) {
            if ((this.GetUserRecordsOperationCompleted == null)) {
                this.GetUserRecordsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetUserRecordsOperationCompleted);
            }
            this.InvokeAsync("GetUserRecords", new object[0], this.GetUserRecordsOperationCompleted, userState);
        }
        
        private void OnGetUserRecordsOperationCompleted(object arg) {
            if ((this.GetUserRecordsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetUserRecordsCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/eStoreSSOLoginAndCalPoint", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public decimal eStoreSSOLoginAndCalPoint(string userid, string key, bool testing) {
            object[] results = this.Invoke("eStoreSSOLoginAndCalPoint", new object[] {
                        userid,
                        key,
                        testing});
            return ((decimal)(results[0]));
        }
        
        /// <remarks/>
        public void eStoreSSOLoginAndCalPointAsync(string userid, string key, bool testing) {
            this.eStoreSSOLoginAndCalPointAsync(userid, key, testing, null);
        }
        
        /// <remarks/>
        public void eStoreSSOLoginAndCalPointAsync(string userid, string key, bool testing, object userState) {
            if ((this.eStoreSSOLoginAndCalPointOperationCompleted == null)) {
                this.eStoreSSOLoginAndCalPointOperationCompleted = new System.Threading.SendOrPostCallback(this.OneStoreSSOLoginAndCalPointOperationCompleted);
            }
            this.InvokeAsync("eStoreSSOLoginAndCalPoint", new object[] {
                        userid,
                        key,
                        testing}, this.eStoreSSOLoginAndCalPointOperationCompleted, userState);
        }
        
        private void OneStoreSSOLoginAndCalPointOperationCompleted(object arg) {
            if ((this.eStoreSSOLoginAndCalPointCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.eStoreSSOLoginAndCalPointCompleted(this, new eStoreSSOLoginAndCalPointCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/eStorePlaceOrder", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public decimal eStorePlaceOrder(AblePointOrder order, bool testing) {
            object[] results = this.Invoke("eStorePlaceOrder", new object[] {
                        order,
                        testing});
            return ((decimal)(results[0]));
        }
        
        /// <remarks/>
        public void eStorePlaceOrderAsync(AblePointOrder order, bool testing) {
            this.eStorePlaceOrderAsync(order, testing, null);
        }
        
        /// <remarks/>
        public void eStorePlaceOrderAsync(AblePointOrder order, bool testing, object userState) {
            if ((this.eStorePlaceOrderOperationCompleted == null)) {
                this.eStorePlaceOrderOperationCompleted = new System.Threading.SendOrPostCallback(this.OneStorePlaceOrderOperationCompleted);
            }
            this.InvokeAsync("eStorePlaceOrder", new object[] {
                        order,
                        testing}, this.eStorePlaceOrderOperationCompleted, userState);
        }
        
        private void OneStorePlaceOrderOperationCompleted(object arg) {
            if ((this.eStorePlaceOrderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.eStorePlaceOrderCompleted(this, new eStorePlaceOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/eStoreRegistered", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public decimal eStoreRegistered(string userid, decimal point) {
            object[] results = this.Invoke("eStoreRegistered", new object[] {
                        userid,
                        point});
            return ((decimal)(results[0]));
        }
        
        /// <remarks/>
        public void eStoreRegisteredAsync(string userid, decimal point) {
            this.eStoreRegisteredAsync(userid, point, null);
        }
        
        /// <remarks/>
        public void eStoreRegisteredAsync(string userid, decimal point, object userState) {
            if ((this.eStoreRegisteredOperationCompleted == null)) {
                this.eStoreRegisteredOperationCompleted = new System.Threading.SendOrPostCallback(this.OneStoreRegisteredOperationCompleted);
            }
            this.InvokeAsync("eStoreRegistered", new object[] {
                        userid,
                        point}, this.eStoreRegisteredOperationCompleted, userState);
        }
        
        private void OneStoreRegisteredOperationCompleted(object arg) {
            if ((this.eStoreRegisteredCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.eStoreRegisteredCompleted(this, new eStoreRegisteredCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailForInternal", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendMailForInternal(string appName) {
            this.Invoke("SendMailForInternal", new object[] {
                        appName});
        }
        
        /// <remarks/>
        public void SendMailForInternalAsync(string appName) {
            this.SendMailForInternalAsync(appName, null);
        }
        
        /// <remarks/>
        public void SendMailForInternalAsync(string appName, object userState) {
            if ((this.SendMailForInternalOperationCompleted == null)) {
                this.SendMailForInternalOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailForInternalOperationCompleted);
            }
            this.InvokeAsync("SendMailForInternal", new object[] {
                        appName}, this.SendMailForInternalOperationCompleted, userState);
        }
        
        private void OnSendMailForInternalOperationCompleted(object arg) {
            if ((this.SendMailForInternalCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailForInternalCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailForSalesAndCorpAfterSAPorderSuccess", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendMailForSalesAndCorpAfterSAPorderSuccess(string appName, string[] orders) {
            this.Invoke("SendMailForSalesAndCorpAfterSAPorderSuccess", new object[] {
                        appName,
                        orders});
        }
        
        /// <remarks/>
        public void SendMailForSalesAndCorpAfterSAPorderSuccessAsync(string appName, string[] orders) {
            this.SendMailForSalesAndCorpAfterSAPorderSuccessAsync(appName, orders, null);
        }
        
        /// <remarks/>
        public void SendMailForSalesAndCorpAfterSAPorderSuccessAsync(string appName, string[] orders, object userState) {
            if ((this.SendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted == null)) {
                this.SendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted);
            }
            this.InvokeAsync("SendMailForSalesAndCorpAfterSAPorderSuccess", new object[] {
                        appName,
                        orders}, this.SendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted, userState);
        }
        
        private void OnSendMailForSalesAndCorpAfterSAPorderSuccessOperationCompleted(object arg) {
            if ((this.SendMailForSalesAndCorpAfterSAPorderSuccessCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailForSalesAndCorpAfterSAPorderSuccessCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendMailForCorpEvery15thEachMonth", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendMailForCorpEvery15thEachMonth() {
            this.Invoke("SendMailForCorpEvery15thEachMonth", new object[0]);
        }
        
        /// <remarks/>
        public void SendMailForCorpEvery15thEachMonthAsync() {
            this.SendMailForCorpEvery15thEachMonthAsync(null);
        }
        
        /// <remarks/>
        public void SendMailForCorpEvery15thEachMonthAsync(object userState) {
            if ((this.SendMailForCorpEvery15thEachMonthOperationCompleted == null)) {
                this.SendMailForCorpEvery15thEachMonthOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendMailForCorpEvery15thEachMonthOperationCompleted);
            }
            this.InvokeAsync("SendMailForCorpEvery15thEachMonth", new object[0], this.SendMailForCorpEvery15thEachMonthOperationCompleted, userState);
        }
        
        private void OnSendMailForCorpEvery15thEachMonthOperationCompleted(object arg) {
            if ((this.SendMailForCorpEvery15thEachMonthCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendMailForCorpEvery15thEachMonthCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1067.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AblePointOrder {
        
        private string cartIDField;
        
        private string storeIDField;
        
        private string orderNoField;
        
        private string userIDField;
        
        private System.DateTime orderDateField;
        
        private decimal totalAmountField;
        
        private string currencyField;
        
        private AblePointCartItem[] cartItemsField;
        
        /// <remarks/>
        public string CartID {
            get {
                return this.cartIDField;
            }
            set {
                this.cartIDField = value;
            }
        }
        
        /// <remarks/>
        public string StoreID {
            get {
                return this.storeIDField;
            }
            set {
                this.storeIDField = value;
            }
        }
        
        /// <remarks/>
        public string OrderNo {
            get {
                return this.orderNoField;
            }
            set {
                this.orderNoField = value;
            }
        }
        
        /// <remarks/>
        public string UserID {
            get {
                return this.userIDField;
            }
            set {
                this.userIDField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime OrderDate {
            get {
                return this.orderDateField;
            }
            set {
                this.orderDateField = value;
            }
        }
        
        /// <remarks/>
        public decimal TotalAmount {
            get {
                return this.totalAmountField;
            }
            set {
                this.totalAmountField = value;
            }
        }
        
        /// <remarks/>
        public string Currency {
            get {
                return this.currencyField;
            }
            set {
                this.currencyField = value;
            }
        }
        
        /// <remarks/>
        public AblePointCartItem[] CartItems {
            get {
                return this.cartItemsField;
            }
            set {
                this.cartItemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1067.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class AblePointCartItem {
        
        private string cartIDField;
        
        private string productIDField;
        
        private string displayPartNoField;
        
        private decimal adjustedPriceField;
        
        private int qtyField;
        
        /// <remarks/>
        public string CartID {
            get {
                return this.cartIDField;
            }
            set {
                this.cartIDField = value;
            }
        }
        
        /// <remarks/>
        public string ProductID {
            get {
                return this.productIDField;
            }
            set {
                this.productIDField = value;
            }
        }
        
        /// <remarks/>
        public string DisplayPartNo {
            get {
                return this.displayPartNoField;
            }
            set {
                this.displayPartNoField = value;
            }
        }
        
        /// <remarks/>
        public decimal AdjustedPrice {
            get {
                return this.adjustedPriceField;
            }
            set {
                this.adjustedPriceField = value;
            }
        }
        
        /// <remarks/>
        public int Qty {
            get {
                return this.qtyField;
            }
            set {
                this.qtyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void CheckLoginCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void GetUserRecordsCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void eStoreSSOLoginAndCalPointCompletedEventHandler(object sender, eStoreSSOLoginAndCalPointCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class eStoreSSOLoginAndCalPointCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal eStoreSSOLoginAndCalPointCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public decimal Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((decimal)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void eStorePlaceOrderCompletedEventHandler(object sender, eStorePlaceOrderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class eStorePlaceOrderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal eStorePlaceOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public decimal Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((decimal)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void eStoreRegisteredCompletedEventHandler(object sender, eStoreRegisteredCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class eStoreRegisteredCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal eStoreRegisteredCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public decimal Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((decimal)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void SendMailForInternalCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void SendMailForSalesAndCorpAfterSAPorderSuccessCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void SendMailForCorpEvery15thEachMonthCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591
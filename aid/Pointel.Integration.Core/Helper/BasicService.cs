﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



namespace Pointel.Integration.Core.Helper
{
    // 
    // This source code was auto-generated by wsdl, Version=4.0.30319.1.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "BasicHttpBinding_IBasicService", Namespace = "http://tempuri.org/")]
    public partial class BasicService : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback InsertCallDetailsAsycOperationCompleted;

        private System.Threading.SendOrPostCallback UpdateCallDetailsAsycOperationCompleted;

        /// <remarks/>
        public BasicService(string url)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
            {
                this.Url = url; //"http://192.168.100.94/BasicService/BasicService.svc";
            }
            else
            {

            }

        }

        /// <remarks/>
        public event InsertCallDetailsAsycCompletedEventHandler InsertCallDetailsAsycCompleted;

        /// <remarks/>
        public event UpdateCallDetailsAsycCompletedEventHandler UpdateCallDetailsAsycCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IBasicService/InsertCallDetailsAsyc", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public ServiceResult InsertCallDetailsAsyc([System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] InsertData data)
        {
            object[] results = this.Invoke("InsertCallDetailsAsyc", new object[] {
                    data});
            return ((ServiceResult)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginInsertCallDetailsAsyc(InsertData data, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("InsertCallDetailsAsyc", new object[] {
                    data}, callback, asyncState);
        }

        /// <remarks/>
        public ServiceResult EndInsertCallDetailsAsyc(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServiceResult)(results[0]));
        }

        /// <remarks/>
        public void InsertCallDetailsAsycAsync(InsertData data)
        {
            this.InsertCallDetailsAsycAsync(data, null);
        }

        /// <remarks/>
        public void InsertCallDetailsAsycAsync(InsertData data, object userState)
        {
            if ((this.InsertCallDetailsAsycOperationCompleted == null))
            {
                this.InsertCallDetailsAsycOperationCompleted = new System.Threading.SendOrPostCallback(this.OnInsertCallDetailsAsycOperationCompleted);
            }
            this.InvokeAsync("InsertCallDetailsAsyc", new object[] {
                    data}, this.InsertCallDetailsAsycOperationCompleted, userState);
        }

        private void OnInsertCallDetailsAsycOperationCompleted(object arg)
        {
            if ((this.InsertCallDetailsAsycCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.InsertCallDetailsAsycCompleted(this, new InsertCallDetailsAsycCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IBasicService/UpdateCallDetailsAsyc", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public ServiceResult UpdateCallDetailsAsyc([System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] UpdateData data)
        {
            object[] results = this.Invoke("UpdateCallDetailsAsyc", new object[] {
                    data});
            return ((ServiceResult)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginUpdateCallDetailsAsyc(UpdateData data, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UpdateCallDetailsAsyc", new object[] {
                    data}, callback, asyncState);
        }

        /// <remarks/>
        public ServiceResult EndUpdateCallDetailsAsyc(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((ServiceResult)(results[0]));
        }

        /// <remarks/>
        public void UpdateCallDetailsAsycAsync(UpdateData data)
        {
            this.UpdateCallDetailsAsycAsync(data, null);
        }

        /// <remarks/>
        public void UpdateCallDetailsAsycAsync(UpdateData data, object userState)
        {
            if ((this.UpdateCallDetailsAsycOperationCompleted == null))
            {
                this.UpdateCallDetailsAsycOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateCallDetailsAsycOperationCompleted);
            }
            this.InvokeAsync("UpdateCallDetailsAsyc", new object[] {
                    data}, this.UpdateCallDetailsAsycOperationCompleted, userState);
        }

        private void OnUpdateCallDetailsAsycOperationCompleted(object arg)
        {
            if ((this.UpdateCallDetailsAsycCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateCallDetailsAsycCompleted(this, new UpdateCallDetailsAsycCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class InsertData
    {

        private CallDetails callDetailsField;

        private KeyTable keyTableField;

        private UserData userDataField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public CallDetails CallDetails
        {
            get
            {
                return this.callDetailsField;
            }
            set
            {
                this.callDetailsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public KeyTable KeyTable
        {
            get
            {
                return this.keyTableField;
            }
            set
            {
                this.keyTableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public UserData UserData
        {
            get
            {
                return this.userDataField;
            }
            set
            {
                this.userDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class CallDetails
    {

        private string ad1Field;

        private string ad10Field;

        private string ad2Field;

        private string ad3Field;

        private string ad4Field;

        private string ad5Field;

        private string ad6Field;

        private string ad7Field;

        private string ad8Field;

        private string ad9Field;

        private string agentAD1Field;

        private string agentAD2Field;

        private string agentPlaceField;

        private string aniField;

        private string connIDField;

        private string disposition1Field;

        private string disposition2Field;

        private string disposition3Field;

        private string dnisField;

        private string eventNameField;

        private string firstNameField;

        private string lastNameField;

        private string thisDNField;

        private string userNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad1
        {
            get
            {
                return this.ad1Field;
            }
            set
            {
                this.ad1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad10
        {
            get
            {
                return this.ad10Field;
            }
            set
            {
                this.ad10Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad2
        {
            get
            {
                return this.ad2Field;
            }
            set
            {
                this.ad2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad3
        {
            get
            {
                return this.ad3Field;
            }
            set
            {
                this.ad3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad4
        {
            get
            {
                return this.ad4Field;
            }
            set
            {
                this.ad4Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad5
        {
            get
            {
                return this.ad5Field;
            }
            set
            {
                this.ad5Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad6
        {
            get
            {
                return this.ad6Field;
            }
            set
            {
                this.ad6Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad7
        {
            get
            {
                return this.ad7Field;
            }
            set
            {
                this.ad7Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad8
        {
            get
            {
                return this.ad8Field;
            }
            set
            {
                this.ad8Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ad9
        {
            get
            {
                return this.ad9Field;
            }
            set
            {
                this.ad9Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string AgentAD1
        {
            get
            {
                return this.agentAD1Field;
            }
            set
            {
                this.agentAD1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string AgentAD2
        {
            get
            {
                return this.agentAD2Field;
            }
            set
            {
                this.agentAD2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string AgentPlace
        {
            get
            {
                return this.agentPlaceField;
            }
            set
            {
                this.agentPlaceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Ani
        {
            get
            {
                return this.aniField;
            }
            set
            {
                this.aniField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ConnID
        {
            get
            {
                return this.connIDField;
            }
            set
            {
                this.connIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Disposition1
        {
            get
            {
                return this.disposition1Field;
            }
            set
            {
                this.disposition1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Disposition2
        {
            get
            {
                return this.disposition2Field;
            }
            set
            {
                this.disposition2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Disposition3
        {
            get
            {
                return this.disposition3Field;
            }
            set
            {
                this.disposition3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Dnis
        {
            get
            {
                return this.dnisField;
            }
            set
            {
                this.dnisField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string EventName
        {
            get
            {
                return this.eventNameField;
            }
            set
            {
                this.eventNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ThisDN
        {
            get
            {
                return this.thisDNField;
            }
            set
            {
                this.thisDNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class UpdateData
    {

        private CallDetails callDetailsField;

        private KeyTable keyTableField;

        private UserData userDataField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public CallDetails CallDetails
        {
            get
            {
                return this.callDetailsField;
            }
            set
            {
                this.callDetailsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public KeyTable KeyTable
        {
            get
            {
                return this.keyTableField;
            }
            set
            {
                this.keyTableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public UserData UserData
        {
            get
            {
                return this.userDataField;
            }
            set
            {
                this.userDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class KeyTable
    {

        private string connIDField;

        private string key1Field;

        private string key10Field;

        private string key2Field;

        private string key3Field;

        private string key4Field;

        private string key5Field;

        private string key6Field;

        private string key7Field;

        private string key8Field;

        private string key9Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ConnID
        {
            get
            {
                return this.connIDField;
            }
            set
            {
                this.connIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key1
        {
            get
            {
                return this.key1Field;
            }
            set
            {
                this.key1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key10
        {
            get
            {
                return this.key10Field;
            }
            set
            {
                this.key10Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key2
        {
            get
            {
                return this.key2Field;
            }
            set
            {
                this.key2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key3
        {
            get
            {
                return this.key3Field;
            }
            set
            {
                this.key3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key4
        {
            get
            {
                return this.key4Field;
            }
            set
            {
                this.key4Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key5
        {
            get
            {
                return this.key5Field;
            }
            set
            {
                this.key5Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key6
        {
            get
            {
                return this.key6Field;
            }
            set
            {
                this.key6Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key7
        {
            get
            {
                return this.key7Field;
            }
            set
            {
                this.key7Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key8
        {
            get
            {
                return this.key8Field;
            }
            set
            {
                this.key8Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key9
        {
            get
            {
                return this.key9Field;
            }
            set
            {
                this.key9Field = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class UserData
    {

        private string calldbidField;

        private string connIDField;

        private ArrayOfKeyValueOfstringstringKeyValueOfstringstring[] userDatasField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Calldbid
        {
            get
            {
                return this.calldbidField;
            }
            set
            {
                this.calldbidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ConnID
        {
            get
            {
                return this.connIDField;
            }
            set
            {
                this.connIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("KeyValueOfstringstring", Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays", IsNullable = false)]
        public ArrayOfKeyValueOfstringstringKeyValueOfstringstring[] UserDatas
        {
            get
            {
                return this.userDatasField;
            }
            set
            {
                this.userDatasField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
    public partial class ArrayOfKeyValueOfstringstringKeyValueOfstringstring
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://schemas.datacontract.org/2004/07/IBasicHttpService")]
    public partial class ServiceResult
    {

        private string resultCodeField;

        private string resultDescriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ResultCode
        {
            get
            {
                return this.resultCodeField;
            }
            set
            {
                this.resultCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ResultDescription
        {
            get
            {
                return this.resultDescriptionField;
            }
            set
            {
                this.resultDescriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void InsertCallDetailsAsycCompletedEventHandler(object sender, InsertCallDetailsAsycCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class InsertCallDetailsAsycCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal InsertCallDetailsAsycCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ServiceResult Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ServiceResult)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void UpdateCallDetailsAsycCompletedEventHandler(object sender, UpdateCallDetailsAsycCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UpdateCallDetailsAsycCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal UpdateCallDetailsAsycCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ServiceResult Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ServiceResult)(this.results[0]));
            }
        }
    }


}

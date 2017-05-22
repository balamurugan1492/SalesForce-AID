using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Settings;

namespace Pointel.Interactions.Contact.Converters
{
    class ContactConvertor
    {
        #region private Member
        public List<Helpers.Attribute> TempOldContact
        {
            set;
            get;
        }
        #endregion

        public ObservableCollection<Helpers.Attribute> ConvertToObservableCollection(AttributesList attribute)
        {
            Settings.ContactDataContext contactContext = Settings.ContactDataContext.GetInstance();
            ObservableCollection<Pointel.Interactions.Contact.Helpers.Attribute> contactList = new ObservableCollection<Helpers.Attribute>();
            TempOldContact = new List<Helpers.Attribute>();
            foreach (string displaAttributeName in ContactDataContext.GetInstance().ContactDisplayedAttributes)
            {
                if (!contactContext.ContactValidAttribute.Keys.Contains(displaAttributeName))
                    continue;
                #region Attribute not null
                if (attribute != null)
                {
                    List<AttributesHeader> displayAttributeList = attribute.Cast<AttributesHeader>().Where(x => x.AttrName == displaAttributeName).ToList<AttributesHeader>();
                    if (displayAttributeList.Count > 0)
                    {
                        //List<string> multipleitems = ((string)ConfigContainer.Instance().GetValue("contact.multiple-value-attributes")).Split(',').ToList();
                        foreach (AttributesHeader attr in displayAttributeList)
                        {
                            //for multivalue attribute
                            if (ContactDataContext.GetInstance().ContactMultipleValueAttributes.Contains(attr.AttrName))
                            {
                                for (int j = 0; j < attr.AttributesInfoList.Count; j++)
                                {
                                    Helpers.Attribute contactDetails = new Helpers.Attribute();

                                    contactDetails.AttributeId = attr.AttributesInfoList[j].AttrId;
                                    //contactDetails.AttributeName = attr.AttrName;
                                    contactDetails.AttributeName = contactContext.ContactValidAttribute[attr.AttrName];
                                    contactDetails.AttributeValue = attr.AttributesInfoList[j].AttrValue.ToString();
                                    contactDetails.Description = attr.AttributesInfoList[j].Description;
                                    contactDetails.Type = Settings.ContactDataContext.AttributeType.Multiple;
                                    contactDetails.IsMandatory = ContactDataContext.GetInstance().ContactMandatoryAttributes.Contains(attr.AttrName);
                                    //check whether the data is primary or not.
                                    contactDetails.Isprimary = attr.AttributesInfoList.Primary.AttrId == contactDetails.AttributeId;
                                    contactList.Add(contactDetails);
                                    AddToTempList(contactDetails);

                                }
                            }
                            //For single value attribute
                            else
                            {
                                Helpers.Attribute contactDetails = new Helpers.Attribute();
                                contactDetails.AttributeId = attr.AttributesInfoList[0].AttrId;
                                //contactDetails.AttributeName = attr.AttrName;
                                contactDetails.AttributeName = contactContext.ContactValidAttribute[attr.AttrName];
                                contactDetails.AttributeValue = attr.AttributesInfoList[0].AttrValue.ToString();
                                contactDetails.Description = attr.AttributesInfoList[0].Description;
                                contactDetails.Isprimary = attr.AttributesInfoList.Primary.AttrId == contactDetails.AttributeId;
                                contactDetails.IsMandatory = ContactDataContext.GetInstance().ContactMandatoryAttributes.Contains(attr.AttrName);
                                contactList.Add(contactDetails);
                                AddToTempList(contactDetails);
                            }
                        }
                    }
                    else
                    {

                        Helpers.Attribute contactDetails = new Helpers.Attribute();
                        // contactDetails.AttributeName = displaAttributeName;
                        contactDetails.AttributeName = contactContext.ContactValidAttribute[displaAttributeName];
                        contactDetails.AttributeValue = string.Empty;
                        contactDetails.Description = string.Empty;
                        //List<string> items = ((string)ConfigContainer.Instance().GetValue("contact.multiple-value-attributes")).Split(',').ToList();
                        if (ContactDataContext.GetInstance().ContactMultipleValueAttributes.Contains(displaAttributeName))
                        {
                            contactDetails.Type = Settings.ContactDataContext.AttributeType.Multiple;
                        }
                        contactList.Add(contactDetails);
                        AddToTempList(contactDetails);
                    }
                }
                #endregion Attribute not null
                else
                {
                    Helpers.Attribute contactDetails = new Helpers.Attribute();

                    //contactDetails.AttributeName = displaAttributeName;
                    contactDetails.AttributeName = contactContext.ContactValidAttribute[displaAttributeName];
                    contactDetails.AttributeValue = string.Empty;
                    contactDetails.Description = string.Empty;
                    contactDetails.IsMandatory = ContactDataContext.GetInstance().ContactMandatoryAttributes.Contains(displaAttributeName);
                    // List<string> multipleItem = ((string)ConfigContainer.Instance().GetValue("contact.multiple-value-attributes")).Split(',').ToList();
                    if (ContactDataContext.GetInstance().ContactMultipleValueAttributes.Contains(displaAttributeName))
                    {
                        contactDetails.Type = Settings.ContactDataContext.AttributeType.Multiple;
                        contactDetails.Isprimary = false;
                    }
                    else
                        contactDetails.Isprimary = true;

                    contactList.Add(contactDetails);
                    AddToTempList(contactDetails);
                }
            }
            return contactList;
        }

        private void AddToTempList(Helpers.Attribute attr)
        {
            Helpers.Attribute contactDetails = new Helpers.Attribute();
            contactDetails.AttributeId = attr.AttributeId;
            contactDetails.AttributeName = attr.AttributeName;
            contactDetails.AttributeValue = attr.AttributeValue;
            contactDetails.Description = attr.Description;
            contactDetails.Isprimary = attr.Isprimary;
            contactDetails.IsMandatory = attr.IsMandatory;
            contactDetails.IsAltered = false;
            TempOldContact.Add(contactDetails);

        }

        public AttributesList GetInsertedAttributeList(ObservableCollection<Helpers.Attribute> contactList)
        {

            AttributesList attributeList = new AttributesList();
            Settings.ContactDataContext contactContext = Settings.ContactDataContext.GetInstance();
            foreach (Helpers.Attribute contact in contactList.Where(x => x.IsAltered == true && string.IsNullOrEmpty(x.AttributeId) && !string.IsNullOrEmpty(x.AttributeValue.Trim())))
            {
                //if (!attributeList.Contains(contact.AttributeName))
                string attributeName = contactContext.ContactValidAttribute.Where(x => x.Value.Equals(contact.AttributeName)).Single().Key;
                if (attributeList.Cast<AttributesHeader>().Where(y => y.AttrName.Equals(attributeName)).ToList().Count == 0)
                {
                    AttributesHeader attributeHeader = new AttributesHeader();
                    //attributeHeader.AttrName = contact.AttributeName;
                    attributeHeader.AttrName = attributeName;
                    AttributesInfoList attributeInfoList = new AttributesInfoList();
                    int i = 1;
                    foreach (Helpers.Attribute contactGroup in contactList.Where(x => x.AttributeName.Equals(contact.AttributeName) && x.IsAltered == true && string.IsNullOrEmpty(x.AttributeId) && !string.IsNullOrEmpty(x.AttributeValue.Trim())))//to reduce unwated check again
                    {
                        AttributesInfo attributeInfo = new AttributesInfo();
                        attributeInfo.AttrValue = contactGroup.AttributeValue;
                        attributeInfo.Description = contactGroup.Description;
                        if (contactGroup.Type == Settings.ContactDataContext.AttributeType.Single)
                        {
                            attributeInfo.AttrIndex = 0;
                        }
                        else
                        {
                            if (contactGroup.Isprimary)
                                attributeInfo.AttrIndex = 0;
                            else
                                attributeInfo.AttrIndex = i++;
                        }
                        attributeInfoList.Add(attributeInfo);
                    }

                    attributeHeader.AttributesInfoList = attributeInfoList;
                    attributeList.Add(attributeHeader);
                }
            }
            if (attributeList.Count == 0)
                attributeList = null;
            return attributeList;

        }

        public AttributesList GetUpdatedAttributeList(ObservableCollection<Helpers.Attribute> contactList)
        {
            AttributesList attributeList = new AttributesList();
            Settings.ContactDataContext contactContext = Settings.ContactDataContext.GetInstance();
            foreach (Helpers.Attribute contact in contactList.Where(x => x.IsAltered == true && !string.IsNullOrEmpty(x.AttributeId) && ((!string.IsNullOrEmpty(x.AttributeValue.Trim()) && x.Type == Settings.ContactDataContext.AttributeType.Multiple) || (x.Type == Settings.ContactDataContext.AttributeType.Single))))
            {
                string attributeName = contactContext.ContactValidAttribute.Where(x => x.Value.Equals(contact.AttributeName)).Single().Key;
                if (attributeList.Cast<AttributesHeader>().Where(y => y.AttrName.Equals(attributeName)).ToList().Count == 0)
                // if(!attributeList.Contains(contact.AttributeName))
                {
                    AttributesHeader attributeHeader = new AttributesHeader();
                    //  attributeHeader.AttrName = contact.AttributeName;
                    attributeHeader.AttrName = attributeName;
                    AttributesInfoList attributeInfoList = new AttributesInfoList();
                    List<Pointel.Interactions.Contact.Helpers.Attribute> lst = contactList.Where(x => x.IsAltered == true && !string.IsNullOrEmpty(x.AttributeId) && x.AttributeName.Equals(contact.AttributeName) && !string.IsNullOrEmpty(x.AttributeValue.Trim())).ToList<Pointel.Interactions.Contact.Helpers.Attribute>();
                    int i = 1;
                    foreach (Helpers.Attribute contactGroup in lst)
                    {
                        AttributesInfo attributeInfo = new AttributesInfo();
                        attributeInfo.AttrValue = contactGroup.AttributeValue;
                        attributeInfo.AttrId = contactGroup.AttributeId;
                        attributeInfo.Description = contactGroup.Description;
                        if (contactGroup.Type == Settings.ContactDataContext.AttributeType.Single)
                        {
                            attributeInfo.AttrIndex = 0;
                        }
                        else
                        {
                            if (contactGroup.Isprimary)
                                attributeInfo.AttrIndex = 0;
                            else
                                attributeInfo.AttrIndex = i++;
                        }
                        attributeInfoList.Add(attributeInfo);
                    }
                    attributeHeader.AttributesInfoList = attributeInfoList;
                    attributeList.Add(attributeHeader);
                }
            }
            if (attributeList.Count == 0)
                attributeList = null;
            return attributeList;
        }

        public DeleteAttributesList GetDeletedAttributeList(Dictionary<string, string> deletedAttribute)
        {
            Settings.ContactDataContext contactContext = Settings.ContactDataContext.GetInstance();
            if (deletedAttribute.Count > 0)
            {
                DeleteAttributesList attributeList = new DeleteAttributesList();
                foreach (var x in deletedAttribute)
                {
                    DeleteAttributesInfo attributeInfoList = new DeleteAttributesInfo();
                    attributeInfoList.AttrName = contactContext.ContactValidAttribute.Where(y => y.Value.Equals(x.Value)).Single().Key;
                    StringList lst = new StringList();
                    lst.Add(x.Key);
                    attributeInfoList.AttrId = lst;
                    attributeList.Add(attributeInfoList);
                }
                return attributeList;
            }

            return null;
        }

        public bool checkMandatoryfields(ObservableCollection<Helpers.Attribute> ContactList, Dictionary<string, string> deletedAttribute, ref string ErrorMessage)
        {
            bool status = true;
            Settings.ContactDataContext contactContext = Settings.ContactDataContext.GetInstance();
            // ErrorMessage = "";//"Can not save this contact. Mandatory fields missing";
            //List<string> items = ((string)ConfigContainer.Instance().GetValue("contact.mandatory-attributes")).Split(',').ToList();
            foreach (string mandatoryList in ContactDataContext.GetInstance().ContactMandatoryAttributes)
            {
                string attributeName = contactContext.ContactValidAttribute[mandatoryList];
                // && !deletedAttribute.ContainsKey(x.AttributeId)
                if (ContactList.Where(x => attributeName.Equals(x.AttributeName) && !string.IsNullOrEmpty(x.AttributeValue.Trim()) && (string.IsNullOrEmpty(x.AttributeId) ? true : !deletedAttribute.ContainsKey(x.AttributeId))).ToList().Count == 0)
                {
                    status = false;
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage += ", " + attributeName;
                    }
                    else
                    {
                        ErrorMessage = attributeName;
                    }

                }
            }
            if (!status)
            {
                if (ErrorMessage.IndexOf(',') >= 0)
                    ErrorMessage = "Can't save this contact. Mandatory fields are missing(" + ErrorMessage + ")";
                else
                    ErrorMessage = "Can't save this contact. Mandatory field is missing(" + ErrorMessage + ")";
            }

            return status;

        }
    }
}

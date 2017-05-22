/*
* =======================================
* Pointel.Interactions.Email.UserControls.DataUserControl
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

using Genesyslab.Platform.Commons.Collections;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Core;
using Pointel.Interactions.DispositionCodes.UserControls;
using Pointel.Interactions.DispositionCodes.Utilities;
using Pointel.Interactions.Email.DataContext;
using Pointel.Interactions.Email.Helper;
using Pointel.Interactions.IPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Pointel.Interactions.Email.UserControls
{
    /// <summary>
    /// Interaction logic for DataUserControl.xaml
    /// </summary>
    public partial class DataUserControl : UserControl
    {
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        public KeyValueCollection CurrentData = null;
        private Disposition _dispositionUC;
        KeyValuePair<string, object> DispositionObjCollection;
        private Dictionary<string, Disposition> _dispositionUserControls = new Dictionary<string, Disposition>();

        private EmailDetails emailDetails = new EmailDetails();

        private string editingKey = null;
        private string editingValue = null;

        public string notes = string.Empty;

        private string interactionId = string.Empty;

        private ContextMenu _caseDataAdd = new ContextMenu();

        private bool isFirstTimeCall = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataUserControl"/> class.
        /// </summary>
        /// <param name="kvpUserData">The KVP user data.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="interactionId">The interaction identifier.</param>
        public DataUserControl(KeyValueCollection kvpUserData, string notes, string interactionId)
        {
            this.CurrentData = kvpUserData;
            this.notes = notes;
            this.interactionId = interactionId;
            InitializeComponent();
        }

        ~DataUserControl()
        {
            CurrentData = null;
            _dispositionUC = null;
            _dispositionUserControls.Clear();
            emailDetails = null;
            editingKey = null;
            editingValue = null;
            notes = string.Empty;
            interactionId = string.Empty;
            _caseDataAdd = null;
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = emailDetails;
            ConvertUserData();

            txtNotes.Text = notes;
            if (!(ConfigContainer.Instance().AllKeys.Contains("email.enable.add-case-data")
                && ((string)ConfigContainer.Instance().GetValue("email.enable.add-case-data")).ToLower().Equals("true"))
                && _caseDataAdd != null && _caseDataAdd.Items.Count > 0)
                btnAddCaseData.Visibility = Visibility.Collapsed;
            LoadDispostionCodes(interactionId);

        }

        /// <summary>
        /// Loads the dispostion codes.
        /// </summary>
        /// <param name="interactionID">The interaction identifier.</param>
        public void LoadDispostionCodes(string interactionID)
        {
            try
            {
                if (ConfigContainer.Instance().AllKeys.Contains("email.disposition.codes") && ConfigContainer.Instance().GetValue("email.disposition.codes") != null)
                //if (EmailDataContext.GetInstance().DispositonCodes != null && EmailDataContext.GetInstance().LoadSubDispositionCodes != null)
                {
                    Dictionary<string, object> dicCMEObjects = new Dictionary<string, object>();
                    Dictionary<string, string> tempDisposition = new Dictionary<string, string>();
                    Dictionary<string, Dictionary<string, string>> subDisposition = new Dictionary<string, Dictionary<string, string>>();
                    dicCMEObjects.Clear();
                    if (ConfigContainer.Instance().AllKeys.Contains("email.subdisposition.codes") &&
                                    ConfigContainer.Instance().GetValue("email.subdisposition.codes") != null)
                        subDisposition = (Dictionary<string, Dictionary<string, string>>)ConfigContainer.Instance().GetValue("email.subdisposition.codes");
                    tempDisposition = (Dictionary<string, string>)ConfigContainer.Instance().GetValue("email.disposition.codes");
                    dicCMEObjects.Add("email.disposition.codes", tempDisposition);
                    dicCMEObjects.Add("email.subdisposition.codes", subDisposition);
                    dicCMEObjects.Add("enable.multidisposition.enabled",
                        (ConfigContainer.Instance().AllKeys.Contains("interaction.enable.multi-dispositioncode") &&
                        ((string)ConfigContainer.Instance().GetValue("interaction.enable.multi-dispositioncode")).ToLower().Equals("true")) ? true : false);
                    dicCMEObjects.Add("DispositionCodeKey", (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty));
                    dicCMEObjects.Add("DispositionName", (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-object-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-object-name") : string.Empty));
                    stpDispCodelist.Children.Clear();
                    if (tempDisposition.Count <= 0)
                    {
                        tabitemDisposition.Visibility = Visibility.Collapsed;
                        return;
                    }

                    Pointel.Interactions.DispositionCodes.InteractionHandler.Listener _dispositionCodeListener = new Pointel.Interactions.DispositionCodes.InteractionHandler.Listener();
                    _dispositionCodeListener.NotifyCMEObjects(dicCMEObjects);
                    DispositionData disData = new DispositionData() { InteractionID = interactionID };
                    _dispositionUC = _dispositionCodeListener.CreateUserControl();
                    DispositionObjCollection = new KeyValuePair<string, object>("DispositionObj", _dispositionUC);
                    stpDispCodelist.Children.Clear();
                    _dispositionUC.NotifyDispositionCodeEvent += new Pointel.Interactions.DispositionCodes.UserControls.Disposition.NotifyDispositionCode(NotifyDispositionCodeEvent);

                    _dispositionUC.Dispositions(Pointel.Interactions.IPlugins.MediaTypes.Email, disData);

                    if (_dispositionUC != null)
                    {
                        if (!_dispositionUserControls.ContainsKey(interactionID))
                            _dispositionUserControls.Add(interactionID, _dispositionUC);
                        else
                            _dispositionUserControls[interactionID] = _dispositionUC;
                        stpDispCodelist.Children.Add(_dispositionUC);

                        //Reload  selected disposition code
                        if (CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)) || CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)))
                        {
                            Dictionary<string, string> dispositionTree = new Dictionary<string, string>();
                            //if (DispositionObjCollection.Value != null)
                            //{
                            //    var dispositionObject = (Pointel.Interactions.DispositionCodes.UserControls.Disposition)
                            //                    DispositionObjCollection.Value;
                            if (CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)))
                            {
                                if (!string.IsNullOrEmpty(CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)].ToString()))
                                {

                                    dispositionTree = CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)].ToString().Split(';').Select(s => s.Split(':')).ToDictionary(a => a[0].Trim().ToString(), a => a[1].Trim().ToString());
                                }

                            }
                            if (CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)))
                            {
                                if (!string.IsNullOrEmpty(CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString()))
                                {
                                    dispositionTree.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty), CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString());
                                }

                            }
                            if (dispositionTree.Count > 0)
                                _dispositionUC.ReLoadDispositionCodes(dispositionTree, interactionId);

                            //}
                        }

                    }


                }
                isFirstTimeCall = false;
            }
            catch (Exception ex)
            {
                logger.Error("LoadDispostionCodes(): " + ex.Message);
            }
        }

        /// <summary>
        /// Notifies the disposition code event.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="data">The data.</param>
        public void NotifyDispositionCodeEvent(MediaTypes mediaType, DispositionData data)
        {
            try
            {
                if (mediaType == MediaTypes.Email)
                {
                    if (CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)))
                    {
                        if (isFirstTimeCall)
                        {
                            isFirstTimeCall = false;
                            Dictionary<string, string> dispositionCode = new Dictionary<string, string>();
                            dispositionCode.Add(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"),
                                        CurrentData[(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"))]);
                            _dispositionUC.ReLoadDispositionCodes(dispositionCode, interactionId);
                        }
                        else
                        {
                            string originalValue = CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)].ToString();
                            if (data.DispostionCode != originalValue)
                            {
                                KeyValueCollection caseData = new KeyValueCollection();
                                caseData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty), data.DispostionCode);
                                InteractionService service = new InteractionService();
                                Pointel.Interactions.Core.Common.OutputValues result = service.UpdateCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                                if (result.MessageCode == "200")
                                {
                                    CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty)] = data.DispostionCode;
                                }
                                caseData.Clear();
                            }
                        }
                    }
                    else
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        caseData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty), data.DispostionCode);
                        InteractionService service = new InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues result = service.AddCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                        if (result.MessageCode == "200")
                        {
                            CurrentData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition.key-name") : string.Empty), data.DispostionCode);
                        }
                        caseData.Clear();
                    }

                    if (CurrentData.ContainsKey((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)))
                    {
                        string originalValue = CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)].ToString();
                        if (data.DispostionCode != originalValue)
                        {
                            string result = string.Join("; ", data.DispostionCollection.Select(x => string.Format("{0}:{1}", x.Key, x.Value)).ToArray());
                            KeyValueCollection caseData = new KeyValueCollection();
                            caseData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty), result);
                            InteractionService service = new InteractionService();
                            Pointel.Interactions.Core.Common.OutputValues output = service.UpdateCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                            if (output.MessageCode == "200")
                            {
                                CurrentData[(ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty)] = result;
                            }
                            caseData.Clear();
                        }
                    }
                    else
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        string result = string.Join("; ", data.DispostionCollection.Select(x => string.Format("{0}:{1}", x.Key, x.Value)).ToArray());
                        caseData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty), result);
                        InteractionService service = new InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues output = service.AddCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                        if (output.MessageCode == "200")
                        {
                            CurrentData.Add((ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-collection.key-name") ? (string)ConfigContainer.Instance().GetValue("interaction.disposition-collection.key-name") : string.Empty), result);
                        }
                        caseData.Clear();
                    }
                    ConvertUserData();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred as " + generalException.ToString());
            }
        }

        /// <summary>
        /// Converts the user data.
        /// </summary>
        private void ConvertUserData()
        {
            try
            {
                emailDetails.EmailCaseData.Clear();
                List<string> CaseDataFilter = null;
                //Load User case data.
                if (CurrentData != null)
                {
                    //Case data with filtering  
                    if (ConfigContainer.Instance().AllKeys.Contains("email.enable.case-data-filter")
                        && ((string)ConfigContainer.Instance().GetValue("email.enable.case-data-filter")).ToLower().Equals("true")
                         && ((ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataFilterKey")
                            && ConfigContainer.Instance().GetValue("EmailAttachDataFilterKey") != null)
                            || (ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataKey")
                            && ((List<string>)ConfigContainer.Instance().GetValue("EmailAttachDataKey")) != null)))
                    {
                        CaseDataFilter = ((ConfigContainer.Instance().GetValue("EmailAttachDataFilterKey")) as List<string>).ToList();
                        if (CaseDataFilter != null)
                            CaseDataFilter.AddRange((List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey")));
                        else
                            CaseDataFilter = (List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey"));
                        if (CaseDataFilter != null)
                            CaseDataFilter = CaseDataFilter.Distinct().ToList();
                    }
                    else
                    {
                        CaseDataFilter = CurrentData.AllKeys.ToList();
                    }
                    if (CaseDataFilter != null && CaseDataFilter.Count > 0)
                    {
                        foreach (string keyName in CaseDataFilter)
                        {
                            if (CurrentData.ContainsKey(keyName))
                                emailDetails.EmailCaseData.Add(new EmailCaseData() { Key = keyName, Value = CurrentData[keyName].ToString() });
                        }
                    }
                    _caseDataAdd.Items.Clear();
                    if (ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataKey") && ConfigContainer.Instance().GetValue("EmailAttachDataKey") != null)
                    {
                        foreach (string key in (List<string>)ConfigContainer.Instance().GetValue("EmailAttachDataKey"))
                        {
                            if (emailDetails.EmailCaseData.Where(x => x.Key.Equals(key)).ToList().Count == 0)
                            {
                                MenuItem _mItem = new MenuItem();
                                _mItem.Header = key;
                                _mItem.Click += new RoutedEventHandler(CaseDataAddMenuitem_Click);
                                _caseDataAdd.Style = (Style)FindResource("Contextmenu");
                                _caseDataAdd.Items.Add(_mItem);
                            }
                        }
                    }
                    if (_caseDataAdd.Items.Count == 0)
                        btnAddCaseData.Visibility = Visibility.Collapsed;
                    else if (emailDetails.EmailCaseData.Count == 0)
                        btnAddCaseData.Visibility = Visibility.Visible;
                    //if (emailDetails.EmailCaseData.Count == 0)
                    //    btnAddCaseData.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the CaseDataAddMenuitem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void CaseDataAddMenuitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                //DGCaseDataInfo.UpdateLayout();
                emailDetails.EmailCaseData.Add(new EmailCaseData() { Key = menuitem.Header.ToString(), Value = string.Empty });
                if (!CurrentData.ContainsKey(menuitem.Header.ToString()))
                {
                    CurrentData.Add(menuitem.Header.ToString(), "");
                }
                DGCaseDataInfo.UpdateLayout();
                //BindGrid();
                if (DGCaseDataInfo.Items.Count > 2)
                    DGCaseDataInfo.ScrollIntoView(DGCaseDataInfo.Items[DGCaseDataInfo.Items.Count - 2]);
                int rowIndex = emailDetails.EmailCaseData.IndexOf(emailDetails.EmailCaseData.Where(x => x.Key.Equals(menuitem.Header.ToString())).SingleOrDefault());
                var dataGridCellInfo = new Microsoft.Windows.Controls.DataGridCellInfo(DGCaseDataInfo.Items[rowIndex], DGCaseDataInfo.Columns[1]);
                var cell = TryToFindGridCell(DGCaseDataInfo, dataGridCellInfo);
                if (cell == null) return;
                cell.Focus();
                DGCaseDataInfo.BeginEdit();
                _caseDataAdd.Items.Remove(menuitem);
                editingKey = menuitem.Header.ToString();
                editingValue = string.Empty;

                if (_caseDataAdd.Items.Count == 0)
                {
                    // Addded by sakthi to hide, if there is no item available to add after added all item. 
                    // Date : 20-02-2016

                    btnAddCaseData.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception commonException)
            {
                logger.Error("Error occurred as : " + commonException.Message.ToString());
            }
        }

        /// <summary>
        /// Tries to find grid cell.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="cellInfo">The cell information.</param>
        /// <returns>Microsoft.Windows.Controls.DataGridCell.</returns>
        static Microsoft.Windows.Controls.DataGridCell TryToFindGridCell(Microsoft.Windows.Controls.DataGrid grid, Microsoft.Windows.Controls.DataGridCellInfo cellInfo)
        {
            Microsoft.Windows.Controls.DataGridCell result = null;
            Microsoft.Windows.Controls.DataGridRow row = null;
            grid.ScrollIntoView(cellInfo.Item);
            grid.UpdateLayout();
            row = (Microsoft.Windows.Controls.DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter presenter = GetVisualChild<Microsoft.Windows.Controls.Primitives.DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as Microsoft.Windows.Controls.DataGridCell;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the visual child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns>T.</returns>
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            var vw = (Visual)VisualTreeHelper.GetChild(parent, 0);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }


        /// <summary>
        /// Handles the Click event of the btnAddCaseData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnAddCaseData_Click(object sender, RoutedEventArgs e)
        {
            if (_caseDataAdd.Items.Count > 0)
            {
                _caseDataAdd.PlacementTarget = btnAddCaseData;
                _caseDataAdd.IsOpen = true;
            }

        }

        /// <summary>
        /// Handles the Click event of the btnSaveNote control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSaveNote_Click(object sender, RoutedEventArgs e)
        {
            notes = txtNotes.Text;
            ContactServerHelper.UpdateInteraction(this.interactionId, ConfigContainer.Instance().PersonDbId, notes, null, 2, null);
        }

        /// <summary>
        /// Handles the TextChanged event of the txtNotes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNotes.Text.Equals(notes))
                btnSaveNote.Visibility = Visibility.Collapsed;
            else
                btnSaveNote.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Handles the Click event of the btnUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isEditDone = true;
                DGCaseDataInfo.CancelEdit();
                var selectedCallData = DGCaseDataInfo.SelectedItem as EmailCaseData;
                string key = selectedCallData.Key;
                string value = selectedCallData.Value;
                if (CurrentData.ContainsKey(key))
                {
                    string originalValue = CurrentData[key].ToString();
                    if (value != originalValue)
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        caseData.Add(key, value);
                        InteractionService interactionService = new InteractionService();
                        Pointel.Interactions.Core.Common.OutputValues result = interactionService.UpdateCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                        if (result.MessageCode == "200")
                        {
                            CurrentData[key] = value;
                        }
                        caseData.Clear();
                    }
                }
                else
                {
                    KeyValueCollection caseData = new KeyValueCollection();
                    caseData.Add(key, value);
                    InteractionService interactionService = new InteractionService();
                    Pointel.Interactions.Core.Common.OutputValues result = interactionService.AddCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                    if (result.MessageCode == "200")
                    {
                        CurrentData.Add(key, value);
                    }
                    caseData.Clear();
                }
                editingValue = editingKey = null;

                //  BindGrid();

            }
            catch (Exception commonException)
            {
                logger.Error("Error ocurred as " + commonException.Message);

            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isEditDone = true;
                DGCaseDataInfo.CancelEdit();
                var selectedCallData = DGCaseDataInfo.SelectedItem as EmailCaseData;
                if (CurrentData.ContainsKey(editingKey))
                {
                    CurrentData[editingKey] = editingValue;
                    selectedCallData.Value = editingValue;
                    editingValue = editingKey = null;
                    DGCaseDataInfo.UpdateLayout();
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }

        }

        /// <summary>
        /// Handles the BeginningEdit event of the DGCaseDataInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridBeginningEditEventArgs"/> instance containing the event data.</param>
        private void DGCaseDataInfo_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            var selectedCallData = DGCaseDataInfo.SelectedItem as EmailCaseData;
            if (selectedCallData != null)
            {
                List<string> emailCaseData = new List<string>();
                if (ConfigContainer.Instance().AllKeys.Contains("EmailAttachDataKey") && ConfigContainer.Instance().GetValue("EmailAttachDataKey") != null)
                    emailCaseData = (List<string>)(ConfigContainer.Instance().GetValue("EmailAttachDataKey"));

                if (ConfigContainer.Instance().AllKeys.Contains("email.enable.modify-case-data") && ((string)ConfigContainer.Instance().GetValue("email.enable.modify-case-data")).ToLower().Equals("true") && emailCaseData != null && emailCaseData.Contains(selectedCallData.Key))
                {
                    editingKey = selectedCallData.Key;
                    editingValue = selectedCallData.Value;
                    e.Cancel = false;
                    return;
                }
            }
            e.Cancel = true;
        }

        bool isEditDone = false;

        /// <summary>
        /// Handles the PreparingCellForEdit event of the DGCaseDataInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs"/> instance containing the event data.</param>
        private void DGCaseDataInfo_PreparingCellForEdit(object sender, Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs e)
        {
            Microsoft.Windows.Controls.DataGridRow dgRow = e.Row;
            dgRow.IsSelected = true;
            var contentPresenter = e.EditingElement as ContentPresenter;
            var editingTemplate = contentPresenter.ContentTemplate;
            var textBox = (editingTemplate as DataTemplate).FindName("txtValue", (contentPresenter as ContentPresenter));
            if (textBox == null) return;

            if (!(textBox is TextBox)) return;
            (textBox as TextBox).Focus();
            isEditDone = false;
        }



        /// <summary>
        /// Handles the RowEditEnding event of the DGCaseDataInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs"/> instance containing the event data.</param>
        private void DGCaseDataInfo_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            try
            {
                Microsoft.Windows.Controls.DataGridRow dgRow = e.Row;
                if (dgRow != null)
                {
                    var selectedCallData = dgRow.Item as EmailCaseData;
                    string key = selectedCallData.Key.ToString().Trim();
                    string value = selectedCallData.Value.ToString().Trim();
                    if (!isEditDone)
                    {
                        isEditDone = true;
                        if (CurrentData.ContainsKey(key))
                        {
                            string originalValue = CurrentData[key].ToString();
                            if (value != originalValue)
                            {
                                KeyValueCollection caseData = new KeyValueCollection();
                                caseData.Add(key, value);
                                InteractionService interactionService = new InteractionService();
                                Pointel.Interactions.Core.Common.OutputValues result = interactionService.UpdateCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                                if (result.MessageCode == "200")
                                {
                                    CurrentData[key] = value;
                                }
                                caseData.Clear();
                            }
                        }
                        else
                        {
                            KeyValueCollection caseData = new KeyValueCollection();
                            caseData.Add(key, value);
                            InteractionService interactionService = new InteractionService();
                            Pointel.Interactions.Core.Common.OutputValues result = interactionService.AddCaseDataProperties(interactionId, EmailDataContext.GetInstance().ProxyClientID, caseData);
                            if (result.MessageCode == "200")
                            {
                                CurrentData.Add(key, value);
                            }
                            caseData.Clear();
                        }
                        return;
                    }
                    else
                    {
                        if (CurrentData.ContainsKey(key))
                        {
                            string originalValue = CurrentData[key].ToString();
                            if (value != originalValue)
                            {
                                CurrentData.Remove(key);
                                CurrentData.Add(key, value);
                            }
                        }
                        else
                        {
                            CurrentData.Add(key, value);
                        }
                    }
                    ConvertUserData();

                }
            }
            catch (Exception ex)
            {
                logger.Error("dgCaseInfo_RowEditEnding: " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the txtValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    btnUpdate_Click(null, null);
                }
                else if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    btnCancel_Click(null, null);
                }


            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the DGCaseDataInfo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void DGCaseDataInfo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.Escape && editingKey != null)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred as " + ex.Message);
            }
        }

        private void tabDataUC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = sender as TabControl;
                var selected = item.SelectedItem as TabItem;
                if (selected.Header.ToString() == "_Dispositions" && _dispositionUC != null)
                {
                    Dictionary<string, string> dispositionCode = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(CurrentData[(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"))]))
                        dispositionCode.Add(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"),
                                    CurrentData[(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"))]);
                    else
                        dispositionCode.Add(ConfigContainer.Instance().GetValue("interaction.disposition.key-name"),
                                                       "None");
                    _dispositionUC.ReLoadDispositionCodes(dispositionCode, interactionId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred at tabDataUC_SelectionChanged" + ex.Message);
            }
        }
    }
}

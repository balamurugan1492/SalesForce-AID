namespace Pointel.Interactions.Chat.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Genesyslab.Platform.Commons.Collections;

    using Pointel.Configuration.Manager;
    using Pointel.Interactions.Chat.Core;
    using Pointel.Interactions.Chat.Core.General;
    using Pointel.Interactions.Chat.Helpers;
    using Pointel.Interactions.Chat.Settings;
    using Pointel.Interactions.DispositionCodes.UserControls;
    using Pointel.Interactions.DispositionCodes.Utilities;
    using Pointel.Interactions.IPlugins;
    using Pointel.Salesforce.Plugin;

    /// <summary>
    /// Interaction logic for InteractionData.xaml
    /// </summary>
    public partial class InteractionData : UserControl
    {
        #region Fields

        private bool isFirstTimeCall = true;
        private ContextMenu _caseDataAdd = new ContextMenu();
        private ChatDataContext _chatDataContext = ChatDataContext.GetInstance();
        ChatUtil _chatUtil = null;
        private string _disPositionKeyName = string.Empty;
        private Disposition _dispositionUC;
        private Dictionary<string, Disposition> _dispositionUserControls = new Dictionary<string, Disposition>();
        private string _interactionID;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        #endregion Fields

        #region Constructors

        public InteractionData(ChatUtil chatUtil, string interactionID)
        {
            try
            {
                InitializeComponent();
                _interactionID = interactionID;
                _chatUtil = chatUtil;
                this.DataContext = _chatUtil;
                _chatUtil.CaseDataUpdated += new ChatUtil.UpdatedCaseData(_chatUtil_CaseDataUpdated);
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in InteractionData() : " + generalException.ToString());
            }
        }

        //private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        ~InteractionData()
        {
            try
            {
                _disPositionKeyName = null;
                _dispositionUC = null;
                if (_dispositionUserControls != null && _dispositionUserControls.Count > 0)
                    _dispositionUserControls.Clear();
                _dispositionUserControls = null;
                _interactionID = null;
                if (_caseDataAdd != null && _caseDataAdd.Items.Count > 0)
                    _caseDataAdd.Items.Clear();
                _caseDataAdd = null;
                _chatUtil.CaseDataUpdated -= _chatUtil_CaseDataUpdated;
                _chatUtil = null;
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in ~InteractionData() :" + generalException.Message);
            }
        }

        #endregion Constructors

        #region Methods

        public static T GetVisualChild<T>(Visual parent)
            where T : Visual
        {
            T child = default(T);
            try
            {
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
            }
            catch (Exception generalException)
            {
                string error = generalException.Message;
                child = null;
            }
            return child;
        }

        public void BindGrid()
        {
            try
            {
                if (_chatUtil.UserData.Count != 0)
                {
                    _chatUtil.NotifyCaseData.Clear();
                    foreach (string key in _chatUtil.UserData.Keys)
                    {
                        if (_chatUtil.NotifyCaseData.Count(p => p.Key == key) == 0)
                        {
                            if (ConfigContainer.Instance().AllKeys.Contains("chat.enable.case-data-filter") && (((string)ConfigContainer.Instance().GetValue("chat.enable.case-data-filter")).ToLower().Equals("true")))
                            {
                                if ((_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataFilterKeys.Contains(key)) || (_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataKeys.Contains(key)))
                                    _chatUtil.NotifyCaseData.Add(new ChatCaseData(key, _chatUtil.UserData[key].ToString()));
                            }
                            else
                            {
                                _chatUtil.NotifyCaseData.Add(new ChatCaseData(key, _chatUtil.UserData[key].ToString()));
                            }
                        }
                    }
                    if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
                        _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DisPositionKeyName, _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString()));
                    if (_chatUtil.UserData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                        _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DispositionCollectionKeyName, _chatUtil.UserData[_chatDataContext.DispositionCollectionKeyName].ToString()));
                    _chatUtil.NotifyCaseData = new ObservableCollection<IChatCaseData>(_chatUtil.NotifyCaseData.OrderBy(callData => callData.Key));
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error Occurred as BindGrid() :" + generalException.Message);
            }
        }

        public void LoadDispostionCodes(string interactionID)
        {
            try
            {
                if (_chatDataContext.LoadDispositionCodes != null && _chatDataContext.LoadSubDispositionCodes != null)
                {
                    _chatDataContext.dicCMEObjects.Clear();
                    _chatDataContext.dicCMEObjects.Add("chat.disposition.codes", _chatDataContext.LoadDispositionCodes);
                    _chatDataContext.dicCMEObjects.Add("chat.subdisposition.codes", _chatDataContext.LoadSubDispositionCodes);
                    if (ConfigContainer.Instance().AllKeys.Contains("interaction.enable.multi-dispositioncode") && (((string)ConfigContainer.Instance().GetValue("interaction.enable.multi-dispositioncode")).ToLower().Equals("true")))
                        _chatDataContext.dicCMEObjects.Add("enable.multidisposition.enabled", true);
                    else
                        _chatDataContext.dicCMEObjects.Add("enable.multidisposition.enabled", false);
                    _chatDataContext.dicCMEObjects.Add("DispositionCodeKey", _chatDataContext.DisPositionKeyName);
                    if (ConfigContainer.Instance().AllKeys.Contains("interaction.disposition-object-name"))
                        _chatDataContext.dicCMEObjects.Add("DispositionName", (string)ConfigContainer.Instance().GetValue("interaction.disposition-object-name"));
                    else
                        _chatDataContext.dicCMEObjects.Add("DispositionName", string.Empty);
                    Pointel.Interactions.DispositionCodes.InteractionHandler.Listener _dispositionCodeListener = new Pointel.Interactions.DispositionCodes.InteractionHandler.Listener();
                    _dispositionCodeListener.NotifyCMEObjects(_chatDataContext.dicCMEObjects);
                    DispositionData disData = new DispositionData() { InteractionID = interactionID };
                    _dispositionUC = _dispositionCodeListener.CreateUserControl();
                    _chatDataContext.DispositionObjCollection = new KeyValuePair<string, object>("DispositionObj", _dispositionUC);
                    stpDispCodelist.Children.Clear();
                    _dispositionUC.NotifyDispositionCodeEvent += new Pointel.Interactions.DispositionCodes.UserControls.Disposition.NotifyDispositionCode(NotifyDispositionCodeEvent);
                    _dispositionUC.Dispositions(Pointel.Interactions.IPlugins.MediaTypes.Chat, disData);

                    if (_dispositionUC != null)
                    {
                        if (!_dispositionUserControls.ContainsKey(interactionID))
                            _dispositionUserControls.Add(interactionID, _dispositionUC);
                        else
                            _dispositionUserControls[interactionID] = _dispositionUC;
                        stpDispCodelist.Children.Add(_dispositionUC);
                    }
                    isFirstTimeCall = false;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error Occurred as LoadDispostionCodes() :" + generalException.Message);
            }
        }

        /// <summary>
        /// Notifies the disposition code event.
        /// </summary>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="data">The data.</param>
        public void NotifyDispositionCodeEvent(MediaTypes mediaType, DispositionData data)
        {
            var chatMedia = new ChatMedia();
            try
            {
                if (mediaType == MediaTypes.Chat)
                {
                    if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
                    {
                        if (isFirstTimeCall)
                        {
                            isFirstTimeCall = false;
                            Dictionary<string, string> dispositionCode = new Dictionary<string, string>();
                            dispositionCode.Add(_chatDataContext.DisPositionKeyName,
                                      _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString());
                            _dispositionUC.ReLoadDispositionCodes(dispositionCode, _interactionID);
                        }
                        else
                        {
                            string originalValue = string.Empty;
                            if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
                                originalValue = _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString();
                            if (data.DispostionCode != originalValue)
                            {
                                KeyValueCollection caseData = new KeyValueCollection();
                                caseData.Add(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                                OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                                if (output.MessageCode == "200")
                                {
                                    _chatUtil.UserData.Remove(_chatDataContext.DisPositionKeyName);
                                    _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatCaseData> tempCaseData = _chatUtil.NotifyCaseData;
                                    int position1 = tempCaseData.IndexOf(tempCaseData.Where(p => p.Key == _chatDataContext.DisPositionKeyName).FirstOrDefault());
                                    _chatUtil.NotifyCaseData.RemoveAt(position1);
                                    _chatUtil.NotifyCaseData.Insert(position1, new ChatCaseData(_chatDataContext.DisPositionKeyName, data.DispostionCode));
                                    NotifyDispositionToSFDC(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                                }
                                caseData.Clear();
                                caseData = null;
                            }
                        }
                    }
                    else
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        caseData.Add(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                        OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                        if (output.MessageCode == "200")
                        {
                            _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                            _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DisPositionKeyName, data.DispostionCode));
                            NotifyDispositionToSFDC(_chatDataContext.DisPositionKeyName, data.DispostionCode);
                        }
                        caseData.Clear();
                        caseData = null;
                    }
                    if (_chatUtil.UserData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                    {
                        string originalValue = _chatUtil.UserData[_chatDataContext.DispositionCollectionKeyName].ToString();
                        if (data.DispostionCode != originalValue)
                        {
                            string result = string.Join("; ", data.DispostionCollection.Select(x => string.Format("{0}:{1}", x.Key, x.Value)).ToArray());
                            if (!string.IsNullOrEmpty(result))
                            {
                                KeyValueCollection caseData = new KeyValueCollection();
                                caseData.Add(_chatDataContext.DispositionCollectionKeyName, result);
                                OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                                if (output.MessageCode == "200")
                                {
                                    _chatUtil.UserData.Remove(_chatDataContext.DispositionCollectionKeyName);
                                    _chatUtil.UserData.Add(_chatDataContext.DispositionCollectionKeyName, result);
                                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatCaseData> tempCaseData = _chatUtil.NotifyCaseData;
                                    int position1 = tempCaseData.IndexOf(tempCaseData.Where(p => p.Key == _chatDataContext.DispositionCollectionKeyName).FirstOrDefault());
                                    _chatUtil.NotifyCaseData.RemoveAt(position1);
                                    _chatUtil.NotifyCaseData.Insert(position1, new ChatCaseData(_chatDataContext.DispositionCollectionKeyName, result));
                                }
                                caseData.Clear();
                                caseData = null;
                            }
                        }
                    }
                    else
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        string result = string.Join("; ", data.DispostionCollection.Select(x => string.Format("{0}:{1}", x.Key, x.Value)).ToArray());
                        if (!string.IsNullOrEmpty(result))
                        {
                            caseData.Add(_chatDataContext.DispositionCollectionKeyName, result);
                            OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                            if (output.MessageCode == "200")
                            {
                                _chatUtil.UserData.Add(_chatDataContext.DispositionCollectionKeyName, result);
                                _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DispositionCollectionKeyName, result));
                            }
                            caseData.Clear();
                            caseData = null;
                        }
                    }
                    if (data.DispostionCode == "None")
                        _chatUtil.IsDispositionSelected = false;
                    else
                        _chatUtil.IsDispositionSelected = true;

                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while NotifyDispositionCodeEvent(): " + generalException.ToString());
                _chatUtil.IsDispositionSelected = false;
            }
            finally
            {
                chatMedia = null;
            }
        }

        static Microsoft.Windows.Controls.DataGridCell TryToFindGridCell(Microsoft.Windows.Controls.DataGrid grid, Microsoft.Windows.Controls.DataGridCellInfo cellInfo)
        {
            Microsoft.Windows.Controls.DataGridCell result = null;
            Microsoft.Windows.Controls.DataGridRow row = null;
            try
            {
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
            }
            catch (Exception generalException)
            {
                string error = generalException.Message;
                result = null;
            }
            return result;
        }

        private void btnAddCaseData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_caseDataAdd.Items.Count > 0)
                {
                    _caseDataAdd.PlacementTarget = btnAddCaseData;
                    _caseDataAdd.IsOpen = true;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in btnAddCaseData_Click() : " + generalException.ToString());
            }
        }

        /// <summary>
        ///     Handles the Click event of the btnClear control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DGCaseDataInfo.CancelEdit();
                BindGrid();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while btnClear_Click(): " + commonException.ToString());
            }
        }

        private void btnSaveNote_Click(object sender, RoutedEventArgs e)
        {
            KeyValueCollection caseData = new KeyValueCollection();
            var chatMedia = new ChatMedia();
            try
            {
                string value = _chatUtil.InteractionNoteContent;
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Plugins.Contact))
                    ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact])
                        .UpdateInteraction(_interactionID, _chatDataContext.OwnerIDorPersonDBID, _chatUtil.InteractionNoteContent,
                        _chatUtil.UserData, 2);
                if (_chatUtil.UserData.ContainsKey("TheComment"))
                {
                    string originalValue = _chatUtil.UserData["TheComment"].ToString();
                    if (value != originalValue)
                    {
                        caseData.Add("TheComment", value);
                        OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                        if (output.MessageCode == "200")
                        {
                            _chatUtil.UserData.Remove("TheComment");
                            _chatUtil.UserData.Add("TheComment", value);
                        }
                        caseData.Clear();
                    }
                }
                else
                {
                    caseData.Add("TheComment", value);
                    OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                    if (output.MessageCode == "200")
                    {
                        _chatUtil.UserData.Add("TheComment", value);
                    }
                    caseData.Clear();
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as : " + commonException.Message.ToString());
            }
            finally
            {
                chatMedia = null;
                caseData = null;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var chatMedia = new ChatMedia();
            KeyValueCollection caseData = new KeyValueCollection();
            try
            {
                var selectedCallData = DGCaseDataInfo.SelectedItem as ChatCaseData;
                string key = selectedCallData.Key.ToString().Trim();
                string value = selectedCallData.Value.ToString().Trim();
                if (_chatUtil.UserData.ContainsKey(key))
                {
                    string originalValue = _chatUtil.UserData[key].ToString();
                    if (value != originalValue)
                    {
                        caseData.Add(key, value);
                        OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                        if (output.MessageCode == "200")
                        {
                            _chatUtil.UserData.Remove(key);
                            _chatUtil.UserData.Add(key, value);
                        }
                        caseData.Clear();
                    }
                }
                else
                {
                    caseData.Add(key, value);
                    OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                    if (output.MessageCode == "200")
                    {
                        _chatUtil.UserData.Add(key, value);
                    }
                    caseData.Clear();
                }
                BindGrid();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while btnUpdate_Click(): " + commonException.ToString());
            }
            finally
            {
                chatMedia = null;
                caseData = null;
            }
        }

        void CaseDataAddMenuitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                if (!_chatUtil.UserData.ContainsKey(menuitem.Header.ToString()))
                {
                    _chatUtil.UserData.Add(menuitem.Header.ToString(), string.Empty);
                }
                if (_chatUtil.NotifyCaseData.Count(p => p.Key == menuitem.Header.ToString()) == 0)
                {
                    _chatUtil.NotifyCaseData.Add(new ChatCaseData(menuitem.Header.ToString(), string.Empty));
                }
                BindGrid();
                _caseDataAdd.Items.Remove(menuitem);
                DGCaseDataInfo.UpdateLayout();
                if (DGCaseDataInfo.Items.Count > 2)
                    DGCaseDataInfo.ScrollIntoView(DGCaseDataInfo.Items[DGCaseDataInfo.Items.Count - 2]);
                int rowIndex = _chatUtil.NotifyCaseData.IndexOf(_chatUtil.NotifyCaseData.Where(p => p.Key == menuitem.Header.ToString()).FirstOrDefault());
                var dataGridCellInfo = new Microsoft.Windows.Controls.DataGridCellInfo(DGCaseDataInfo.Items[rowIndex], DGCaseDataInfo.Columns[1]);
                var cell = TryToFindGridCell(DGCaseDataInfo, dataGridCellInfo);
                if (cell == null) return;
                cell.Focus();
                DGCaseDataInfo.BeginEdit();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as : " + commonException.Message.ToString());
            }
        }

        private void DGCaseDataInfo_BeginningEdit(object sender, Microsoft.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            try
            {
                var selectedCaseData = DGCaseDataInfo.SelectedItem as ChatCaseData;
                if (selectedCaseData != null)
                {
                    //if (_chatDataContext.IsChatEnabledModifyCaseData)
                    //{
                    //    e.Cancel = false;
                    //    return;
                    //}
                    //else if (chatUtil.NotifyCaseData.Count(p => p.Key == selectedCallData.Key.ToString()) > 0)
                    //{
                    //    e.Cancel = false;
                    //    return;
                    //}
                    //else
                    //{
                    //    e.Cancel = true;
                    //}
                    if (_chatDataContext.LoadCaseDataFilterKeys != null && _chatDataContext.LoadCaseDataFilterKeys.Count > 0)
                    {
                        //if (!string.IsNullOrEmpty(selectedCaseData.Value.ToString()))
                        //{
                        if (_chatDataContext
                                .LoadCaseDataFilterKeys.Any(
                                    x =>
                                        x.ToString().Trim().ToLower() ==
                                        selectedCaseData.Key.ToString().Trim().ToLower()) && (((string)ConfigContainer.Instance().GetValue("chat.enable.modify-case-data")).ToLower().Equals("true")))
                        {
                            e.Cancel = false;
                            return;
                        }
                        if (_chatDataContext
                            .LoadCaseDataKeys.Any(
                                x =>
                                    x.ToString().Trim().ToLower() ==
                                    selectedCaseData.Key.ToString().Trim().ToLower()) && (((string)ConfigContainer.Instance().GetValue("chat.enable.modify-case-data")).ToLower().Equals("true")))
                        {
                            e.Cancel = false;
                            return;
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                        //}
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as DGCaseDataInfo_BeginningEdit() : " + _generalException.ToString());
            }
        }

        private void DGCaseDataInfo_PreparingCellForEdit(object sender, Microsoft.Windows.Controls.DataGridPreparingCellForEditEventArgs e)
        {
            try
            {
                var contentPresenter = e.EditingElement as ContentPresenter;
                var editingTemplate = contentPresenter.ContentTemplate;
                var textBox = (editingTemplate as DataTemplate).FindName("txtValue", (contentPresenter as ContentPresenter));
                if (textBox == null) return;
                if (!(textBox is TextBox)) return;
                (textBox as TextBox).Focus();
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in DGCaseDataInfo_PreparingCellForEdit() : " + generalException.ToString());
            }
        }

        private void DGCaseDataInfo_RowEditEnding(object sender, Microsoft.Windows.Controls.DataGridRowEditEndingEventArgs e)
        {
            var chatMedia = new ChatMedia();
            try
            {
                Microsoft.Windows.Controls.DataGridRow dgRow = e.Row;
                if (dgRow != null)
                {
                    var selectedCallData = dgRow.Item as ChatCaseData;
                    string key = selectedCallData.Key.ToString().Trim();
                    string value = selectedCallData.Value.ToString().Trim();
                    if (_chatUtil.UserData.ContainsKey(key))
                    {
                        string originalValue = _chatUtil.UserData[key].ToString();
                        if (value != originalValue)
                        {
                            KeyValueCollection caseData = new KeyValueCollection();
                            caseData.Add(key, value);
                            OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                            if (output.MessageCode == "200")
                            {
                                _chatUtil.UserData.Remove(key);
                                _chatUtil.UserData.Add(key, value);
                            }
                            caseData.Clear();
                        }
                    }
                    else
                    {
                        KeyValueCollection caseData = new KeyValueCollection();
                        caseData.Add(key, value);
                        OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatUtil.ProxyId, caseData);
                        if (output.MessageCode == "200")
                        {
                            _chatUtil.UserData.Add(key, value);
                        }
                        caseData.Clear();
                    }
                    BindGrid();
                }
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred while DGCaseDataInfo_RowEditEnding(): " + commonException.ToString());
            }
            finally
            {
                chatMedia = null;
            }
        }

        private void NotifyDispositionToSFDC(string keyName, string value)
        {
            try
            {
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(IPlugins.Plugins.Salesforce))
                    ((ISFDCConnector)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Salesforce]).NotifyChatDispositionCode(_interactionID, keyName, value);
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as " + _generalException.Message);
            }
        }

        private void radioDispButton_Checked(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    var radioButton = (RadioButton)sender;
            //    var chatMedia = new ChatMedia();
            //    string[] tempSessionData = Name.Split('_');
            //    if (tempSessionData[1] == _interactionID)
            //    {
            //        var temp = _chatDataContext.LoadDispositionCodes.FirstOrDefault(x => x.Key.Contains(radioButton.Content.ToString()));
            //        if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
            //        {
            //            string originalValue = _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString();
            //            if (temp.Key != originalValue)
            //            {
            //                KeyValueCollection caseData = new KeyValueCollection();
            //                caseData.Add(_chatDataContext.DisPositionKeyName, temp.Key);
            //                OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatDataContext.ProxyId, caseData);
            //                if (output.MessageCode == "200")
            //                {
            //                    _chatUtil.UserData.Remove(_chatDataContext.DisPositionKeyName);
            //                    _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, temp.Key);
            //                    ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatCaseData> tempCaseData = _chatUtil.NotifyCaseData;
            //                    int position1 = tempCaseData.IndexOf(tempCaseData.Where(p => p.Key == _chatDataContext.DisPositionKeyName).FirstOrDefault());
            //                    _chatUtil.NotifyCaseData.RemoveAt(position1);
            //                    _chatUtil.NotifyCaseData.Insert(position1, new ChatCaseData(_chatDataContext.DisPositionKeyName, temp.Key));
            //                }
            //                caseData.Clear();
            //            }
            //        }
            //        else
            //        {
            //            KeyValueCollection caseData = new KeyValueCollection();
            //            caseData.Add(_chatDataContext.DisPositionKeyName, temp.Key);
            //            OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatDataContext.ProxyId, caseData);
            //            if (output.MessageCode == "200")
            //            {
            //                _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, temp.Key);
            //                _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DisPositionKeyName, temp.Key));
            //            }
            //            caseData.Clear();
            //        }
            //        BindGrid();
            //    }
            //    else
            //    {
            //        KeyValueCollection caseData = new KeyValueCollection();
            //        caseData.Add(_disPositionKeyName, radioButton.Content.ToString());
            //        OutputValues output = chatMedia.DoUpdateCaseDataProperties(tempSessionData[1], Convert.ToInt32(tempSessionData[2]), caseData);
            //    }
            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = true;
            //}
            //catch (Exception commonException)
            //{
            //    _logger.Error("Chat_InteractionData_radioDispButton_Checked:" + commonException);
            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = false;
            //}
        }

        private void radioDispButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            try
            {
                var toolTipRadioButton = (RadioButton)sender;
                foreach (var radioButton in LogicalTreeHelper.GetChildren(stpDispCodelist).Cast<RadioButton>().Where(radioButton => radioButton.Content.ToString().Equals(toolTipRadioButton.Content.ToString())))
                {
                    radioButton.ToolTip = radioButton.Content.ToString();
                }
            }
            catch (Exception _generalException)
            {
                _logger.Error("Error occurred as radioDispButton_MouseEnter() : " + _generalException.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the rdbutton control.
        /// </summary>
        /// <param name="sender">tabDispositionhe source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void rdbutton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //try
            //{

            //    var radioButton = (RadioButton)sender;
            //    var chatMedia = new ChatMedia();
            //    if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
            //    {
            //        string originalValue = _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString();
            //        if (originalValue != string.Empty)
            //        {
            //            KeyValueCollection caseData = new KeyValueCollection();
            //            caseData.Add(_chatDataContext.DisPositionKeyName, "");
            //            OutputValues output = chatMedia.DoUpdateCaseDataProperties(_interactionID, _chatDataContext.ProxyId, caseData);
            //            if (output.MessageCode == "200")
            //            {
            //                _chatUtil.UserData.Remove(_chatDataContext.DisPositionKeyName);
            //                _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, "");
            //                ObservableCollection<Pointel.Interactions.Chat.Helpers.IChatCaseData> tempCaseData = _chatUtil.NotifyCaseData;
            //                int position1 = tempCaseData.IndexOf(tempCaseData.Where(p => p.Key == _chatDataContext.DisPositionKeyName).FirstOrDefault());
            //                _chatUtil.NotifyCaseData.RemoveAt(position1);
            //                _chatUtil.NotifyCaseData.Insert(position1, new ChatCaseData(_chatDataContext.DisPositionKeyName, ""));
            //            }
            //            caseData.Clear();
            //        }
            //    }
            //    else
            //    {
            //        KeyValueCollection caseData = new KeyValueCollection();
            //        caseData.Add(_chatDataContext.DisPositionKeyName, "");
            //        OutputValues output = chatMedia.DoAddCaseDataProperties(_interactionID, _chatDataContext.ProxyId, caseData);
            //        if (output.MessageCode == "200")
            //        {
            //            _chatUtil.UserData.Add(_chatDataContext.DisPositionKeyName, "");
            //            _chatUtil.NotifyCaseData.Add(new ChatCaseData(_chatDataContext.DisPositionKeyName, ""));
            //        }
            //        caseData.Clear();
            //    }
            //    BindGrid();
            //    (_chatDataContext.MainWindowSession[_interactionID] as ChatUtil).IsDispositionSelected = true;
            //}
            //catch (Exception commonException)
            //{
            //    _logger.Error("rdbutton_MouseLeftButtonUp:" + commonException);
            //}
        }

        private void tabDisposition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = sender as TabControl;
                var selected = item.SelectedItem as TabItem;
                if (selected != null && selected.Header.ToString() == "_Dispositions" && _dispositionUC != null)
                {
                    Dictionary<string, string> dispositionCode = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(_chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString()))
                        dispositionCode.Add(_chatDataContext.DisPositionKeyName,
                                  _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString());
                    else
                        dispositionCode.Add(_chatDataContext.DisPositionKeyName,
                             "None");

                    _dispositionUC.ReLoadDispositionCodes(dispositionCode, _interactionID);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at tabDisposition_SelectionChanged" + ex.Message);
            }
        }

        private void txtNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_chatUtil != null)
                    _chatUtil.InteractionNoteContent = txtNotes.Text.ToString();
            }
            catch (Exception commonException)
            {
                _logger.Error("Error occurred as : " + commonException.Message.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _chatUtil.IsChatEnabledAddCaseData = _chatDataContext.IsChatEnabledAddCaseData;

                if ((((string)ConfigContainer.Instance().GetValue("chat.enable.modify-case-data")).ToLower().Equals("true")))
                    _chatUtil.IsChatEnabledModifyCaseData = true;
                else
                    _chatUtil.IsChatEnabledModifyCaseData = false;

                foreach (string item in _chatDataContext.LoadCaseDataKeys)
                {
                    if (_chatUtil.NotifyCaseData.Count(p => p.Key.ToLower().Trim() == item.ToString().ToLower().Trim()) == 0)
                    {
                        MenuItem _mItem = new MenuItem();
                        _mItem.Header = item;
                        _mItem.Click += new RoutedEventHandler(CaseDataAddMenuitem_Click);
                        _caseDataAdd.Style = (Style)FindResource("Contextmenu");
                        _caseDataAdd.Items.Add(_mItem);
                    }
                }
                if (_chatUtil.IxnType == "Consult")
                {
                    stpDispCodelist.IsEnabled = false;
                    gdNote.IsEnabled = false;
                    gdCaseData.IsEnabled = false;
                }
                else
                {
                    stpDispCodelist.IsEnabled = true;
                    gdCaseData.IsEnabled = true;
                    gdNote.IsEnabled = true;
                }
                _disPositionKeyName = _chatDataContext.DisPositionKeyName;
                LoadDispostionCodes(_interactionID);
                if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName) || _chatUtil.UserData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                {
                    Dictionary<string, string> dispositionTree = new Dictionary<string, string>();
                    if (_chatDataContext.DispositionObjCollection.Value != null)
                    {
                        var dispositionObject = (Pointel.Interactions.DispositionCodes.UserControls.Disposition)
                                        _chatDataContext.DispositionObjCollection.Value;
                        if (_chatUtil.UserData.ContainsKey(_chatDataContext.DispositionCollectionKeyName))
                        {
                            if (!string.IsNullOrEmpty(_chatUtil.UserData[_chatDataContext.DispositionCollectionKeyName].ToString()))
                            {
                                _chatUtil.IsDispositionSelected = true;
                                dispositionTree = _chatUtil.UserData[_chatDataContext.DispositionCollectionKeyName].ToString().Split(';').Select(s => s.Split(':')).ToDictionary(a => a[0].Trim().ToString(), a => a[1].Trim().ToString());
                            }
                            else
                                _chatUtil.IsDispositionSelected = false;
                        }
                        if (_chatUtil.UserData.ContainsKey(_chatDataContext.DisPositionKeyName))
                        {
                            if (!string.IsNullOrEmpty(_chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString()) && _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString() != "None")
                            {
                                _chatUtil.IsDispositionSelected = true;
                                dispositionTree.Add(_chatDataContext.DisPositionKeyName, _chatUtil.UserData[_chatDataContext.DisPositionKeyName].ToString());
                            }
                            else
                                _chatUtil.IsDispositionSelected = false;
                        }
                        if (dispositionTree.Count > 0)
                            dispositionObject.ReLoadDispositionCodes(dispositionTree, _interactionID);
                        dispositionTree.Clear();
                        dispositionTree = null;
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while UserControl_Loaded() :" + generalException.Message);
            }
        }

        void _chatUtil_CaseDataUpdated()
        {
            try
            {
                List<MenuItem> dummyMenu = new List<MenuItem>();
                if (_caseDataAdd != null && _caseDataAdd.Items.Count > 0 && _chatUtil.UserData.Count > 0)
                {
                    foreach (MenuItem menuItem in _caseDataAdd.Items)
                        dummyMenu.Add(menuItem);

                    foreach (MenuItem mnu in dummyMenu)
                    {
                        if (_chatUtil.UserData.ContainsKey(mnu.Header.ToString()) && _caseDataAdd.Items.Contains(mnu))
                        {
                            _caseDataAdd.Items.Remove(mnu);
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in_chatUtil_CaseDataUpdated() : " + generalException.ToString());
            }
        }

        #endregion Methods
    }
}
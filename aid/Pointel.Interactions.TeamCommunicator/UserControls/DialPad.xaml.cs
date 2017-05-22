/*
* =====================================
* Pointel.Interactions.TeamCommunicator.UserControls
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Reporting.Protocols;
using Genesyslab.Platform.Reporting.Protocols.StatServer;
using Pointel.Configuration.Manager;
using Pointel.Connection.Manager;
using Pointel.Interactions.TeamCommunicator.AppReader;
using Pointel.Interactions.TeamCommunicator.ConnectionManager;
using Pointel.Interactions.TeamCommunicator.Helpers;
using Pointel.Interactions.TeamCommunicator.InteractionListener;
using Pointel.Interactions.TeamCommunicator.Request;
using Pointel.Interactions.TeamCommunicator.Settings;
using Pointel.Interactions.TeamCommunicator.WinForms;
using System.Threading;


namespace Pointel.Interactions.TeamCommunicator.UserControls
{
    /// <summary>
    /// Interaction logic for Check.xaml
    /// </summary>
    [Export(typeof(UserControl))]
    public partial class DialPad : UserControl
    {
        #region Private Members

        private bool isGroupEnabled = false;
        private bool isSelectAllClicked = true;
        private bool isFavoriteClicked = false;
        private bool isRecentClicked = false;
        LuceneParser parser = new LuceneParser();
        Dictionary<string, string> dictionaryValues = new Dictionary<string, string>();

        #endregion

        #region Delegate Members

        public delegate string ResponseNotification(Dictionary<string, string> dictionaryValues);

        #endregion

        #region Events Declaration

        public static event ResponseNotification responseNotify;

        #endregion

        #region Private Read Only Member

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        #endregion

        #region Image Files

        private BitmapImage favImageSource;

        #endregion

        [ImportingConstructor]
        public DialPad([Import("InteractionType")]Pointel.Interactions.IPlugins.InteractionType interactionType,
            [Import("OperationType")]Pointel.Interactions.IPlugins.OperationType operationType,
           [Import("RefFunction")] Func<Dictionary<string, string>, string> refFunction)
        {
            try
            {
                InitializeComponent();
                _logger.Info("Team Communicator - InteractionType : " + interactionType.ToString());
                _logger.Info("Team Communicator - OperationType : " + operationType.ToString());
                if (responseNotify != null)
                    responseNotify = null;
                responseNotify += new ResponseNotification(refFunction);
                this.DataContext = Datacontext.GetInstance();
                isSelectAllClicked = true;
                btnSelectAll.IsChecked = true;
                btnFavourite.IsChecked = false;
                btnRecent.IsChecked = false;
                Datacontext.GetInstance().CurrentInteractionType = interactionType;
                Datacontext.GetInstance().SelectedOperationType = operationType;
                Datacontext.GetInstance().InternalTargets = "";

                Listener._statusUpdation += new Listener.StatusUpdation(Listener__statusUpdation);

                if (Datacontext.GetInstance().TeamCommunicator != null)
                    Datacontext.GetInstance().TeamCommunicator.Clear();

                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                _logger.Error("DialPad() : Error : " + ex.Message.ToString());
            }
        }


        private void RequestTeamCommunicatorStatistics()
        {
            try
            {
                //StatisticsConnectionManager _statisticsConnectionManager = new StatisticsConnectionManager();
                //string primaryHost = string.Empty;
                //string primaryPort = string.Empty;
                //string secondaryHost = string.Empty;
                //string secondaryPort = string.Empty;
                //int addpServerTimeOut = 30;
                //int addClientTimeOut = 60;
                //int warmStandbyTimeOut = 10;
                //short warmStandbyAttempts = 5;

                //if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-host"))//primary Host -----------------------------------------------------------
                //    primaryHost = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-host");
                //if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-port"))//primary port -----------------------------------------------------------
                //    primaryPort = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-port");
                ////if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary Host -----------------------------------------------------------
                ////    secondaryHost = (string)ConfigContainer.Instance().GetValue("");
                ////if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary port -----------------------------------------------------------
                ////    secondaryPort = (string)ConfigContainer.Instance().GetValue("");

                //if (string.IsNullOrEmpty(secondaryHost))
                //    secondaryHost = primaryHost;
                //if (string.IsNullOrEmpty(secondaryPort))
                //    secondaryPort = primaryPort;

                //bool isStatServerOpened = false;
                //if (!string.IsNullOrEmpty(primaryHost) && !string.IsNullOrEmpty(primaryPort))
                //    isStatServerOpened = _statisticsConnectionManager.ConnectStatServer(primaryHost, primaryPort, secondaryHost, secondaryPort,
                //                addpServerTimeOut, addClientTimeOut, warmStandbyTimeOut, warmStandbyAttempts);

                //if (isStatServerOpened)
                if (ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()].State == Genesyslab.Platform.Commons.Protocols.ChannelState.Opened)
                {
                    string[] configuredAgentGroups = new string[5000];
                    if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.enable-config-agent-groups") &&
                           ((string)ConfigContainer.Instance().GetValue("teamcommunicator.enable-config-agent-groups")).ToLower().Equals("true") &&
                           ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.consult-agent-groups") &&
                           ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).ToString() != string.Empty)
                    {
                        string[] agentGroups = ((string)ConfigContainer.Instance().GetValue("teamcommunicator.consult-agent-groups")).Split(',');
                        int index = 0;
                        foreach (string groupName in agentGroups)
                        {
                            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries.CfgAgentGroupQuery qAgentGroup = new CfgAgentGroupQuery();
                            qAgentGroup.TenantDbid = ConfigContainer.Instance().TenantDbId;
                            qAgentGroup.Name = groupName;
                            CfgAgentGroup agentGroup = ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgAgentGroup>(qAgentGroup);
                            if (agentGroup != null && agentGroup.Agents != null && agentGroup.Agents.Count > 0)
                            {
                                if (!configuredAgentGroups.Contains(agentGroup.GroupInfo.Name))
                                {
                                    configuredAgentGroups[index] = (agentGroup.GroupInfo.Name);
                                    index++;
                                }
                            }
                        }
                    }
                    configuredAgentGroups = configuredAgentGroups.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string dynamicStatSectionName = string.Empty;
                    string businessAttributeName = string.Empty;

                    if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                                !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic-stats")))
                        businessAttributeName = (string)ConfigContainer.Instance().GetValue("dynamic-stats");

                    if (ConfigContainer.Instance().AllKeys.Contains("dynamic-stats") &&
                                !string.IsNullOrEmpty((string)ConfigContainer.Instance().GetValue("dynamic.statistics.name")))
                        dynamicStatSectionName = (string)ConfigContainer.Instance().GetValue("dynamic.statistics.name");

                    Thread thTeamCommunicatortatistics = new Thread(delegate()
                    {
                        RequestStatistics.RequestTeamCommunicatorStatistics(businessAttributeName, dynamicStatSectionName, configuredAgentGroups, StatisticObjectType.GroupAgents, 1, 1);
                    });
                    thTeamCommunicatortatistics.Start();
                }
                else
                {
                    _logger.Warn("Stat server connection not opened in Team communicator");
                }

            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while establishing connection to stat server in Team communicator " +
                            ((generalException.InnerException == null) ? generalException.Message : generalException.InnerException.ToString()));
            }
        }

        private void CloseStatServer()
        {
            try
            {
                ((StatServerProtocol)ProtocolManagers.Instance().ProtocolManager[ServerType.Statisticsserver.ToString()]).Close();
            }
            catch (Exception generalEsception)
            {


            }
        }

        private void LoadSearchParameters()
        {
            try
            {
                List<Datacontext.SelectorFilters> mediaSearchList = new List<Datacontext.SelectorFilters>();


                if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                {
                    if (Datacontext.GetInstance().FilterList.Count < 1)
                    {
                        mediaSearchList.Clear();
                        mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                        mediaSearchList.Add(Datacontext.SelectorFilters.Skill);
                        comboBox1.ItemsSource = mediaSearchList;
                        comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                    }
                    else
                    {
                        mediaSearchList.Clear();
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent))
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AgentGroup))
                            mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Skill))
                            mediaSearchList.Add(Datacontext.SelectorFilters.Skill);

                        if (mediaSearchList.Count > 0)
                        {
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Skill);
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                {
                    if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                    {
                        mediaSearchList.Clear();
                        mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                        comboBox1.ItemsSource = mediaSearchList;
                        comboBox1.SelectedItem = Datacontext.SelectorFilters.InteractionQueue;
                        comboBox1.IsEnabled = false;
                    }
                    else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Supervisor)
                    {
                        if (Datacontext.GetInstance().FilterList.Count < 1)
                        {
                            mediaSearchList.Clear();
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            if (Datacontext.GetInstance().IsSupervisorMoveWorkbinEnabled)
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            if (Datacontext.GetInstance().IsSupervisorMoveQueueEnabled)
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                        }
                        else
                        {
                            mediaSearchList.Clear();
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent) && Datacontext.GetInstance().IsSupervisorMoveWorkbinEnabled)
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.InteractionQueue) && Datacontext.GetInstance().IsSupervisorMoveQueueEnabled)
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            if (mediaSearchList.Count > 0)
                            {
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedIndex = 0;
                            }
                            else
                            {
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                                if (Datacontext.GetInstance().IsSupervisorMoveWorkbinEnabled)
                                    mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                                if (Datacontext.GetInstance().IsSupervisorMoveQueueEnabled)
                                    mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                            }
                        }
                    }
                    else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                    {
                        if (Datacontext.GetInstance().FilterList.Count < 1)
                        {
                            mediaSearchList.Clear();
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                            mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                        }
                        else
                        {
                            mediaSearchList.Clear();
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent))
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AgentGroup))
                                mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.InteractionQueue))
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);

                            if (mediaSearchList.Count > 0)
                            {
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedIndex = 0;
                            }
                            else
                            {
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                                mediaSearchList.Add(Datacontext.SelectorFilters.AgentGroup);
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedIndex = 0;
                            }
                        }
                    }
                    else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                    {
                        if (Datacontext.GetInstance().FilterList.Count < 1)
                        {
                            mediaSearchList.Clear();
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            //mediaSearchList.Add("AgentGroup");
                            //mediaSearchList.Add("InteractionQueue");
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = "AllTypes";
                        }
                        else
                        {
                            mediaSearchList.Clear();
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent))
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            //if (Datacontext.GetInstance().FilterList.Contains("AgentGroup"))
                            //    mediaSearchList.Add("AgentGroup");
                            if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.InteractionQueue))
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            if (mediaSearchList.Count > 0)
                            {
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedIndex = 0;
                            }
                            else
                            {
                                mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                                mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                                mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                                // mediaSearchList.Add("AgentGroup");
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                                comboBox1.ItemsSource = mediaSearchList;
                                comboBox1.SelectedIndex = 0;
                            }
                        }
                    }
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                {
                    if (Datacontext.GetInstance().FilterList.Count < 1)
                    {
                        mediaSearchList.Clear();
                        mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        //mediaSearchList.Add("AgentGroup");
                        //mediaSearchList.Add("InteractionQueue");
                        comboBox1.ItemsSource = null;
                        comboBox1.ItemsSource = mediaSearchList;
                        comboBox1.SelectedItem = "AllTypes";
                    }
                    else
                    {
                        mediaSearchList.Clear();
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent))
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        //if (Datacontext.GetInstance().FilterList.Contains("AgentGroup"))
                        //    mediaSearchList.Add("AgentGroup");
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.InteractionQueue))
                            mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                        if (mediaSearchList.Count > 0)
                        {
                            comboBox1.ItemsSource = null;
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            // mediaSearchList.Add("AgentGroup");
                            comboBox1.ItemsSource = null;
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                {
                    if (Datacontext.GetInstance().FilterList.Count < 1)
                    {
                        mediaSearchList.Clear();
                        mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        //mediaSearchList.Add("AgentGroup");
                        //mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                        comboBox1.ItemsSource = mediaSearchList;
                        comboBox1.SelectedItem = Datacontext.SelectorFilters.AllTypes;
                    }
                    else
                    {
                        mediaSearchList.Clear();
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.AllTypes))
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Agent))
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                        //if (Datacontext.GetInstance().FilterList.Contains("AgentGroup"))
                        //    mediaSearchList.Add("AgentGroup");
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.InteractionQueue))
                            mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                        if (mediaSearchList.Count > 0)
                        {
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            mediaSearchList.Add(Datacontext.SelectorFilters.AllTypes);
                            mediaSearchList.Add(Datacontext.SelectorFilters.Agent);
                            mediaSearchList.Add(Datacontext.SelectorFilters.InteractionQueue);
                            // mediaSearchList.Add("AgentGroup");
                            //comboBox1.ItemsSource = mediaSearchList;
                            //comboBox1.SelectedItem = "AllTypes";
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                }
                else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                {
                    if (Datacontext.GetInstance().FilterList.Count < 1)
                    {
                        mediaSearchList.Clear();
                        mediaSearchList.Add(Datacontext.SelectorFilters.Contact);
                        comboBox1.ItemsSource = mediaSearchList;
                        comboBox1.SelectedItem = Datacontext.SelectorFilters.Contact;
                    }
                    else
                    {
                        mediaSearchList.Clear();
                        if (Datacontext.GetInstance().FilterList.Contains(Datacontext.SelectorFilters.Contact))
                            mediaSearchList.Add(Datacontext.SelectorFilters.Contact);

                        if (mediaSearchList.Count > 0)
                        {
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            mediaSearchList.Add(Datacontext.SelectorFilters.Contact);
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedItem = Datacontext.SelectorFilters.Contact;
                            comboBox1.ItemsSource = mediaSearchList;
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                }
                else
                    comboBox1.ItemsSource = Datacontext.GetInstance().FilterList;

            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString());
            }
        }


        /// <summary>
        /// Handles the Click event of the btnSelectAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            //TeamCGrid.Visibility = System.Windows.Visibility.Visible;
            btnSelectAll.IsChecked = true;
            btnFavourite.IsChecked = false;
            btnRecent.IsChecked = false;
            isSelectAllClicked = true;
            isFavoriteClicked = false;
            isRecentClicked = false;
            if (Datacontext.GetInstance().TeamCommunicator != null)
                Datacontext.GetInstance().TeamCommunicator.Clear();
            if (txtSearch.Text != string.Empty)
                txtSearch_TextChanged(null, null);
            else
                Datacontext.GetInstance().InternalTargets = "";
        }

        /// <summary>
        /// Handles the Click event of the btnFavourite control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnFavourite_Click(object sender, RoutedEventArgs e)
        {
            //TeamCGrid.Visibility = System.Windows.Visibility.Visible;
            btnSelectAll.IsChecked = false;
            btnFavourite.IsChecked = true;
            btnRecent.IsChecked = false;

            isSelectAllClicked = false;
            isFavoriteClicked = true;
            isRecentClicked = false;
            if (Datacontext.GetInstance().TeamCommunicator != null)
                Datacontext.GetInstance().TeamCommunicator.Clear();
            txtSearch_TextChanged(null, null);
        }

        /// <summary>
        /// Handles the Click event of the btnRecent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnRecent_Click(object sender, RoutedEventArgs e)
        {
            //TeamCGrid.Visibility = System.Windows.Visibility.Visible;

            btnSelectAll.IsChecked = false;
            btnFavourite.IsChecked = false;
            btnRecent.IsChecked = true;

            isSelectAllClicked = false;
            isFavoriteClicked = false;
            isRecentClicked = true;
            if (Datacontext.GetInstance().TeamCommunicator != null)
                Datacontext.GetInstance().TeamCommunicator.Clear();
            txtSearch_TextChanged(null, null);
        }

        /// <summary>
        /// Handles the Click event of the btnGroup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnGroup_Click(object sender, RoutedEventArgs e)
        {
            if (isGroupEnabled == false)
            {
                btnGroup.IsChecked = true;
                isGroupEnabled = true;
                imgGroup.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/UnGroup.png", UriKind.Relative));
            }
            else
            {
                btnGroup.IsChecked = false;
                isGroupEnabled = false;
                imgGroup.Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Group.png", UriKind.Relative));
            }
            //TeamCGrid.Visibility = System.Windows.Visibility.Visible;
            txtSearch_TextChanged(null, null);

        }


        private void DialAgentInteraction(string uniqueIdentity, string searchedType)
        {
            try
            {
                string employeeID = "";
                KeyValuePair<string, string> kvpInfo = new KeyValuePair<string, string>();
                if (Datacontext.GetInstance().hshAgentEmployeeIdUserName != null)
                {
                    kvpInfo = Datacontext.GetInstance().hshAgentEmployeeIdUserName.Cast<DictionaryEntry>().ToDictionary(kvp => (string)kvp.Key, kvp =>
                                (string)kvp.Value).Where(kvp => kvp.Value == uniqueIdentity).FirstOrDefault();
                }
                if (kvpInfo.Key != null)
                    employeeID = kvpInfo.Key;

                string place = "";
                if (employeeID != null && employeeID != string.Empty)
                {
                    if (Datacontext.GetInstance().hshAgentPlace != null && Datacontext.GetInstance().hshAgentPlace.Count > 0 &&
                                        Datacontext.GetInstance().hshAgentPlace.ContainsKey(employeeID))
                        place = Datacontext.GetInstance().hshAgentPlace[employeeID].ToString();
                    else
                        place = "";

                    if (Datacontext.GetInstance().hshAgentDN != null && Datacontext.GetInstance().hshAgentDN.Count > 0 &&
                                        Datacontext.GetInstance().hshAgentDN.ContainsKey(employeeID))
                        Datacontext.GetInstance().SelectedDN = Datacontext.GetInstance().hshAgentDN[employeeID].ToString();
                    else
                        Datacontext.GetInstance().SelectedDN = "";

                    dictionaryValues = new Dictionary<string, string>();
                    dictionaryValues.Clear();
                    dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                    dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                    dictionaryValues.Add("SearchedType", searchedType);
                    dictionaryValues.Add("UniqueIdentity", uniqueIdentity);
                    dictionaryValues.Add("SelectedDN", Datacontext.GetInstance().SelectedDN);
                    dictionaryValues.Add("EmployeeId", employeeID);
                    dictionaryValues.Add("Place", place.ToString());

                    responseNotify.Invoke(dictionaryValues);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        private void DialOtherInteraction(string uniqueIdentity, string searchedType)
        {
            try
            {
                dictionaryValues.Clear();
                dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                dictionaryValues.Add("SearchedType", searchedType);
                dictionaryValues.Add("UniqueIdentity", uniqueIdentity);
                dictionaryValues.Add("RoutingAddress", Datacontext.GetInstance().RoutingAddress);
                responseNotify.Invoke(dictionaryValues);  
            }
            catch (Exception ex)
            {
                _logger.Error(((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
        }

        /// <summary>
        /// Handles the Click event of the mnuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void mnuItem_Click(object sender, RoutedEventArgs e)
        {
            //Implement the call functionality here to make call
            Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator teamComm = ((FrameworkElement)sender).DataContext as Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator;

            //Implement the call based on the Selected Type if it is Agent Call DN, if AgentGroup or Skill call Routing Point
            //TeamCGrid.Visibility = System.Windows.Visibility.Hidden;

            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
            {
                if (teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                {
                    //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                    //    teamComm.UniqueIdentity, Datacontext.GetInstance().RoutingAddress, "");
                    DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                }
                else
                {
                    DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                }
            }

            if (!Datacontext.GetInstance().dtRecentInteractions.Columns["UniqueIdentity"].ToString().Contains(teamComm.UniqueIdentity))
            {
                DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                dtRow["Type"] = teamComm.SearchedType;
                dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
            }
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// Handles the Click event of the mnuFavorite control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void mnuFavorite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isIntOrNot = false;
                Int64 EnteredIntValue = 0;
                Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator teamComm = ((FrameworkElement)sender).DataContext as Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator;

                if (txtSearch.Text != string.Empty && txtSearch.Text != null)
                    isIntOrNot = Int64.TryParse(txtSearch.Text, out EnteredIntValue);//If textbox is not empty
                else               //If textbox is empty select based on the selected row in datagrid
                    isIntOrNot = Int64.TryParse(teamComm.SearchedName, out EnteredIntValue);

                if (isIntOrNot)//Entered text is a number(DN)
                    Datacontext.GetInstance().IsFavoriteItem = false;
                else
                    Datacontext.GetInstance().IsFavoriteItem = true;
                Keyboard.ClearFocus();
                MenuItem mnuItem = sender as MenuItem;
                if (mnuItem.Header.ToString().ToLower().Contains("add"))
                {
                    Datacontext.GetInstance().IsEditFavorite = false;
                    Datacontext.GetInstance().UniqueIdentity = teamComm.UniqueIdentity;
                    Datacontext.GetInstance().FavoriteItemSelectedType = teamComm.SearchedType.ToString();
                    Datacontext.GetInstance().FavoriteDisplayName = teamComm.SearchedName;

                    Favorites favMenu = new Favorites();
                    favMenu.ShowDialog();
                }
                else if (mnuItem.Header.ToString().ToLower().Contains("remove"))
                {
                    //Show Message box while deleting the favorite item
                    Pointel.Interactions.TeamCommunicator.WinForms.MessageBox showMessageBox = new Pointel.Interactions.TeamCommunicator.WinForms.MessageBox("Information",
                        "Are you sure, you want to remove this favorite item", "Yes", "No");
                    showMessageBox.ShowDialog();
                    if (showMessageBox.DialogResult == true)
                    {
                        foreach (DataRow row in Datacontext.GetInstance().dtFavorites.Rows)
                        {
                            if (row["UniqueIdentity"].ToString() == teamComm.UniqueIdentity)
                            {
                                XMLHandler handler = new XMLHandler();
                                handler.RemoveFavorite(row["UniqueIdentity"].ToString(), row["Type"].ToString());
                                Datacontext.GetInstance().dtFavorites.Rows.Remove(row);
                                break;
                            }
                        }
                    }
                }
                else if (mnuItem.Header.ToString().ToLower().Contains("edit"))
                {
                    Datacontext.GetInstance().IsEditFavorite = true;
                    Datacontext.GetInstance().UniqueIdentity = teamComm.UniqueIdentity;
                    Datacontext.GetInstance().FavoriteItemSelectedType = teamComm.SearchedType.ToString();
                    Datacontext.GetInstance().FavoriteDisplayName = teamComm.SearchedName;
                    Favorites favMenu = new Favorites();
                    favMenu.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                _logger.Error("mnuFavorite_Click : " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the DGTeamCommunicator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DGTeamCommunicator_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TeamCGrid.Visibility = System.Windows.Visibility.Visible;
        }


        /// <summary>
        /// Handles the SelectionChanged event of the comboBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBox1.SelectedValue != null)
                    Datacontext.GetInstance().SelectedType = (Datacontext.SelectorFilters)Enum.Parse(typeof(Datacontext.SelectorFilters), comboBox1.SelectedValue.ToString());

                if (Datacontext.GetInstance().TeamCommunicator != null)
                    Datacontext.GetInstance().TeamCommunicator.Clear();

                txtSearch_TextChanged(null, null);
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : comboBox1_SelectionChanged : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator teamComm = ((FrameworkElement)sender).DataContext as Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator;
                Datacontext.GetInstance().FavoriteItemSelectedType = teamComm.SearchedType.ToString();
                Datacontext.GetInstance().SelectedDN = "";

                if (Datacontext.GetInstance().CurrentInteractionType == Interactions.IPlugins.InteractionType.Contact)
                {
                    dictionaryValues.Clear();
                    if (string.IsNullOrEmpty(teamComm.EmailAddress))
                        return;
                    else
                    {
                        dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                        dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                        dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                        dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                        dictionaryValues.Add("EmailAddress", teamComm.EmailAddress.Split(',')[0]);
                    }
                    if (!string.IsNullOrEmpty(teamComm.UniqueIdentity) && !(teamComm.UniqueIdentity.Contains("@")))
                        dictionaryValues.Add("ContactId", teamComm.UniqueIdentity);
                    else
                        dictionaryValues.Add("ContactId", string.Empty);

                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }

                    responseNotify.Invoke(dictionaryValues);


                    //if (Datacontext.GetInstance().SearchedContactDocuments.ContainsKey(teamComm.UniqueIdentity))
                    //{
                    //    Lucene.Net.Documents.Document doc = Datacontext.GetInstance().SearchedContactDocuments[teamComm.UniqueIdentity];
                    //    if (doc == null)
                    //        return;
                    //    if (doc.GetField("EmailAddress").StringValue() != null && doc.GetField("EmailAddress").StringValue() != string.Empty)
                    //    {
                    //        dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                    //        dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                    //        dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                    //        dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                    //        dictionaryValues.Add("EmailAddress", doc.GetField("EmailAddress").StringValue());
                    //    }
                    //    var fields = doc.GetFields();
                    //    bool isContactIdAvailable = false;
                    //    foreach (var item in fields)
                    //    {
                    //        if (((Lucene.Net.Documents.Field)item).Name().ToString() == "ContactId")
                    //            isContactIdAvailable = true;
                    //    }


                    //    if (isContactIdAvailable && fields != null && doc.GetField("ContactId").StringValue() != null &&
                    //                doc.GetField("ContactId").StringValue() != string.Empty)
                    //        dictionaryValues.Add("ContactId", doc.GetField("ContactId").StringValue());
                    //    else
                    //        dictionaryValues.Add("ContactId", string.Empty);
                    //    responseNotify.Invoke(dictionaryValues);
                    //    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    //    if (foundRows != null && foundRows.Count < 1)
                    //    {
                    //        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                    //        dtRow["Type"] = teamComm.SearchedType;
                    //        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                    //        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                    //        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    //    }
                    //}
                }

                else if (teamComm.ImageToolTip.Contains("Add"))
                {
                    bool isIntOrNot = false;
                    Int64 EnteredIntValue = 0;
                    isIntOrNot = Int64.TryParse(txtSearch.Text, out EnteredIntValue);

                    if (isIntOrNot)//Entered text is a number(DN)
                        Datacontext.GetInstance().IsFavoriteItem = false;
                    else
                        Datacontext.GetInstance().IsFavoriteItem = true;

                    Datacontext.GetInstance().FavoriteDisplayName = teamComm.SearchedName;
                    Datacontext.GetInstance().UniqueIdentity = teamComm.UniqueIdentity;
                    Datacontext.GetInstance().FavoriteItemSelectedType = teamComm.SearchedType.ToString();
                    (this.Parent as Grid).Children.Clear();

                    Favorites favMenu = new Favorites();
                    favMenu.ShowDialog();
                }
                else if (teamComm.ImageToolTip.Contains("Remove"))
                {
                    Pointel.Interactions.TeamCommunicator.WinForms.MessageBox showMessageBox = new Pointel.Interactions.TeamCommunicator.WinForms.MessageBox("Information",
                        "Are you sure, you want to remove this favorite item", "Yes", "No");
                    showMessageBox.ShowDialog();
                    if (showMessageBox.DialogResult == true)
                    {
                        foreach (DataRow row in Datacontext.GetInstance().dtFavorites.Rows)
                        {
                            if (row["UniqueIdentity"].ToString() == teamComm.UniqueIdentity)
                            {
                                Datacontext.GetInstance().dtFavorites.Rows.Remove(row);
                                XMLHandler handler = new XMLHandler();
                                handler.RemoveFavorite(row["UniqueIdentity"].ToString(), row["Type"].ToString());
                                txtSearch_TextChanged(null, null);
                                break;
                            }
                        }
                    }
                }
                else if (teamComm.ImageToolTip.ToLower().Contains("consult"))
                {
                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }

                    if (Datacontext.GetInstance().CurrentInteractionType != Pointel.Interactions.IPlugins.InteractionType.Voice &&
                        teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                    {
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //    teamComm.UniqueIdentity, Datacontext.GetInstance().RoutingAddress, "");
                        DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                    }
                    else
                    {
                        if (teamComm.SearchedType == Datacontext.SelectorFilters.Agent)
                        {
                            DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                        }
                        else
                        {
                            DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                            //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                            //    teamComm.UniqueIdentity, Datacontext.GetInstance().SelectedDN, "");
                        }
                    }

                }
                else if (teamComm.ImageToolTip.ToLower().Contains("call"))
                {
                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }

                    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice &&
                        teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                    {
                        if (teamComm.SearchedType == Datacontext.SelectorFilters.DN)
                        {
                            dictionaryValues.Clear();
                            dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                            dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                            dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                            dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                            dictionaryValues.Add("SelectedDN", teamComm.DN);
                            responseNotify.Invoke(dictionaryValues);
                            //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                            //    teamComm.UniqueIdentity, teamComm.DN, "");
                        }
                        else
                        {
                            DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                            //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                            //    teamComm.UniqueIdentity, Datacontext.GetInstance().RoutingAddress, "");
                        }
                    }
                    else
                    {
                        DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                        //if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                        //    _fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //        teamComm.UniqueIdentity, Datacontext.GetInstance().SelectedDN, place.ToString());
                        //else
                        //    _fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //        employeeID, Datacontext.GetInstance().SelectedDN, place.ToString());
                        //}
                    }

                }
                else if (teamComm.ImageToolTip.ToLower().Contains("queue"))
                {

                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }
                    DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                    //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                    //    teamComm.UniqueIdentity, "", "");
                }
                else if (teamComm.ImageToolTip.ToLower().Contains("workbin"))
                {
                    Datacontext.GetInstance().ActionMenu.Items.Clear();
                    foreach (string workbinItem in Datacontext.GetInstance().EmailWorkbins)
                    {
                        MenuItem mnuItem = new MenuItem();
                        mnuItem.Header = "Move to " + workbinItem + " workbin";
                        mnuItem.StaysOpenOnClick = true;
                        mnuItem.Background = System.Windows.Media.Brushes.Transparent;
                        mnuItem.Icon = new System.Windows.Controls.Image { Height = 15, Width = 15, Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative)) };
                        mnuItem.Click += new RoutedEventHandler(MnuItemMedia_Click);
                        Datacontext.GetInstance().ActionMenu.Items.Add(mnuItem);
                    }
                    Button btnTemp = sender as Button;
                    Datacontext.GetInstance().ActionMenu.PlacementTarget = btnTemp;
                    Datacontext.GetInstance().ActionMenu.IsOpen = true;
                    Datacontext.GetInstance().ActionMenu.StaysOpen = true;
                    Datacontext.GetInstance().ActionMenu.Focus();
                }

                #region Commented Need to reuse

                //else if (teamComm.SearchedType.ToLower() == "agent")
                //{
                //    string employeeID = "";
                //    string place = "";
                //    var name = (from row in Datacontext.GetInstance().dtPersons.AsEnumerable()
                //                where
                //                    row.Field<string>("UserName").ToString() == teamComm.UniqueIdentity
                //                select row.Field<string>("EmployeeID")).First<string>();
                //    if (name != null)
                //        employeeID = name;
                //    if (employeeID != null && employeeID != string.Empty)
                //    {
                //        if (Datacontext.GetInstance().hshAgentPlace.ContainsKey(employeeID))
                //            place = Datacontext.GetInstance().hshAgentPlace[employeeID].ToString();
                //        else
                //            place = "";

                //        if (Datacontext.GetInstance().hshAgentDN.ContainsKey(employeeID))
                //            Datacontext.GetInstance().SelectedDN = Datacontext.GetInstance().hshAgentDN[employeeID].ToString();
                //        else
                //            Datacontext.GetInstance().SelectedDN = "";

                //        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                //            _fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType, employeeID, Datacontext.GetInstance().SelectedDN, place.ToString());
                //        else
                //            _fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType, employeeID, Datacontext.GetInstance().SelectedDN, place.ToString());


                //        List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                //        if (foundRows.Count < 1)
                //        {
                //            DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                //            dtRow["Type"] = teamComm.SearchedType;
                //            dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                //            dtRow["DN"] = teamComm.SearchedType.ToLower().Equals("agent") ? Datacontext.GetInstance().SelectedDN : "";
                //            Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                //        }
                //    }
                //}

                #endregion
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : btnCall_Click : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the MovetoWorkbin control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void MnuItemMedia_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator teamComm = ((FrameworkElement)sender).DataContext as
                Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator;
            #region InteractionType.Voice

            if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
            {
                if (teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                {
                    if (teamComm.SearchedType == Datacontext.SelectorFilters.DN)
                    {
                        dictionaryValues.Clear();
                        dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                        dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                        dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                        dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                        dictionaryValues.Add("SelectedDN", teamComm.DN);
                        responseNotify.Invoke(dictionaryValues);
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //    teamComm.UniqueIdentity, teamComm.DN, "");
                    }
                    else
                    {
                        DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //    teamComm.UniqueIdentity, Datacontext.GetInstance().RoutingAddress, "");
                    }
                }
                else
                {
                    DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                }
                List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                if (foundRows != null && foundRows.Count < 1)
                {
                    DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                    dtRow["Type"] = teamComm.SearchedType;
                    dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                    dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                    Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                }
            }

            #endregion

            #region InteractionType.WorkBin
            else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
            {
                if (teamComm.SearchedType == Datacontext.SelectorFilters.InteractionQueue)
                {
                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }
                    DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                }

                else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin ||
                    Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Supervisor)
                {
                    string moveWorkbinValue = "";
                    MenuItem mnuItem = sender as MenuItem;
                    string workbinMenu = mnuItem.Header.ToString();
                    moveWorkbinValue = workbinMenu.Replace("Move to ", "");
                    moveWorkbinValue = moveWorkbinValue.Remove(moveWorkbinValue.Length - 8);
                    //moveWorkbinValue = workbinMenu.Replace("Move to ", "").Replace(" workbin", "");
                    if (teamComm.SearchedType == Datacontext.SelectorFilters.Agent)
                    {
                        string employeeID = "";
                        KeyValuePair<string, string> kvpInfo = new KeyValuePair<string, string>();
                        kvpInfo = Datacontext.GetInstance().hshAgentEmployeeIdUserName.Cast<DictionaryEntry>().ToDictionary(kvp => (string)kvp.Key, kvp =>
                                        (string)kvp.Value).Where(kvp => kvp.Value == teamComm.UniqueIdentity).FirstOrDefault();

                        if (kvpInfo.Key != null)
                            employeeID = kvpInfo.Key;

                        string place = "";
                        if (employeeID != null && employeeID != string.Empty)
                        {
                            if (Datacontext.GetInstance().hshAgentPlace.ContainsKey(employeeID))
                                place = Datacontext.GetInstance().hshAgentPlace[employeeID].ToString();
                            else
                                place = "";

                            if (Datacontext.GetInstance().hshAgentDN.ContainsKey(employeeID))
                                Datacontext.GetInstance().SelectedDN = Datacontext.GetInstance().hshAgentDN[employeeID].ToString();
                            else
                                Datacontext.GetInstance().SelectedDN = "";

                            List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x =>
                       x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                            if (foundRows != null && foundRows.Count < 1)
                            {
                                DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                                dtRow["Type"] = teamComm.SearchedType;
                                dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                                dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                                Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                            }
                            dictionaryValues.Clear();
                            dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                            dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                            dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                            dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                            dictionaryValues.Add("SelectedDN", Datacontext.GetInstance().SelectedDN);
                            dictionaryValues.Add("EmployeeId", employeeID);
                            dictionaryValues.Add("Place", place.ToString());
                            dictionaryValues.Add("WorkbinName", moveWorkbinValue);
                            responseNotify.Invoke(dictionaryValues);
                        }
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //    employeeID, moveWorkbinValue, place);
                    }
                    else
                    {
                        List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x =>
                       x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                        if (foundRows != null && foundRows.Count < 1)
                        {
                            DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                            dtRow["Type"] = teamComm.SearchedType;
                            dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                            dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                            Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                        }
                        dictionaryValues.Clear();
                        dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                        dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                        dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                        dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                        dictionaryValues.Add("WorkbinName", moveWorkbinValue);
                        responseNotify.Invoke(dictionaryValues);
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(),
                        //    teamComm.UniqueIdentity, moveWorkbinValue, "");
                    }


                }
                else if (Datacontext.GetInstance().SelectedOperationType == IPlugins.OperationType.Transfer)
                {
                    List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count < 1)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                        dtRow["Type"] = teamComm.SearchedType;
                        dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                        dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                        Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                    }

                    if (teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                    {
                        DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                        //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(), teamComm.UniqueIdentity,
                        //    Datacontext.GetInstance().RoutingAddress, "");
                    }
                    else
                    {
                        DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                    }

                }
            }
            #endregion

            #region InteractionType Email or Chat
            else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email ||
                Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
            {
                List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                if (foundRows != null && foundRows.Count < 1)
                {
                    DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                    dtRow["Type"] = teamComm.SearchedType;
                    dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                    dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                    Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                }

                if (teamComm.SearchedType != Datacontext.SelectorFilters.Agent)
                {
                    DialOtherInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                    //_fireDialNumber.Invoke(Datacontext.GetInstance().CurrentInteractionType, Datacontext.GetInstance().SelectedOperationType, teamComm.SearchedType.ToString(), teamComm.UniqueIdentity,
                    //    Datacontext.GetInstance().RoutingAddress, "");
                }
                else
                {
                    DialAgentInteraction(teamComm.UniqueIdentity, teamComm.SearchedType.ToString());
                }

            }

            #endregion

            #region Interaction Type Contact

            else if (Datacontext.GetInstance().CurrentInteractionType == Interactions.IPlugins.InteractionType.Contact)
            {
                dictionaryValues.Clear();
                //if (Datacontext.GetInstance().SearchedContactDocuments.ContainsKey(teamComm.UniqueIdentity))
                //{
                // Lucene.Net.Documents.Document doc = Datacontext.GetInstance().SearchedContactDocuments[teamComm.UniqueIdentity];
                if (string.IsNullOrEmpty(teamComm.EmailAddress))
                    return;
                else
                {
                    dictionaryValues.Add("InteractionType", Datacontext.GetInstance().CurrentInteractionType.ToString());
                    dictionaryValues.Add("OperationType", Datacontext.GetInstance().SelectedOperationType.ToString());
                    dictionaryValues.Add("SearchedType", teamComm.SearchedType.ToString());
                    dictionaryValues.Add("UniqueIdentity", teamComm.UniqueIdentity);
                    dictionaryValues.Add("EmailAddress", menuItem.Tag.ToString());
                }
                //var fields = doc.GetFields();
                //bool isContactIdAvailable = false;
                //foreach (var item in fields)
                //{
                //    if (((Lucene.Net.Documents.Field)item).Name().ToString() == "ContactId")
                //        isContactIdAvailable = true;
                //}

                //if (isContactIdAvailable && fields != null && doc.GetField("ContactId").StringValue() != null &&
                //            doc.GetField("ContactId").StringValue() != string.Empty)
                if (!string.IsNullOrEmpty(teamComm.UniqueIdentity) && !(teamComm.UniqueIdentity.Contains("@")))
                    dictionaryValues.Add("ContactId", teamComm.UniqueIdentity);
                else
                    dictionaryValues.Add("ContactId", string.Empty);
                List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                if (foundRows != null && foundRows.Count < 1)
                {
                    DataRow dtRow = Datacontext.GetInstance().dtRecentInteractions.NewRow();
                    dtRow["Type"] = teamComm.SearchedType;
                    dtRow["UniqueIdentity"] = teamComm.UniqueIdentity;
                    dtRow["DN"] = teamComm.SearchedType.Equals(Datacontext.SelectorFilters.Agent) ? Datacontext.GetInstance().SelectedDN : Datacontext.GetInstance().RoutingAddress;
                    Datacontext.GetInstance().dtRecentInteractions.Rows.Add(dtRow);
                }
                responseNotify.Invoke(dictionaryValues);


                //}
            }
            #endregion
        }

        /// <summary>
        /// Handles the Click event of the btnContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnContext_Click(object sender, RoutedEventArgs e)
        {
            _logger.Info("btnContext_Click - Entry");

            RemoveParent(Datacontext.GetInstance().MnuCall);
            RemoveParent(Datacontext.GetInstance().MnuAddFavorite);
            RemoveParent(Datacontext.GetInstance().MnuEditFavorite);
            RemoveParent(Datacontext.GetInstance().MnuRemoveFavorite);

            Datacontext.GetInstance().ActionMenu = new ContextMenu();
            //Datacontext.GetInstance().ActionMenu.Items.Clear();
            try
            {
                Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator teamComm = ((FrameworkElement)sender).DataContext as Pointel.Interactions.TeamCommunicator.Helpers.TeamCommunicator;
                Datacontext.GetInstance().FavoriteItemSelectedType = teamComm.SearchedType.ToString();
                string typedString = txtSearch.Text;
                Int64 EnteredIntValue = 0;
                bool IsIntOrNot = false;

                //If value is entered in textbox, check based on that
                if (typedString != string.Empty && typedString != null)
                    IsIntOrNot = Int64.TryParse(typedString, out EnteredIntValue);
                else//If value is not entered have to display all favorites and chack based on which one is clicked
                    IsIntOrNot = Int64.TryParse(teamComm.UniqueIdentity, out EnteredIntValue);

                if (IsIntOrNot)//Entered text is a number(DN)
                {
                    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                    {

                        Datacontext.GetInstance().FavoriteDisplayName = txtSearch.Text;
                        Datacontext.GetInstance().UniqueIdentity = teamComm.UniqueIdentity;

                        //Datacontext.GetInstance().ActionMenu.Items.Clear();
                        bool isFavorite = false;
                        List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == teamComm.UniqueIdentity).ToList();
                        if (foundRows != null && foundRows.Count > 0)
                            isFavorite = true;

                        Button btnTemp = sender as Button;
                        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuCall);
                        //if (isFavorite == false)
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                        //else
                        //{
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                        //}

                        Datacontext.GetInstance().ActionMenu.PlacementTarget = btnTemp;
                        Datacontext.GetInstance().ActionMenu.IsOpen = true;
                        Datacontext.GetInstance().ActionMenu.StaysOpen = true;
                        Datacontext.GetInstance().ActionMenu.Focus();
                    }
                }
                else
                {
                    Datacontext.GetInstance().FavoriteDisplayName = teamComm.SearchedName;
                    Datacontext.GetInstance().UniqueIdentity = teamComm.UniqueIdentity;

                    MenuItem mnuItemMedia = new MenuItem();
                    mnuItemMedia.StaysOpenOnClick = true;
                    mnuItemMedia.Background = System.Windows.Media.Brushes.Transparent;
                    mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);

                    bool isFavorite = false;
                    List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                        teamComm.UniqueIdentity).ToList();
                    if (foundRows != null && foundRows.Count > 0)
                        isFavorite = true;

                    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat ||
                        Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                    {
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Chat)
                        {
                            if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                mnuItemMedia.Header = "Instant Chat Transfer";
                            else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Conference)
                                mnuItemMedia.Header = "Instant Chat Conference";
                            else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Consult)
                                mnuItemMedia.Header = "Start a Consult Interaction";

                            mnuItemMedia.Icon = new System.Windows.Controls.Image
                            {
                                Height = 15,
                                Width = 15,
                                Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Chat.png", UriKind.Relative))
                            };
                        }
                        else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Email)
                        {
                            if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                                mnuItemMedia.Header = "Instant Email Transfer";
                            else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Conference)
                                mnuItemMedia.Header = "Instant Email Conference";
                            mnuItemMedia.Icon = new System.Windows.Controls.Image
                            {
                                Height = 15,
                                Width = 15,
                                Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative))
                            };
                        }

                        if (teamComm.Status == "Ready" || teamComm.Status == "NotReady" || teamComm.Status == "ConditionallyReady")
                            Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);

                        //if (isFavorite == false)
                        //        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                        //else
                        //{
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                        //}
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.WorkBin)
                    {
                        //if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Queue)
                        //{
                        //mnuItemMedia = new MenuItem();
                        //mnuItemMedia.Header = "Move to Queue";
                        //mnuItemMedia.Icon = new System.Windows.Controls.Image
                        //{
                        //    Height = 15,
                        //    Width = 15,
                        //    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative))
                        //};
                        //Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);

                        //if (isFavorite == false)
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                        //else
                        //{
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                        //}
                        //}
                        if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Supervisor)
                        {
                            if (teamComm.SearchedType == Datacontext.SelectorFilters.Agent || teamComm.SearchedType == Datacontext.SelectorFilters.AgentGroup)
                            {
                                foreach (string workbinItem in Datacontext.GetInstance().TeamWorkbins)
                                {
                                    mnuItemMedia = new MenuItem();
                                    mnuItemMedia.Header = "Move to " + workbinItem + " workbin";
                                    mnuItemMedia.Icon = new System.Windows.Controls.Image
                                    {
                                        Height = 15,
                                        Width = 15,
                                        Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative))
                                    };
                                    mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                                    Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);
                                }
                            }
                            else if (teamComm.SearchedType == Datacontext.SelectorFilters.InteractionQueue)
                            {
                                mnuItemMedia = new MenuItem();
                                mnuItemMedia.Header = "Move to Queue";
                                mnuItemMedia.Icon = new System.Windows.Controls.Image
                                {
                                    Height = 15,
                                    Width = 15,
                                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative))
                                };
                                mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                                Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);

                            }

                            //if (isFavorite == false)
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                            //else
                            //{
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                            //}

                        }

                        else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Workbin)
                        {
                            if (teamComm.SearchedType == Datacontext.SelectorFilters.Agent || teamComm.SearchedType == Datacontext.SelectorFilters.AgentGroup)
                            {
                                foreach (string workbinItem in Datacontext.GetInstance().EmailWorkbins)
                                {
                                    mnuItemMedia = new MenuItem();
                                    mnuItemMedia.Header = "Move to " + workbinItem + " workbin";
                                    mnuItemMedia.Icon = new System.Windows.Controls.Image
                                    {
                                        Height = 15,
                                        Width = 15,
                                        Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.workbin.png", UriKind.Relative))
                                    };
                                    mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                                    Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);
                                }
                            }
                            if (teamComm.SearchedType == Datacontext.SelectorFilters.InteractionQueue)
                            {
                                mnuItemMedia = new MenuItem();
                                mnuItemMedia.Header = "Move to Queue";
                                mnuItemMedia.Icon = new System.Windows.Controls.Image
                                {
                                    Height = 15,
                                    Width = 15,
                                    Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/move.to.queue.png", UriKind.Relative))
                                };
                                mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                                Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);

                                //if (isFavorite == false)
                                //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                                //else
                                //{
                                //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                                //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                                //}
                            }
                        }
                        else if (Datacontext.GetInstance().SelectedOperationType == Pointel.Interactions.IPlugins.OperationType.Transfer)
                        {
                            mnuItemMedia.Header = "Instant Email Transfer";
                            mnuItemMedia.Icon = new System.Windows.Controls.Image
                            {
                                Height = 15,
                                Width = 15,
                                Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative))
                            };
                            mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);

                            if (teamComm.Status == "Ready" || teamComm.Status == "NotReady" || teamComm.Status == "ConditionallyReady")
                                Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);

                            //if (isFavorite == false)
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                            //else
                            //{
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                            //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                            //}
                        }
                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                    {
                        //Check the status of the selected item if it is Ready or NotReady and allow to call
                        if (teamComm.Status == "Ready" || teamComm.Status == "NotReady" || teamComm.Status == "ConditionallyReady")
                            Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuCall);
                        //Check the favorite status of the selected item if it is not favorite, allow to add favorite
                        //if (isFavorite == false)
                        //        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                        //else
                        //{
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                        //    Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                        //}

                    }
                    else if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact)
                    {
                        if (!string.IsNullOrEmpty(teamComm.EmailAddress))
                        {
                            foreach (string emailID in teamComm.EmailAddress.Split(','))
                            {
                                if (!string.IsNullOrEmpty(emailID.Trim()))
                                {
                                    mnuItemMedia = new MenuItem();
                                    mnuItemMedia.Header = "Add e-mail address (" + emailID + ")";
                                    mnuItemMedia.Tag = emailID;
                                    mnuItemMedia.Icon = new System.Windows.Controls.Image
                                    {
                                        Height = 15,
                                        Width = 15,
                                        Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative))
                                    };
                                    mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                                    Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);
                                }
                            }
                        }
                        if (teamComm.UniqueIdentity.Contains("@"))
                            goto End;
                        #region oldcode
                        //if (Datacontext.GetInstance().SearchedContactDocuments.ContainsKey(teamComm.UniqueIdentity))
                        //{
                        //    Lucene.Net.Documents.Document doc = Datacontext.GetInstance().SearchedContactDocuments[teamComm.UniqueIdentity];
                        //    if (doc == null)
                        //        return;
                        //    if (doc.GetField("EmailAddress").StringValue() != null && doc.GetField("EmailAddress").StringValue() != string.Empty)
                        //    {
                        //        mnuItemMedia = new MenuItem();
                        //        mnuItemMedia.Header = "Add e-mail address (" + doc.GetField("EmailAddress").StringValue() + ")";
                        //        mnuItemMedia.Icon = new System.Windows.Controls.Image
                        //        {
                        //            Height = 15,
                        //            Width = 15,
                        //            Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Email.png", UriKind.Relative))
                        //        };
                        //        mnuItemMedia.Click += new RoutedEventHandler(MnuItemMedia_Click);
                        //        Datacontext.GetInstance().ActionMenu.Items.Add(mnuItemMedia);
                        //    }
                        //}
                        #endregion
                    }

                    if (isFavorite == false)
                    {
                        Datacontext.GetInstance().MnuAddFavorite = new MenuItem();
                        Datacontext.GetInstance().MnuAddFavorite.StaysOpenOnClick = true;
                        Datacontext.GetInstance().MnuAddFavorite.Background = System.Windows.Media.Brushes.Transparent;
                        Datacontext.GetInstance().MnuAddFavorite.Header = "Add to favorite";
                        Datacontext.GetInstance().MnuAddFavorite.Icon = new System.Windows.Controls.Image
                        {
                            Height = 15,
                            Width = 15,
                            Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative))
                        };
                        Datacontext.GetInstance().MnuAddFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);
                        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuAddFavorite);
                    }
                    else
                    {
                        Datacontext.GetInstance().MnuRemoveFavorite = new MenuItem();
                        Datacontext.GetInstance().MnuRemoveFavorite.StaysOpenOnClick = true;
                        Datacontext.GetInstance().MnuRemoveFavorite.Background = System.Windows.Media.Brushes.Transparent;
                        Datacontext.GetInstance().MnuRemoveFavorite.Header = "Remove from Favorites";
                        Datacontext.GetInstance().MnuRemoveFavorite.Icon = new System.Windows.Controls.Image
                        {
                            Height = 15,
                            Width = 15,
                            Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Remove.png", UriKind.Relative))
                        };


                        Datacontext.GetInstance().MnuEditFavorite = new MenuItem();
                        Datacontext.GetInstance().MnuEditFavorite.StaysOpenOnClick = true;
                        Datacontext.GetInstance().MnuEditFavorite.Background = System.Windows.Media.Brushes.Transparent;
                        Datacontext.GetInstance().MnuEditFavorite.Header = "Edit favorite";
                        Datacontext.GetInstance().MnuEditFavorite.Icon = new System.Windows.Controls.Image
                        {
                            Height = 15,
                            Width = 15,
                            Source = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.Edit.png", UriKind.Relative))
                        };
                        Datacontext.GetInstance().MnuRemoveFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);
                        Datacontext.GetInstance().MnuEditFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);

                        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuEditFavorite);
                        Datacontext.GetInstance().ActionMenu.Items.Add(Datacontext.GetInstance().MnuRemoveFavorite);
                    }
                End:
                    Button btnTemp = sender as Button;
                    Datacontext.GetInstance().ActionMenu.PlacementTarget = btnTemp;
                    Datacontext.GetInstance().ActionMenu.IsOpen = true;
                    Datacontext.GetInstance().ActionMenu.StaysOpen = true;
                    Datacontext.GetInstance().ActionMenu.Focus();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : btnContext_Click : " + generalException.Message.ToString());
            }
            _logger.Info("btnContext_Click - Exit");
        }

        private void RemoveParent(MenuItem mnuItem)
        {
            if (Datacontext.GetInstance().ActionMenu == null)
                return;
            List<MenuItem> mnuLists = new List<MenuItem>(Datacontext.GetInstance().ActionMenu.Items.Cast<MenuItem>().ToList());
            if (mnuLists != null && mnuLists.Count > 0)
            {
                foreach (var item in mnuLists)
                {
                    if (Datacontext.GetInstance().ActionMenu.Items.Contains(item))
                        Datacontext.GetInstance().ActionMenu.Items.Remove(item);
                }
                mnuLists = null;
                Datacontext.GetInstance().ActionMenu = null;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;
                if (Datacontext.GetInstance().TeamCommunicator != null)
                    Datacontext.GetInstance().TeamCommunicator.Clear();
                Datacontext.GetInstance().InternalTargets = "";

                #region TextBox not empty

                if (txtSearch.Text != string.Empty)
                {
                    string typedString = txtSearch.Text.ToLower();
                    if (Datacontext.GetInstance().TeamCommunicator != null)
                        Datacontext.GetInstance().TeamCommunicator.Clear();
                    Int64 EnteredIntValue = 0;
                    bool isIntOrNot = false;
                    isIntOrNot = Int64.TryParse(typedString, out EnteredIntValue);

                    if (isIntOrNot)//Entered text is a number(DN)
                    {
                        string favToolTip = "";
                        favImageSource = null;
                        bool isFav = false;
                        List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() == typedString).ToList();
                        if (foundRows != null)
                        {
                            foreach (DataRow dr in foundRows)
                            {
                                isFav = true;
                                favImageSource = new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Favourite.png", UriKind.Relative));
                                favToolTip = dr["DisplayName"].ToString();
                                break;
                            }
                        }
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Voice)
                        {
                            Datacontext.GetInstance().TeamCommunicator.Add(new Pointel.Interactions.TeamCommunicator.Helpers.
                                TeamCommunicator("Establish a new Phone Call",
                            new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Voice.Short.png", UriKind.Relative)), typedString,
                                null, Datacontext.GetInstance().CurrentMediaImageSource, Datacontext.GetInstance().CurrentInteractionType, "", new BitmapImage(new Uri("/Agent.Interaction.Desktop;component/Images/Agent.png", UriKind.Relative)), typedString, Datacontext.SelectorFilters.DN,
                                typedString, typedString, favImageSource, favToolTip));
                            if (isFav)
                                Datacontext.GetInstance().InternalTargets = "1 " + "Matching Internal Targets";
                            else
                                Datacontext.GetInstance().InternalTargets = "0 " + "Matching Internal Targets";
                        }
                        else
                        {
                            SearchTextResults(typedString);
                        }
                    }
                    else //If the entered value is not numeral(ie DN)
                    {
                        SearchTextResults(typedString);
                    }
                }

                #endregion

                #region TextBox Empty

                else
                {
                    Datacontext.GetInstance().InternalTargets = "";

                    if (Datacontext.GetInstance().TeamCommunicator != null)
                        Datacontext.GetInstance().TeamCommunicator.Clear();

                    #region isSelectAllClicked
                    if (isSelectAllClicked)
                    {

                    }
                    #endregion

                    #region isFavoriteClicked
                    else if (isFavoriteClicked)
                    {
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact
                         && Datacontext.GetInstance().dtFavorites != null
                         && Datacontext.GetInstance().dtFavorites.Rows.Count > 0)
                        {
                            var listContactFavourites = Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(x => x["Type"].ToString() == "Contact").ToList<DataRow>();
                            string[] contactIDs = new string[listContactFavourites.Count];
                            for (int i = 0; i < listContactFavourites.Count; i++)
                            {
                                contactIDs[i] = listContactFavourites[i]["UniqueIdentity"].ToString();
                            }
                            parser.GetSearchResults("*", Datacontext.GetInstance().SelectedType, contactIDs);
                        }
                        else
                            parser.GetSearchResults("*", Datacontext.GetInstance().SelectedType);

                        ObservableCollection<ITeamCommunicator> temp = new ObservableCollection<ITeamCommunicator>(Datacontext.GetInstance().TeamCommunicator);
                        temp.Clear();
                        foreach (var data in Datacontext.GetInstance().TeamCommunicator)
                        {
                            List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                data.UniqueIdentity).ToList();
                            if (foundRows != null && foundRows.Count > 0)
                                temp.Add(data);
                        }


                        Datacontext.GetInstance().TeamCommunicator.Clear();
                        Datacontext.GetInstance().TeamCommunicator = temp;// as ObservableCollection<ITeamCommunicator>;
                        DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;

                        Datacontext.GetInstance().InternalTargets = Datacontext.GetInstance().TeamCommunicator.Count.ToString() + " " + "Matching Internal Targets";
                        GroupResults();

                    }
                    #endregion

                    #region isRecentClicked
                    else if (isRecentClicked)
                    {
                        if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact
                        && Datacontext.GetInstance().dtRecentInteractions != null
                        && Datacontext.GetInstance().dtRecentInteractions.Rows.Count > 0)
                        {
                            var listContactRecentIxns = Datacontext.GetInstance().dtRecentInteractions.AsEnumerable().Where(x => x["Type"].ToString() == "Contact").ToList<DataRow>();
                            string[] contactIDs = new string[listContactRecentIxns.Count];
                            for (int i = 0; i < listContactRecentIxns.Count; i++)
                            {
                                contactIDs[i] = listContactRecentIxns[i]["UniqueIdentity"].ToString();
                            }
                            parser.GetSearchResults("*", Datacontext.GetInstance().SelectedType, contactIDs);
                        }
                        else
                            parser.GetSearchResults("*", Datacontext.GetInstance().SelectedType);

                        ObservableCollection<ITeamCommunicator> temp = new ObservableCollection<ITeamCommunicator>(Datacontext.GetInstance().TeamCommunicator);
                        temp.Clear();
                        foreach (var data in Datacontext.GetInstance().TeamCommunicator)
                        {
                            List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                                data.UniqueIdentity).ToList();
                            if (foundRows != null && foundRows.Count > 0)
                                temp.Add(data);
                        }
                        Datacontext.GetInstance().InternalTargets = temp.Count + "+  " + "Matching Internal Targets";

                        Datacontext.GetInstance().TeamCommunicator.Clear();
                        Datacontext.GetInstance().TeamCommunicator = temp;// as ObservableCollection<ITeamCommunicator>;
                        DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;

                        Datacontext.GetInstance().InternalTargets = Datacontext.GetInstance().TeamCommunicator.Count.ToString() + " " + "Matching Internal Targets";
                        GroupResults();

                    }
                    #endregion

                }

                #endregion

            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : txtSearch_TextChanged : " + generalException.Message.ToString());
            }
        }

        private void SearchTextResults(string typedString)
        {
            try
            {
                #region isSelectAllClicked
                if (isSelectAllClicked)
                {

                    parser.GetSearchResults(typedString, Datacontext.GetInstance().SelectedType);

                    #region Display Matching Contacts and Internal targets
                    if (Datacontext.GetInstance().CurrentInteractionType != Pointel.Interactions.IPlugins.InteractionType.Contact)
                        Datacontext.GetInstance().InternalTargets = Datacontext.GetInstance().TeamCommunicator.Count.ToString() + " " + "Matching Internal Targets";
                    #endregion
                }
                #endregion

                #region isRecentClicked
                else if (isRecentClicked)
                {
                    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact
                        && Datacontext.GetInstance().dtRecentInteractions != null
                        && Datacontext.GetInstance().dtRecentInteractions.Rows.Count > 0)
                    {
                        var listContactRecentIxns = Datacontext.GetInstance().dtRecentInteractions.AsEnumerable().Where(x => x["Type"].ToString() == "Contact").ToList<DataRow>();
                        string[] contactIDs = new string[listContactRecentIxns.Count];
                        for (int i = 0; i < listContactRecentIxns.Count; i++)
                        {
                            contactIDs[i] = listContactRecentIxns[i]["UniqueIdentity"].ToString();
                        }
                        parser.GetSearchResults(typedString, Datacontext.GetInstance().SelectedType, contactIDs);
                    }
                    else
                        parser.GetSearchResults(typedString, Datacontext.GetInstance().SelectedType);

                    ObservableCollection<ITeamCommunicator> temp = new ObservableCollection<ITeamCommunicator>(Datacontext.GetInstance().TeamCommunicator);
                    temp.Clear();

                    foreach (var data in Datacontext.GetInstance().TeamCommunicator)
                    {

                        List<DataRow> foundRows = Datacontext.GetInstance().dtRecentInteractions.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                        data.UniqueIdentity).ToList();
                        if (foundRows != null && foundRows.Count > 0)
                            temp.Add(data);
                    }

                    Datacontext.GetInstance().TeamCommunicator.Clear();
                    Datacontext.GetInstance().TeamCommunicator = temp;// as ObservableCollection<ITeamCommunicator>;
                    DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;

                    Datacontext.GetInstance().InternalTargets = Datacontext.GetInstance().TeamCommunicator.Count.ToString() + " " + "Matching Internal Targets";
                }
                #endregion

                #region isFavoriteClicked
                else if (isFavoriteClicked)
                {
                    if (Datacontext.GetInstance().CurrentInteractionType == Pointel.Interactions.IPlugins.InteractionType.Contact
                          && Datacontext.GetInstance().dtFavorites != null
                          && Datacontext.GetInstance().dtFavorites.Rows.Count > 0)
                    {
                        var listContactFavourites = Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(x => x["Type"].ToString() == "Contact").ToList<DataRow>();
                        string[] contactIDs = new string[listContactFavourites.Count];
                        for (int i = 0; i < listContactFavourites.Count; i++)
                        {
                            contactIDs[i] = listContactFavourites[i]["UniqueIdentity"].ToString();
                        }
                        parser.GetSearchResults(typedString, Datacontext.GetInstance().SelectedType, contactIDs);
                    }
                    else
                        parser.GetSearchResults(typedString, Datacontext.GetInstance().SelectedType);

                    ObservableCollection<ITeamCommunicator> temp = new ObservableCollection<ITeamCommunicator>(Datacontext.GetInstance().TeamCommunicator);
                    temp.Clear();

                    foreach (var data in Datacontext.GetInstance().TeamCommunicator)
                    {
                        List<DataRow> foundRows = Datacontext.GetInstance().dtFavorites.Rows.Cast<DataRow>().Where(x => x["UniqueIdentity"].ToString() ==
                        data.UniqueIdentity).ToList();
                        if (foundRows != null && foundRows.Count > 0)
                            temp.Add(data);
                    }

                    Datacontext.GetInstance().TeamCommunicator.Clear();
                    Datacontext.GetInstance().TeamCommunicator = temp;// as ObservableCollection<ITeamCommunicator>;
                    DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;

                    int internalTargets = 0;
                    internalTargets = Datacontext.GetInstance().TeamCommunicator.Count;
                    Datacontext.GetInstance().InternalTargets = internalTargets.ToString() + " " + "Matching Internal Targets";
                }
                #endregion

                GroupResults();
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        private void GroupResults()
        {
            try
            {
                if (isGroupEnabled)//Check grouping enabled or not
                {
                    int maxSize = 10;
                    //If grouping is enabled
                    if (Datacontext.GetInstance().SelectedType == Datacontext.SelectorFilters.AllTypes)
                    {
                        if (isFavoriteClicked)
                            maxSize = Datacontext.GetInstance().MaxFavouriteSize * 2;
                        else if (isRecentClicked)
                            maxSize = Datacontext.GetInstance().RecentMaxRecords * 2;
                        else
                            maxSize = Datacontext.GetInstance().MaxSuggestionSize * 2;

                        ObservableCollection<ITeamCommunicator> team = new ObservableCollection<ITeamCommunicator>((from i in DGTeamCommunicator.ItemsSource
                                as ObservableCollection<ITeamCommunicator>
                                                                                                                    orderby i.SearchedName
                                                                                                                    select i).Take<ITeamCommunicator>(maxSize));
                        ListCollectionView collection = new ListCollectionView(team);
                        collection.GroupDescriptions.Add(new PropertyGroupDescription("SearchedType"));
                        DGTeamCommunicator.ItemsSource = collection;
                    }
                    else
                    {
                        if (isFavoriteClicked)
                            maxSize = Datacontext.GetInstance().MaxFavouriteSize;
                        else if (isRecentClicked)
                            maxSize = Datacontext.GetInstance().RecentMaxRecords;
                        else
                            maxSize = Datacontext.GetInstance().MaxSuggestionSize;

                        ObservableCollection<ITeamCommunicator> team = new ObservableCollection<ITeamCommunicator>((from i in DGTeamCommunicator.ItemsSource
                                as ObservableCollection<ITeamCommunicator>
                                                                                                                    orderby i.SearchedName
                                                                                                                    select i).Take<ITeamCommunicator>(maxSize));

                        if (Datacontext.GetInstance().SelectedType == Datacontext.SelectorFilters.Contact && (isFavoriteClicked || isRecentClicked))
                            team = new ObservableCollection<ITeamCommunicator>((from i in DGTeamCommunicator.ItemsSource
                                   as ObservableCollection<ITeamCommunicator>
                                                                                orderby i.SearchedName
                                                                                select i).Take<ITeamCommunicator>(Datacontext.GetInstance().TeamCommunicator.Count));
                        ListCollectionView collection = new ListCollectionView(team);
                        collection.GroupDescriptions.Add(new PropertyGroupDescription("SearchedType"));
                        DGTeamCommunicator.ItemsSource = collection;
                    }
                }
                else//Grouping is not enabled 
                {
                    int maxSize = 10;
                    if (Datacontext.GetInstance().SelectedType == Datacontext.SelectorFilters.AllTypes)
                    {
                        if (isFavoriteClicked)
                            maxSize = Datacontext.GetInstance().MaxFavouriteSize * 2;
                        else if (isRecentClicked)
                            maxSize = Datacontext.GetInstance().RecentMaxRecords * 2;
                        else
                            maxSize = Datacontext.GetInstance().MaxSuggestionSize * 2;
                    }
                    else
                    {
                        //if (Datacontext.GetInstance().SelectedType == Datacontext.SelectorFilters.Contact && (isFavoriteClicked || isRecentClicked))
                        //    return;
                        if (isFavoriteClicked)
                            maxSize = Datacontext.GetInstance().MaxFavouriteSize;
                        else if (isRecentClicked)
                            maxSize = Datacontext.GetInstance().RecentMaxRecords;
                        else
                            maxSize = Datacontext.GetInstance().MaxSuggestionSize;
                    }
                    Datacontext.GetInstance().TeamCommunicator = new ObservableCollection<ITeamCommunicator>((from i in Datacontext.GetInstance().TeamCommunicator
                                                                                                              orderby i.SearchedName
                                                                                                              select i).Take<ITeamCommunicator>(maxSize));
                    DGTeamCommunicator.ItemsSource = Datacontext.GetInstance().TeamCommunicator;
                }
            }
            catch (Exception ex)
            {
                _logger.Error((ex.InnerException == null) ? ex.Message : ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// Listener__statuses the updation.
        /// </summary>
        public void Listener__statusUpdation()
        {
            //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            //{
            try
            {
                //if (txtSearch.Text != string.Empty)
                {
                    if (Datacontext.GetInstance().IsStatAlive)
                        txtSearch_TextChanged(null, null);

                    GroupResults();

                }
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : Listener__statusUpdation() : " + generalException.Message.ToString());
            }
            // }));
        }

        private void TeamCommunicator_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Datacontext.GetInstance().TeamCommunicator.Clear();
                Listener._statusUpdation -= new Listener.StatusUpdation(Listener__statusUpdation);
                Datacontext.GetInstance().MnuCall.Click -= new RoutedEventHandler(mnuItem_Click);
                Datacontext.GetInstance().MnuAddFavorite.Click -= new RoutedEventHandler(mnuFavorite_Click);
                Datacontext.GetInstance().MnuRemoveFavorite.Click -= new RoutedEventHandler(mnuFavorite_Click);
                Datacontext.GetInstance().MnuEditFavorite.Click -= new RoutedEventHandler(mnuFavorite_Click);

                //RemoveParent(Datacontext.GetInstance().MnuCall);
                //RemoveParent(Datacontext.GetInstance().MnuAddFavorite);
                //RemoveParent(Datacontext.GetInstance().MnuEditFavorite);
                //RemoveParent(Datacontext.GetInstance().MnuRemoveFavorite);
                //CloseStatServer();
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : TeamCommunicator_Unloaded() : " + generalException.Message.ToString());
            }
        }

        private void TeamCommunicator_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Datacontext.GetInstance().TeamCommunicator != null)
                    Datacontext.GetInstance().TeamCommunicator.Clear();


                Datacontext.GetInstance().MnuCall.Click += new RoutedEventHandler(mnuItem_Click);
                //Datacontext.GetInstance().MnuAddFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);
                //Datacontext.GetInstance().MnuRemoveFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);
                //Datacontext.GetInstance().MnuEditFavorite.Click += new RoutedEventHandler(mnuFavorite_Click);
                LoadSearchParameters();
                txtSearch.Clear();
                if (Datacontext.GetInstance().CurrentInteractionType == IPlugins.InteractionType.Contact)
                    txtErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
                else
                {
                    if (Datacontext.GetInstance().IsStatAlive)
                    {
                        RequestTeamCommunicatorStatistics();
                        txtErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        if (!ConnectStatServer())
                        {
                            txtSearch.IsEnabled = false;
                            ToolGrid.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            RequestTeamCommunicatorStatistics();
                            txtErrorMessage.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
                txtSearch.Focus();
            }
            catch (Exception generalException)
            {
                _logger.Error("DialPad Class : TeamCommunicator_Loaded() : " + generalException.Message.ToString());
            }
        }

        public bool ConnectStatServer()
        {
            bool isStatServerOpened = false;
            try
            {
                StatisticsConnectionManager _statisticsConnectionManager = new StatisticsConnectionManager();
                string primaryHost = string.Empty;
                string primaryPort = string.Empty;
                string secondaryHost = string.Empty;
                string secondaryPort = string.Empty;
                int addpServerTimeOut = 30;
                int addClientTimeOut = 60;
                int warmStandbyTimeOut = 10;
                short warmStandbyAttempts = 5;

                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-host"))//primary Host -----------------------------------------------------------
                    primaryHost = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-host");
                if (ConfigContainer.Instance().AllKeys.Contains("teamcommunicator.statistics-port"))//primary port -----------------------------------------------------------
                    primaryPort = (string)ConfigContainer.Instance().GetValue("teamcommunicator.statistics-port");
                //if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary Host -----------------------------------------------------------
                //    secondaryHost = (string)ConfigContainer.Instance().GetValue("");
                //if (ConfigContainer.Instance().AllKeys.Contains(""))//secondary port -----------------------------------------------------------
                //    secondaryPort = (string)ConfigContainer.Instance().GetValue("");

                if (string.IsNullOrEmpty(secondaryHost))
                    secondaryHost = primaryHost;
                if (string.IsNullOrEmpty(secondaryPort))
                    secondaryPort = primaryPort;

                if (!string.IsNullOrEmpty(primaryHost) && !string.IsNullOrEmpty(primaryPort))
                    isStatServerOpened = _statisticsConnectionManager.ConnectStatServer(primaryHost, primaryPort, secondaryHost, secondaryPort,
                                addpServerTimeOut, addClientTimeOut, warmStandbyTimeOut, warmStandbyAttempts);

                if (isStatServerOpened)
                {
                    _logger.Debug("Stat server connection opened");
                    Listener _listener = new Listener();
                    _listener.RequestAgentGroupStatistics();
                }
                else
                {
                    _logger.Warn("Stat server connection is not opened");
                }
                Datacontext.GetInstance().AllPersons = null;
            }
            catch (Exception ex)
            {
                _logger.Error("ConnectStatServer() : " + ex.Message.ToString());
            }
            return isStatServerOpened;
        }
    }
}

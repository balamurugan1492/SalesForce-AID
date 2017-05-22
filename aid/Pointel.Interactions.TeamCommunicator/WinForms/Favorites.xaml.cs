/*
* =====================================
* Pointel.Interactions.TeamCommunicator.WinForms
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pointel.Interactions.TeamCommunicator.AppReader;
using Pointel.Interactions.TeamCommunicator.Settings;


namespace Pointel.Interactions.TeamCommunicator.WinForms
{
    /// <summary>
    /// Interaction logic for Favorites.xaml
    /// </summary>
    public partial class Favorites : Window
    {
        private List<string> internalAgentFavoriteList = new List<string>();
        private List<string> internalAgentGroupFavoriteList = new List<string>();
        private List<string> internalSkillFavoriteList = new List<string>();
        private List<string> customFavoriteList = new List<string>();

        XMLHandler xmlHandler = new XMLHandler();

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");

        /// <summary>
        /// Initializes a new instance of the <see cref="Favorites"/> class.
        /// </summary>
        public Favorites()
        {
            try
            {

                InitializeComponent();
                this.DataContext = Datacontext.GetInstance();
                InitializeEmptyListFields();
                InitializeUI();

            }
            catch (Exception generalException)
            {
                _logger.Error("Favorites : Favorites() Error : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Initializes the empty list fields.
        /// </summary>
        private void InitializeEmptyListFields()
        {
            internalAgentFavoriteList.Add("Category");
            internalAgentFavoriteList.Add("ContactType");
            internalAgentFavoriteList.Add("DisplayName");
            internalAgentFavoriteList.Add("DisplayCategory");
            internalAgentFavoriteList.Add("EmailAddress");
            internalAgentFavoriteList.Add("EmployeeId");
            internalAgentFavoriteList.Add("FirstName");
            internalAgentFavoriteList.Add("LastName");
            internalAgentFavoriteList.Add("LevelDbId");
            internalAgentFavoriteList.Add("LevelType");
            internalAgentFavoriteList.Add("PhoneNumber");
            internalAgentFavoriteList.Add("TenantName");
            internalAgentFavoriteList.Add("UserName");

            internalAgentGroupFavoriteList.Add("Category");
            internalAgentGroupFavoriteList.Add("DisplayName");
            //internalAgentGroupFavoriteList.Add("DisplayCategory");
            //internalAgentGroupFavoriteList.Add("Id");
            internalAgentGroupFavoriteList.Add("LevelDbId");
            internalAgentGroupFavoriteList.Add("LevelType");
            internalAgentGroupFavoriteList.Add("Name");
            internalAgentGroupFavoriteList.Add("Number");
            internalAgentGroupFavoriteList.Add("TenantName");

            internalSkillFavoriteList.Add("Category");
            internalSkillFavoriteList.Add("DisplayName");
            //internalSkillFavoriteList.Add("DisplayCategory");
            //internalSkillFavoriteList.Add("Id");
            internalSkillFavoriteList.Add("LevelDbId");
            internalSkillFavoriteList.Add("LevelType");
            internalSkillFavoriteList.Add("Name");
            internalSkillFavoriteList.Add("Number");

            customFavoriteList.Add("Category");
            customFavoriteList.Add("DisplayName");
            //customFavoriteList.Add("DisplayCategory");
            customFavoriteList.Add("EmailAddress");
            customFavoriteList.Add("FirstName");
            //customFavoriteList.Add("Id");
            customFavoriteList.Add("LastName");
            customFavoriteList.Add("LevelDbId");
            customFavoriteList.Add("LevelType");
            customFavoriteList.Add("PhoneNumber");
        }

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        private void InitializeUI()
        {
            try
            {

                #region Edit Favorite
                if (Datacontext.GetInstance().IsEditFavorite)
                {
                    lblTitle.Content = "Edit Favorites";
                    #region Editing Favorite Type - Internal Item
                    if (Datacontext.GetInstance().IsFavoriteItem)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                        foreach (DataRow row in Datacontext.GetInstance().dtFavorites.Rows)
                        {
                            if (row["UniqueIdentity"].ToString() == Datacontext.GetInstance().UniqueIdentity)
                            {
                                dtRow = row;
                                break;
                            }
                        }
                        #region Internal Favorite List is Set
                        if (Datacontext.GetInstance().InternalFavoriteList.Count > 0)
                        {
                            foreach (string value in Datacontext.GetInstance().InternalFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in Datacontext.GetInstance().InternalFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    ComboBox cmbCategory = new ComboBox();
                                    tempGrid.Children.Add(lbl);
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.Width = 175;
                                    cmbCategory.Height = 25;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.SelectedValue = dtRow["Category"].ToString();
                                    cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    Label lblDisplayName = new Label();
                                    tempGrid.Children.Add(lbl);
                                    lblDisplayName.Name = "lblDisplayName";
                                    //lblDisplayName.Height = 25;
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    lblDisplayName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("emailaddress"))
                                    {
                                        Label lblEmailAddress = new Label();
                                        tempGrid.Children.Add(lbl);
                                        lblEmailAddress.Name = "lblEmailAddress";
                                        //lblEmailAddress.Height = 25;
                                        lblEmailAddress.Margin = new Thickness(5);
                                        lblEmailAddress.Content = dtRow["EmailAddress"].ToString();
                                        tempGrid.Children.Add(lblEmailAddress);
                                        Grid.SetRow(lblEmailAddress, 0);
                                        Grid.SetColumn(lblEmailAddress, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("firstname"))
                                    {
                                        Label lblFirstName = new Label();
                                        tempGrid.Children.Add(lbl);
                                        lblFirstName.Name = "lblFirstName";
                                        //lblFirstName.Height = 25;
                                        lblFirstName.Content = dtRow["FirstName"].ToString();
                                        lblFirstName.Margin = new Thickness(5);
                                        tempGrid.Children.Add(lblFirstName);
                                        Grid.SetRow(lblFirstName, 0);
                                        Grid.SetColumn(lblFirstName, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("lastname"))
                                    {
                                        Label lblLastName = new Label();
                                        tempGrid.Children.Add(lbl);
                                        lblLastName.Name = "lblLastName";
                                        //lblLastName.Height = 25;
                                        lblLastName.Content = dtRow["LastName"].ToString();
                                        lblLastName.Margin = new Thickness(5);
                                        tempGrid.Children.Add(lblLastName);
                                        Grid.SetRow(lblLastName, 0);
                                        Grid.SetColumn(lblLastName, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("phonenumber"))
                                    {
                                        Label lblPhoneNumber = new Label();
                                        tempGrid.Children.Add(lbl);
                                        // lblPhoneNumber.Height = 25;
                                        lblPhoneNumber.Name = "txtPhoneNumber";
                                        lblPhoneNumber.Content = dtRow["PhoneNumber"].ToString();
                                        lblPhoneNumber.Margin = new Thickness(5);
                                        tempGrid.Children.Add(lblPhoneNumber);
                                        Grid.SetRow(lblPhoneNumber, 0);
                                        Grid.SetColumn(lblPhoneNumber, 1);
                                    }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }
                            InitializeButtons(index);
                        }
                        #endregion Internal Favorite List is Set

                        #region Internal Favorite List is Not Set
                        else
                        {
                            LoadInternalEmptyListUI();
                            ComboBox cmbCategory = (ComboBox)LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                            cmbCategory.SelectedValue = dtRow["Category"].ToString();
                            cmbCategory.Height = 25;
                            //foreach (DataRow row in Datacontext.GetInstance().dtFavorites.Rows)
                            //{
                            //    if (row["UniqueIdentity"].ToString() == Datacontext.GetInstance().UniqueIdentity)
                            //    {
                            //        if (row["Category"].ToString() != null && row["Category"].ToString() != string.Empty)
                            //        {
                            //            ComboBox cmbCategory = FindChild<ComboBox>(Application.Current.MainWindow, "cmbCategory");
                            //            cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                            //            cmbCategory.SelectedValue = row["Category"].ToString();
                            //        }
                            //    }
                            //}
                        }
                        #endregion Internal Favorite List is Not Set
                    }
                    #endregion Editing Favorite Type - Internal Item

                    #region Editing Favorite  Type - DN
                    else
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                        foreach (DataRow row in Datacontext.GetInstance().dtFavorites.Rows)
                        {
                            if (row["UniqueIdentity"].ToString() == Datacontext.GetInstance().UniqueIdentity)
                            {
                                dtRow = row;
                                break;
                            }
                        }
                        #region Custom Favorite Field List is Set
                        if (Datacontext.GetInstance().CustomFavoriteList.Count > 0)
                        {
                            foreach (string value in Datacontext.GetInstance().CustomFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in Datacontext.GetInstance().CustomFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                tempGrid.Children.Add(lbl);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    ComboBox cmbCategory = new ComboBox();
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.Width = 25;
                                    cmbCategory.Width = 175;
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    var cat = from dt in Datacontext.GetInstance().dtFavorites.AsEnumerable()
                                              where dt.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity
                                              select dt.Field<string>("Category");
                                    cmbCategory.SelectedValue = dtRow["Category"].ToString();
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    Label lblDisplayName = new Label();
                                    lblDisplayName.Name = "lblDisplayName";
                                    //lblDisplayName.Height = 25;
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    lblDisplayName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (value.ToLower().Contains("emailaddress"))
                                {
                                    TextBox txtEmailAddress = new TextBox();
                                    txtEmailAddress.Name = "txtEmailAddress";
                                    txtEmailAddress.Height = 25;
                                    txtEmailAddress.Text = dtRow["EmailAddress"].ToString();
                                    txtEmailAddress.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtEmailAddress);
                                    Grid.SetRow(txtEmailAddress, 0);
                                    Grid.SetColumn(txtEmailAddress, 1);
                                }
                                if (value.ToLower().Contains("firstname"))
                                {
                                    TextBox txtFirstName = new TextBox();
                                    txtFirstName.Name = "txtFirstName";
                                    txtFirstName.Height = 25;
                                    txtFirstName.Text = dtRow["FirstName"].ToString();
                                    txtFirstName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtFirstName);
                                    Grid.SetRow(txtFirstName, 0);
                                    Grid.SetColumn(txtFirstName, 1);
                                }
                                if (value.ToLower().Contains("lastname"))
                                {
                                    TextBox txtLastName = new TextBox();
                                    txtLastName.Name = "txtLastName";
                                    txtLastName.Height = 25;
                                    txtLastName.Text = dtRow["LastName"].ToString();
                                    txtLastName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtLastName);
                                    Grid.SetRow(txtLastName, 0);
                                    Grid.SetColumn(txtLastName, 1);
                                }
                                if (value.ToLower().Contains("phonenumber"))
                                {
                                    TextBox txtPhoneNumber = new TextBox();
                                    txtPhoneNumber.Name = "txtPhoneNumber";
                                    txtPhoneNumber.Height = 25;
                                    txtPhoneNumber.Text = dtRow["PhoneNumber"].ToString();
                                    txtPhoneNumber.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtPhoneNumber);
                                    Grid.SetRow(txtPhoneNumber, 0);
                                    Grid.SetColumn(txtPhoneNumber, 1);
                                }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }
                            InitializeButtons(index);
                        }
                        #endregion Custom Favorite Field List is Set

                        #region Custom Favorite Field List is not Set
                        else
                        {
                            foreach (string value in customFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in customFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                tempGrid.Children.Add(lbl);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    ComboBox cmbCategory = new ComboBox();
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.Height = 25;
                                    cmbCategory.Width = 175;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.SelectedValue = dtRow["Category"].ToString();
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    Label lblDisplayName = new Label();
                                    lblDisplayName.Name = "lblDisplayName";
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    lblDisplayName.Margin = new Thickness(5);
                                    //lblDisplayName.Height = 25;
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (value.ToLower().Contains("emailaddress"))
                                {
                                    TextBox txtEmailAddress = new TextBox();
                                    txtEmailAddress.Name = "txtEmailAddress";
                                    txtEmailAddress.Height = 25;
                                    txtEmailAddress.Text = dtRow["EmailAddress"].ToString();
                                    txtEmailAddress.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtEmailAddress);
                                    Grid.SetRow(txtEmailAddress, 0);
                                    Grid.SetColumn(txtEmailAddress, 1);
                                }
                                if (value.ToLower().Contains("firstname"))
                                {
                                    TextBox txtFirstName = new TextBox();
                                    txtFirstName.Name = "txtFirstName";
                                    txtFirstName.Height = 25;
                                    txtFirstName.Text = dtRow["FirstName"].ToString();
                                    txtFirstName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtFirstName);
                                    Grid.SetRow(txtFirstName, 0);
                                    Grid.SetColumn(txtFirstName, 1);
                                }
                                if (value.ToLower().Contains("lastname"))
                                {
                                    TextBox txtLastName = new TextBox();
                                    txtLastName.Name = "txtLastName";
                                    txtLastName.Height = 25;
                                    txtLastName.Text = dtRow["LastName"].ToString();
                                    txtLastName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtLastName);
                                    Grid.SetRow(txtLastName, 0);
                                    Grid.SetColumn(txtLastName, 1);
                                }
                                if (value.ToLower().Contains("phonenumber"))
                                {
                                    TextBox txtPhoneNumber = new TextBox();
                                    txtPhoneNumber.Name = "txtPhoneNumber";
                                    txtPhoneNumber.Height = 25;
                                    txtPhoneNumber.Text = dtRow["PhoneNumber"].ToString();
                                    txtPhoneNumber.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtPhoneNumber);
                                    Grid.SetRow(txtPhoneNumber, 0);
                                    Grid.SetColumn(txtPhoneNumber, 1);
                                }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }
                            InitializeButtons(index);
                        }
                        #endregion Custom Favorite Field List is not Set
                    }
                    #endregion Editing Favorite Type - DN
                }
                #endregion Edit Favorite

                #region Add New Favorite
                else
                {
                    lblTitle.Content = "Add Favorites";
                    #region Add Favorite Type - Internal Item
                    if (Datacontext.GetInstance().IsFavoriteItem)
                    {
                        #region Internal Favorite List is Set
                        if (Datacontext.GetInstance().InternalFavoriteList.Count > 0)
                        {

                            //if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                            //    _requestConfObject.Invoke("Agent", Datacontext.GetInstance().UniqueIdentity);
                            //else if (Datacontext.GetInstance().FavoriteItemSelectedType == "AgentGroup")
                            //    _requestConfObject.Invoke("AgentGroup", Datacontext.GetInstance().UniqueIdentity);
                            //else
                            //    _requestConfObject.Invoke("Skill", Datacontext.GetInstance().UniqueIdentity);

                            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                            if (Datacontext.GetInstance().SearchedAgentDocuments.ContainsKey(Datacontext.GetInstance().UniqueIdentity))
                                doc = Datacontext.GetInstance().SearchedAgentDocuments[Datacontext.GetInstance().UniqueIdentity];

                            foreach (string value in Datacontext.GetInstance().InternalFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in Datacontext.GetInstance().InternalFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                tempGrid.Name = "grid" + value;
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    tempGrid.Children.Add(lbl);
                                    ComboBox cmbCategory = new ComboBox();
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.Width = 175;
                                    cmbCategory.Height = 25;
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    tempGrid.Children.Add(lbl);
                                    Label lblDisplayName = new Label();
                                    lblDisplayName.Name = "lblDisplayName";
                                    lblDisplayName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    lblDisplayName.Margin = new Thickness(5);
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("emailaddress"))
                                    {
                                        tempGrid.Children.Add(lbl);
                                        Label lblEmailAddress = new Label();
                                        lblEmailAddress.Name = "lblEmailAddress";
                                        lblEmailAddress.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        lblEmailAddress.Margin = new Thickness(5);
                                        lblEmailAddress.Content = doc.GetField("EmailAddress").StringValue();
                                        tempGrid.Children.Add(lblEmailAddress);
                                        Grid.SetRow(lblEmailAddress, index);
                                        Grid.SetColumn(lblEmailAddress, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("firstname"))
                                    {
                                        tempGrid.Children.Add(lbl);
                                        Label lblFirstName = new Label();
                                        lblFirstName.Name = "lblFirstName";
                                        lblFirstName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        lblFirstName.Margin = new Thickness(5);
                                        lblFirstName.Content = doc.GetField("FirstName").StringValue();
                                        tempGrid.Children.Add(lblFirstName);
                                        Grid.SetRow(lblFirstName, index);
                                        Grid.SetColumn(lblFirstName, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("lastname"))
                                    {
                                        tempGrid.Children.Add(lbl);
                                        Label lblLastName = new Label();
                                        lblLastName.Name = "lblLastName";
                                        lblLastName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        lblLastName.Margin = new Thickness(5);
                                        lblLastName.Content = doc.GetField("LastName").StringValue();
                                        tempGrid.Children.Add(lblLastName);
                                        Grid.SetRow(lblLastName, index);
                                        Grid.SetColumn(lblLastName, 1);
                                    }
                                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                    if (value.ToLower().Contains("phonenumber"))
                                    {
                                        tempGrid.Children.Add(lbl);
                                        Label lblPhoneNumber = new Label();
                                        lblPhoneNumber.Name = "lblPhoneNumber";
                                        lblPhoneNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        lblPhoneNumber.Margin = new Thickness(5);
                                        lblPhoneNumber.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                        tempGrid.Children.Add(lblPhoneNumber);
                                        Grid.SetRow(lblPhoneNumber, index);
                                        Grid.SetColumn(lblPhoneNumber, 1);
                                    }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }
                            InitializeButtons(index);
                        }
                        #endregion Internal Favorite List is Set

                        #region Internal Favorite List is Not Set
                        else
                        {
                            LoadInternalEmptyListUI();
                        }
                        #endregion Internal Favorite List is Not Set
                    }
                    #endregion Add Favorite Type - Internal Item

                    #region Add Favorite Type -  DN
                    else
                    {
                        #region Custom Favorite Field List is Set
                        if (Datacontext.GetInstance().CustomFavoriteList.Count > 0)
                        {
                            foreach (string value in Datacontext.GetInstance().CustomFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in Datacontext.GetInstance().CustomFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                //lbl.Height = 25;
                                tempGrid.Children.Add(lbl);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    ComboBox cmbCategory = new ComboBox();
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.Height = 25;
                                    cmbCategory.Width = 175;
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.HorizontalAlignment = HorizontalAlignment.Left;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    Label lblDisplayName = new Label();
                                    lblDisplayName.Name = "lblDisplayName";
                                    lblDisplayName.Height = 25;
                                    lblDisplayName.Width = 175;
                                    lblDisplayName.HorizontalAlignment = HorizontalAlignment.Left;
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    lblDisplayName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (value.ToLower().Contains("emailaddress"))
                                {
                                    TextBox txtEmailAddress = new TextBox();
                                    txtEmailAddress.Name = "txtEmailAddress";
                                    txtEmailAddress.Height = 25;
                                    txtEmailAddress.Width = 175;
                                    txtEmailAddress.HorizontalAlignment = HorizontalAlignment.Left;
                                    txtEmailAddress.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtEmailAddress);
                                    Grid.SetRow(txtEmailAddress, 0);
                                    Grid.SetColumn(txtEmailAddress, 1);
                                }
                                if (value.ToLower().Contains("firstname"))
                                {
                                    TextBox txtFirstName = new TextBox();
                                    txtFirstName.Name = "txtFirstName";
                                    txtFirstName.Height = 25;
                                    txtFirstName.HorizontalAlignment = HorizontalAlignment.Left;
                                    txtFirstName.Margin = new Thickness(5);
                                    txtFirstName.Width = 175;
                                    tempGrid.Children.Add(txtFirstName);
                                    Grid.SetRow(txtFirstName, 0);
                                    Grid.SetColumn(txtFirstName, 1);
                                }
                                if (value.ToLower().Contains("lastname"))
                                {
                                    TextBox txtLastName = new TextBox();
                                    txtLastName.Name = "txtLastName";
                                    txtLastName.Height = 25;
                                    txtLastName.Width = 175;
                                    txtLastName.HorizontalAlignment = HorizontalAlignment.Left;
                                    txtLastName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtLastName);
                                    Grid.SetRow(txtLastName, 0);
                                    Grid.SetColumn(txtLastName, 1);
                                }
                                if (value.ToLower().Contains("phonenumber"))
                                {
                                    TextBox txtPhoneNumber = new TextBox();
                                    txtPhoneNumber.Name = "txtPhoneNumber";
                                    txtPhoneNumber.Width = 175;
                                    txtPhoneNumber.HorizontalAlignment = HorizontalAlignment.Left;
                                    txtPhoneNumber.PreviewKeyDown += new KeyEventHandler(cmshow_KeyDown);
                                    txtPhoneNumber.Height = 25;
                                    txtPhoneNumber.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtPhoneNumber);
                                    Grid.SetRow(txtPhoneNumber, 0);
                                    Grid.SetColumn(txtPhoneNumber, 1);
                                }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }

                            InitializeButtons(index);
                        }
                        #endregion Custom Favorite Field List is Set

                        #region Custom Favorite Field List is not Set
                        else
                        {
                            foreach (string value in customFavoriteList)
                            {
                                RowDefinition rowDefinition = new RowDefinition();
                                rowDefinition.Height = new GridLength(0);
                                mainGrid.RowDefinitions.Add(rowDefinition);
                            }
                            RowDefinition rowDefinitions = new RowDefinition();
                            rowDefinitions.Height = new GridLength(0);
                            mainGrid.RowDefinitions.Add(rowDefinitions);
                            int index = 0;
                            foreach (string value in customFavoriteList)
                            {
                                Grid tempGrid = new Grid();
                                ColumnDefinition col1 = new ColumnDefinition();
                                ColumnDefinition col2 = new ColumnDefinition();
                                col1.Width = new GridLength(100);
                                col2.Width = new GridLength(200);
                                tempGrid.ColumnDefinitions.Add(col1);
                                tempGrid.ColumnDefinitions.Add(col2);
                                Label lbl = new Label();
                                lbl.Content = value;
                                lbl.Margin = new Thickness(5);
                                tempGrid.Children.Add(lbl);
                                Grid.SetRow(lbl, 0);
                                Grid.SetColumn(lbl, 0);
                                if (value.ToLower().Contains("category"))
                                {
                                    ComboBox cmbCategory = new ComboBox();
                                    cmbCategory.Name = "cmbCategory";
                                    cmbCategory.Height = 25;
                                    cmbCategory.Width = 175;
                                    cmbCategory.IsEditable = true;
                                    cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                                    cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    cmbCategory.Margin = new Thickness(5);
                                    tempGrid.Children.Add(cmbCategory);
                                    Grid.SetRow(cmbCategory, 0);
                                    Grid.SetColumn(cmbCategory, 1);
                                }
                                if (value.ToLower().Contains("displayname"))
                                {
                                    Label lblDisplayName = new Label();
                                    lblDisplayName.Name = "lblDisplayName";
                                    //lblDisplayName.Height=25;
                                    lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                                    lblDisplayName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    lblDisplayName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(lblDisplayName);
                                    Grid.SetRow(lblDisplayName, 0);
                                    Grid.SetColumn(lblDisplayName, 1);
                                }
                                if (value.ToLower().Contains("emailaddress"))
                                {
                                    TextBox txtEmailAddress = new TextBox();
                                    txtEmailAddress.Name = "txtEmailAddress";
                                    txtEmailAddress.Height = 25;
                                    txtEmailAddress.Margin = new Thickness(5);
                                    txtEmailAddress.Width = 175;
                                    txtEmailAddress.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    tempGrid.Children.Add(txtEmailAddress);
                                    Grid.SetRow(txtEmailAddress, 0);
                                    Grid.SetColumn(txtEmailAddress, 1);
                                }
                                if (value.ToLower().Contains("firstname"))
                                {
                                    TextBox txtFirstName = new TextBox();
                                    txtFirstName.Name = "txtFirstName";
                                    txtFirstName.Height = 25;
                                    txtFirstName.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtFirstName);
                                    txtFirstName.Width = 175;
                                    txtFirstName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    Grid.SetRow(txtFirstName, 0);
                                    Grid.SetColumn(txtFirstName, 1);
                                }
                                if (value.ToLower().Contains("lastname"))
                                {
                                    TextBox txtLastName = new TextBox();
                                    txtLastName.Name = "txtLastName";
                                    txtLastName.Height = 25;
                                    txtLastName.Margin = new Thickness(5);
                                    txtLastName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    tempGrid.Children.Add(txtLastName);
                                    txtLastName.Width = 175;
                                    Grid.SetRow(txtLastName, 0);
                                    Grid.SetColumn(txtLastName, 1);
                                }
                                if (value.ToLower().Contains("phonenumber"))
                                {
                                    TextBox txtPhoneNumber = new TextBox();
                                    txtPhoneNumber.Name = "txtPhoneNumber";
                                    txtPhoneNumber.Height = 25;
                                    txtPhoneNumber.Width = 175;
                                    txtPhoneNumber.Margin = new Thickness(5);
                                    tempGrid.Children.Add(txtPhoneNumber);
                                    txtPhoneNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                    Grid.SetRow(txtPhoneNumber, 0);
                                    Grid.SetColumn(txtPhoneNumber, 1);
                                }
                                mainGrid.Children.Add(tempGrid);
                                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                                Grid.SetRow(tempGrid, index);
                                index++;
                            }

                            InitializeButtons(index);
                        }
                        #endregion Custom Favorite Field List is not Set
                    }
                    #endregion Add Favorite Type - DN
                }
                #endregion Add New Favorite

            }
            catch (Exception generalException)
            {
                _logger.Error("Favorites : InitializeUI() Error : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Loads the internal empty list UI.
        /// </summary>
        private void LoadInternalEmptyListUI()
        {
            int index = 0;
            try
            {

                #region Selected is Agent
                if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                {
                    //_requestConfObject.Invoke("Agent", Datacontext.GetInstance().UniqueIdentity);
                    Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                    if (Datacontext.GetInstance().SearchedAgentDocuments.ContainsKey(Datacontext.GetInstance().UniqueIdentity))
                        doc = Datacontext.GetInstance().SearchedAgentDocuments[Datacontext.GetInstance().UniqueIdentity];
                    foreach (string value in internalAgentFavoriteList)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Name = "row" + value;
                        rowDefinition.Height = new GridLength(0);
                        mainGrid.RowDefinitions.Add(rowDefinition);
                    }
                    RowDefinition rowDefinitions = new RowDefinition();
                    rowDefinitions.Height = new GridLength(0);
                    mainGrid.RowDefinitions.Add(rowDefinitions);
                    foreach (string value in internalAgentFavoriteList)
                    {
                        Grid tempGrid = new Grid();
                        ColumnDefinition col1 = new ColumnDefinition();
                        ColumnDefinition col2 = new ColumnDefinition();
                        col1.Width = new GridLength(100);
                        col2.Width = new GridLength(200);
                        tempGrid.ColumnDefinitions.Add(col1);
                        tempGrid.ColumnDefinitions.Add(col2);

                        Label lbl = new Label();
                        lbl.Content = value;
                        lbl.Margin = new Thickness(5);
                        tempGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, index);
                        Grid.SetColumn(lbl, 0);

                        if (value == "Category")
                        {
                            ComboBox cmbCategory = new ComboBox();
                            cmbCategory.Name = "cmbCategory";
                            cmbCategory.Height = 25;
                            cmbCategory.Width = 175;
                            cmbCategory.IsEditable = true;
                            cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                            cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            cmbCategory.Margin = new Thickness(5);
                            tempGrid.Children.Add(cmbCategory);
                            Grid.SetRow(cmbCategory, index);
                            Grid.SetColumn(cmbCategory, 1);
                        }
                        if (value == "DisplayName")
                        {
                            Label lblDisplayName = new Label();
                            lblDisplayName.Name = "lblDisplayName";
                            lblDisplayName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblDisplayName.Margin = new Thickness(5);
                            lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            tempGrid.Children.Add(lblDisplayName);
                            Grid.SetRow(lblDisplayName, index);
                            Grid.SetColumn(lblDisplayName, 1);
                        }
                        if (value == "EmailAddress")
                        {
                            Label lblEmailAddress = new Label();
                            lblEmailAddress.Name = "lblEmailAddress";
                            lblEmailAddress.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblEmailAddress.Margin = new Thickness(5);
                            lblEmailAddress.Content = doc.GetField("EmailAddress").StringValue();
                            tempGrid.Children.Add(lblEmailAddress);
                            Grid.SetRow(lblEmailAddress, index);
                            Grid.SetColumn(lblEmailAddress, 1);
                        }
                        if (value == "EmployeeId")
                        {
                            Label lblEmployeeId = new Label();
                            lblEmployeeId.Name = "lblEmployeeId";
                            lblEmployeeId.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblEmployeeId.Margin = new Thickness(5);
                            lblEmployeeId.Content = doc.GetField("EmployeeID").StringValue();
                            tempGrid.Children.Add(lblEmployeeId);
                            Grid.SetRow(lblEmployeeId, index);
                            Grid.SetColumn(lblEmployeeId, 1);
                        }
                        if (value == "FirstName")
                        {
                            Label lblFirstName = new Label();
                            lblFirstName.Name = "lblFirstName";
                            lblFirstName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblFirstName.Margin = new Thickness(5);
                            lblFirstName.Content = doc.GetField("FirstName").StringValue();
                            tempGrid.Children.Add(lblFirstName);
                            Grid.SetRow(lblFirstName, index);
                            Grid.SetColumn(lblFirstName, 1);
                        }
                        if (value == "LastName")
                        {
                            Label lblLastName = new Label();
                            lblLastName.Name = "lblLastName";
                            lblLastName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblLastName.Margin = new Thickness(5);
                            lblLastName.Content = doc.GetField("LastName").StringValue();
                            tempGrid.Children.Add(lblLastName);
                            Grid.SetRow(lblLastName, index);
                            Grid.SetColumn(lblLastName, 1);
                        }
                        if (value == "LevelDbId")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelDBID").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "LevelType")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelType").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "PhoneNumber")
                        {
                            Label lblPhoneNumber = new Label();
                            lblPhoneNumber.Name = "lblPhoneNumber";
                            lblPhoneNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblPhoneNumber.Margin = new Thickness(5);
                            lblPhoneNumber.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            tempGrid.Children.Add(lblPhoneNumber);
                            Grid.SetRow(lblPhoneNumber, index);
                            Grid.SetColumn(lblPhoneNumber, 1);
                        }
                        if (value == "TenantName")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("TenantName").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "UserName")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Name = "lblUniqueIdentity";
                            lblTempValue.Content = doc.GetField("UserName").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        mainGrid.Children.Add(tempGrid);
                        mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                        Grid.SetRow(tempGrid, index);
                        index++;
                    }
                }
                #endregion Selected is Agent

                #region Selected is Agent Group
                else if (Datacontext.GetInstance().FavoriteItemSelectedType == "AgentGroup")
                {

                    Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                    if (Datacontext.GetInstance().SearchedAgentGroupDocuments.ContainsKey(Datacontext.GetInstance().UniqueIdentity))
                        doc = Datacontext.GetInstance().SearchedAgentGroupDocuments[Datacontext.GetInstance().UniqueIdentity];
                    foreach (string value in internalAgentGroupFavoriteList)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new GridLength(0);
                        mainGrid.RowDefinitions.Add(rowDefinition);
                    }
                    RowDefinition rowDefinitions = new RowDefinition();
                    rowDefinitions.Height = new GridLength(0);
                    mainGrid.RowDefinitions.Add(rowDefinitions);
                    index = 0;
                    foreach (string value in internalAgentGroupFavoriteList)
                    {
                        Grid tempGrid = new Grid();
                        ColumnDefinition col1 = new ColumnDefinition();
                        ColumnDefinition col2 = new ColumnDefinition();
                        col1.Width = new GridLength(100);
                        col2.Width = new GridLength(200);
                        tempGrid.ColumnDefinitions.Add(col1);
                        tempGrid.ColumnDefinitions.Add(col2);

                        Label lbl = new Label();
                        lbl.Content = value;
                        lbl.Margin = new Thickness(5);
                        tempGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, index);
                        Grid.SetColumn(lbl, 0);

                        if (value == "Category")
                        {
                            ComboBox cmbCategory = new ComboBox();
                            cmbCategory.Name = "cmbCategory";
                            cmbCategory.Height = 25;
                            cmbCategory.Width = 175;
                            cmbCategory.IsEditable = true;
                            cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                            cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            cmbCategory.Margin = new Thickness(5);
                            tempGrid.Children.Add(cmbCategory);
                            Grid.SetRow(cmbCategory, index);
                            Grid.SetColumn(cmbCategory, 1);
                        }
                        if (value == "DisplayName")
                        {
                            Label lblDisplayName = new Label();
                            lblDisplayName.Name = "lblDisplayName";
                            lblDisplayName.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblDisplayName.Margin = new Thickness(5);
                            lblDisplayName.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            tempGrid.Children.Add(lblDisplayName);
                            Grid.SetRow(lblDisplayName, index);
                            Grid.SetColumn(lblDisplayName, 1);
                        }
                        if (value == "Number")
                        {
                            Label lblNumber = new Label();
                            lblNumber.Name = "lblNumber";
                            lblNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblNumber.Margin = new Thickness(5);
                            lblNumber.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            tempGrid.Children.Add(lblNumber);
                            Grid.SetRow(lblNumber, index);
                            Grid.SetColumn(lblNumber, 1);
                        }

                        if (value == "LevelDbId")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelDBID").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        //if (value == "LevelDbId")
                        //{
                        //    Label lblTempValue = new Label();
                        //    lblTempValue.Content = Datacontext.GetInstance().AgentGroup.GroupInfo.ToString();
                        //    tempGrid.Children.Add(lblTempValue);
                        //    Grid.SetRow(lblTempValue, index);
                        //    Grid.SetColumn(lblTempValue, 1);
                        //}
                        if (value == "LevelType")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelType").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "TenantName")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("TenantName").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "Name")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "PhoneNumber")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        mainGrid.Children.Add(tempGrid);
                        mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                        Grid.SetRow(tempGrid, index);
                        index++;
                    }
                }
                #endregion  Selected is Agent Group

                #region Selected is Skill
                else if (Datacontext.GetInstance().FavoriteItemSelectedType == "Skill")
                {

                    Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                    if (Datacontext.GetInstance().SearchedSkillDocuments.ContainsKey(Datacontext.GetInstance().UniqueIdentity))
                        doc = Datacontext.GetInstance().SearchedSkillDocuments[Datacontext.GetInstance().UniqueIdentity];
                    foreach (string value in internalSkillFavoriteList)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new GridLength(0);
                        mainGrid.RowDefinitions.Add(rowDefinition);
                    }
                    RowDefinition rowDefinitions = new RowDefinition();
                    rowDefinitions.Height = new GridLength(0);
                    mainGrid.RowDefinitions.Add(rowDefinitions);
                    index = 0;
                    foreach (string value in internalSkillFavoriteList)
                    {
                        Grid tempGrid = new Grid();
                        ColumnDefinition col1 = new ColumnDefinition();
                        ColumnDefinition col2 = new ColumnDefinition();
                        col1.Width = new GridLength(100);
                        col2.Width = new GridLength(200);
                        tempGrid.ColumnDefinitions.Add(col1);
                        tempGrid.ColumnDefinitions.Add(col2);

                        Label lbl = new Label();
                        lbl.Content = value;
                        lbl.Margin = new Thickness(5);
                        tempGrid.Children.Add(lbl);
                        Grid.SetRow(lbl, index);
                        Grid.SetColumn(lbl, 0);

                        if (value == "Category")
                        {
                            ComboBox cmbCategory = new ComboBox();
                            cmbCategory.Name = "cmbCategory";
                            cmbCategory.Height = 25;
                            cmbCategory.IsEditable = true;
                            cmbCategory.Width = 175;
                            cmbCategory.ItemsSource = Datacontext.GetInstance().CategoryNamesList;
                            cmbCategory.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            cmbCategory.Margin = new Thickness(5);
                            tempGrid.Children.Add(cmbCategory);
                            Grid.SetRow(cmbCategory, index);
                            Grid.SetColumn(cmbCategory, 1);
                        }
                        if (value == "DisplayName")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "Number")
                        {
                            Label lblNumber = new Label();
                            lblNumber.Name = "lblNumber";
                            lblNumber.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblNumber.Margin = new Thickness(5);
                            lblNumber.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            tempGrid.Children.Add(lblNumber);
                            Grid.SetRow(lblNumber, index);
                            Grid.SetColumn(lblNumber, 1);
                        }
                        if (value == "LevelDbId")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelDBID").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "LevelType")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("LevelType").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "TenantName")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = doc.GetField("TenantName").StringValue();
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "Name")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        if (value == "Number")
                        {
                            Label lblTempValue = new Label();
                            lblTempValue.Content = Datacontext.GetInstance().FavoriteDisplayName;
                            lblTempValue.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            lblTempValue.Margin = new Thickness(5);
                            tempGrid.Children.Add(lblTempValue);
                            Grid.SetRow(lblTempValue, index);
                            Grid.SetColumn(lblTempValue, 1);
                        }
                        mainGrid.Children.Add(tempGrid);
                        mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                        Grid.SetRow(tempGrid, index);
                        index++;
                    }
                }
                #endregion Selected is Skill

                #region Add Ok, Cancel Buttons

                InitializeButtons(index);

                #endregion Add Ok, Cancel Buttons

            }
            catch (Exception generalException)
            {
                _logger.Error("Favorites : LoadInternalEmptyListUI() Error : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region Add/Edit Favorite

                #region Favorite Type - Internal Item

                if (Datacontext.GetInstance().IsFavoriteItem)
                {
                    #region Internal Favorite List is Set

                    if (Datacontext.GetInstance().InternalFavoriteList.Count > 0)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                        foreach (string value in Datacontext.GetInstance().InternalFavoriteList)
                        {
                            if (value.ToLower().Contains("category"))
                            {
                                //Find the Logical child created dynamically
                                var cmb = LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                                if (cmb is ComboBox)
                                {
                                    if (cmb as ComboBox != null)
                                    {
                                        dtRow["Category"] = (cmb as ComboBox).Text;
                                    }
                                }
                            }
                            if (value.ToLower().Contains("displayname"))
                            {
                                dtRow["DisplayName"] = Datacontext.GetInstance().FavoriteDisplayName;
                            }
                            if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                if (value.ToLower().Contains("emailaddress"))
                                {
                                    dtRow["EmailAddress"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "lblEmailAddress")) as Label).Content;
                                }
                            if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                if (value.ToLower().Contains("firstname"))
                                {
                                    dtRow["FirstName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "lblFirstName")) as Label).Content;
                                }
                            if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                if (value.ToLower().Contains("lastname"))
                                {
                                    dtRow["LastName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "lblLastName")) as Label).Content;
                                }
                            if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                                if (value.ToLower().Contains("phonenumber"))
                                {
                                    dtRow["PhoneNumber"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "lblPhoneNumber")) as Label).Content;
                                }
                        }
                        if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                       !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                            Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                        dtRow["UniqueIdentity"] = Datacontext.GetInstance().UniqueIdentity;
                        dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;

                        bool recordPresent = false;
                        recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                        if (!recordPresent)
                        {  //Row not present add it
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        else
                        {  //Row is already present update it
                            Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                    }
                    #endregion

                    #region Internal Favorite List is not Set
                    else
                    {
                        #region Agent
                        if (Datacontext.GetInstance().FavoriteItemSelectedType == "Agent")
                        {
                            DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                            ComboBox cmbCategory = (ComboBox)LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                            Label lblDisplayName = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblDisplayName");
                            Label lblEmailAddress = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblEmailAddress");
                            Label lblFirstName = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblFirstName");
                            Label lblLastName = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblLastName");
                            Label lblPhoneNumber = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblPhoneNumber");
                            Label lblUniqueIdentity = (Label)LogicalTreeHelper.FindLogicalNode(mainGrid, "lblUniqueIdentity");
                            dtRow["Category"] = cmbCategory.Text;
                            dtRow["DisplayName"] = lblDisplayName.Content;
                            dtRow["EmailAddress"] = lblEmailAddress.Content;
                            dtRow["FirstName"] = lblFirstName.Content;
                            dtRow["LastName"] = lblLastName.Content;
                            dtRow["PhoneNumber"] = lblPhoneNumber.Content;
                            dtRow["UniqueIdentity"] = lblUniqueIdentity.Content;
                            dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;
                            if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                      !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                                Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());

                            bool recordPresent = false;
                            recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                            if (!recordPresent)
                            {  //Row not present add it
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            else
                            {  //Row is already present update it
                                Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                        }
                        #endregion Agent

                        #region AgentGroup
                        else if (Datacontext.GetInstance().FavoriteItemSelectedType == "AgentGroup")
                        {
                            DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                            ComboBox cmbCategory = (ComboBox)LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                            dtRow["Category"] = cmbCategory.Text;
                            dtRow["DisplayName"] = Datacontext.GetInstance().FavoriteDisplayName;
                            dtRow["FirstName"] = "";
                            dtRow["LastName"] = "";
                            dtRow["PhoneNumber"] = Datacontext.GetInstance().FavoriteDisplayName;
                            dtRow["EmailAddress"] = "";
                            dtRow["UniqueIdentity"] = Datacontext.GetInstance().UniqueIdentity;
                            dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;
                            if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                        !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                                Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                            bool recordPresent = false;
                            recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                            if (!recordPresent)
                            {  //Row not present add it
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            else
                            {  //Row is already present update it
                                Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                        }
                        #endregion AgentGroup

                        #region Skill
                        else
                        {
                            DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                            ComboBox cmbCategory = (ComboBox)LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                            dtRow["Category"] = cmbCategory.Text;
                            dtRow["DisplayName"] = Datacontext.GetInstance().FavoriteDisplayName;
                            dtRow["FirstName"] = "";
                            dtRow["LastName"] = "";
                            dtRow["PhoneNumber"] = Datacontext.GetInstance().FavoriteDisplayName;
                            dtRow["EmailAddress"] = "";
                            dtRow["UniqueIdentity"] = Datacontext.GetInstance().UniqueIdentity;
                            dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;
                            if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                        !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                                Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                            bool recordPresent = false;
                            recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                            if (!recordPresent)
                            {  //Row not present add it
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            else
                            {  //Row is already present update it
                                Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                                Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                            }
                            xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                        }
                        #endregion Skill
                    }
                    #endregion Internal Favorite List is not Set
                }

                #endregion Favorite Type - Internal Item

                #region Favorite Type - DN

                else
                {
                    #region Custom Favorite List is Set

                    if (Datacontext.GetInstance().CustomFavoriteList.Count > 0)
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                        foreach (string value in Datacontext.GetInstance().CustomFavoriteList)
                        {
                            if (value.ToLower().Contains("category"))
                            {
                                //Find the Logical child created dynamically
                                var cmb = LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");

                                if (cmb is ComboBox)
                                {
                                    if (cmb as ComboBox != null)
                                    {
                                        dtRow["Category"] = (cmb as ComboBox).Text;
                                    }
                                }
                            }
                            if (value.ToLower().Contains("displayname"))
                            {
                                dtRow["DisplayName"] = Datacontext.GetInstance().FavoriteDisplayName;
                            }
                            if (value.ToLower().Contains("emailaddress"))
                            {
                                dtRow["EmailAddress"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtEmailAddress")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("firstname"))
                            {
                                dtRow["FirstName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtFirstName")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("lastname"))
                            {
                                dtRow["LastName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtLastName")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("phonenumber"))
                            {
                                dtRow["PhoneNumber"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtPhoneNumber")) as TextBox).Text;
                            }
                        }
                        if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                         !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                            Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                        dtRow["UniqueIdentity"] = Datacontext.GetInstance().UniqueIdentity;
                        dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;
                        bool recordPresent = false;
                        recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                        if (!recordPresent)
                        {  //Row not present add it
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        else
                        {  //Row is already present update it
                            Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                    }

                    #endregion Custom Favorite List is Set

                    #region Custom Favorite List is Not Set

                    else
                    {
                        DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                        foreach (string value in customFavoriteList)
                        {
                            if (value.ToLower().Contains("category"))
                            {
                                //Find the Logical child created dynamically
                                var cmb = LogicalTreeHelper.FindLogicalNode(mainGrid, "cmbCategory");
                                if (cmb is ComboBox)
                                {
                                    if (cmb as ComboBox != null)
                                    {
                                        dtRow["Category"] = (cmb as ComboBox).Text;
                                    }
                                }
                            }
                            if (value.ToLower().Contains("displayname"))
                            {
                                dtRow["DisplayName"] = Datacontext.GetInstance().FavoriteDisplayName;
                            }
                            if (value.ToLower().Contains("emailaddress"))
                            {
                                dtRow["EmailAddress"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtEmailAddress")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("firstname"))
                            {
                                dtRow["FirstName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtFirstName")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("lastname"))
                            {
                                dtRow["LastName"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtLastName")) as TextBox).Text;
                            }
                            if (value.ToLower().Contains("phonenumber"))
                            {
                                dtRow["PhoneNumber"] = ((LogicalTreeHelper.FindLogicalNode(mainGrid, "txtPhoneNumber")) as TextBox).Text;
                            }
                        }
                        if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                       !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                            Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                        dtRow["UniqueIdentity"] = Datacontext.GetInstance().UniqueIdentity;
                        dtRow["Type"] = Datacontext.GetInstance().FavoriteItemSelectedType;
                        bool recordPresent = false;
                        recordPresent = Datacontext.GetInstance().dtFavorites.AsEnumerable().Any(row => Datacontext.GetInstance().UniqueIdentity == row.Field<String>("UniqueIdentity"));

                        if (!recordPresent)
                        {  //Row not present add it
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        else
                        {  //Row is already present update it
                            Datacontext.GetInstance().dtFavorites.AsEnumerable().Where(r => r.Field<string>("UniqueIdentity") == Datacontext.GetInstance().UniqueIdentity).ToList().ForEach(row => row.Delete());
                            Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                        }
                        xmlHandler.ModifyXmlData(dtRow, dtRow["UniqueIdentity"].ToString(), dtRow["Type"].ToString());
                    }

                    #endregion Custom Favorite List is Not Set
                }

                #endregion Favorite Type - DN

                #endregion Add Favorite

                this.Close();
            }
            catch (Exception generalException)
            {
                _logger.Error("Favorites : btnOk_Click() Error : " + generalException.Message.ToString());
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Label control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }

        #region Add Ok, Cancel Buttons

        /// <summary>
        /// Initializes the buttons.
        /// </summary>
        /// <param name="index">The index.</param>
        private void InitializeButtons(int index)
        {
            try
            {

                Grid temp = new Grid();
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                temp.ColumnDefinitions.Add(colDef1);
                colDef1.Width = new GridLength(100);
                colDef2.Width = new GridLength(200);
                temp.ColumnDefinitions.Add(colDef2);

                Button btnOk = new Button();
                btnOk.Content = "Ok";
                btnOk.Height = 23;
                btnOk.Margin = new Thickness(0, 10, 105, 10);
                btnOk.Width = 80;
                btnOk.HorizontalAlignment = HorizontalAlignment.Right;
                btnOk.Style = (Style)FindResource("NormalButton");

                btnOk.Click += new RoutedEventHandler(btnOk_Click);
                temp.Children.Add(btnOk);
                Grid.SetRow(btnOk, 0);
                Grid.SetColumn(btnOk, 1);

                Button btnCancel = new Button();
                btnCancel.Content = "Cancel";
                btnCancel.Height = 23;
                btnCancel.Margin = new Thickness(0, 10, 20, 10);
                btnCancel.Width = 80;
                btnCancel.HorizontalAlignment = HorizontalAlignment.Right;
                btnCancel.Style = (Style)FindResource("NormalButton");

                btnCancel.Click += new RoutedEventHandler(btnCancel_Click);
                temp.Children.Add(btnCancel);
                Grid.SetRow(btnCancel, 0);
                Grid.SetColumn(btnCancel, 1);

                mainGrid.Children.Add(temp);
                mainGrid.RowDefinitions[index].Height = GridLength.Auto;
                Grid.SetRow(temp, index);
            }
            catch (Exception generalException)
            {
                _logger.Error("Favorites : InitializeButtons() Error : " + generalException.Message.ToString());
            }
        }

        #endregion Add Ok, Cancel Buttons

        /// <summary>
        /// Handles the KeyDown event of the cmshow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void cmshow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (e.Key == Key.D3 || e.Key == Key.D8)
                {
                    if (e.Key == Key.D3)
                    {

                    }
                    if (e.Key == Key.D8)
                    {

                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.D0:
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                    case Key.NumLock:
                    case Key.NumPad0:
                    case Key.NumPad1:
                    case Key.NumPad2:
                    case Key.NumPad3:
                    case Key.NumPad4:
                    case Key.NumPad5:
                    case Key.NumPad6:
                    case Key.NumPad7:
                    case Key.NumPad8:
                    case Key.NumPad9:
                    case Key.Multiply:
                    case Key.Left:
                    case Key.Right:
                    case Key.End:
                    case Key.Home:
                    case Key.Prior:
                    case Key.Next:
                        {
                            break;
                        }
                    case Key.Delete:
                    case Key.Back:
                        break;
                    case Key.Enter:
                        //if (!Datacontext.isRinging && !Datacontext.GetInstance().isOnCall)
                        //{

                        //}
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
            }

        }

    }
}

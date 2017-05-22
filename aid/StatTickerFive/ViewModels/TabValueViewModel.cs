using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StatTickerFive.Helpers;
using System.Collections.ObjectModel;
using Pointel.Statistics.Core.Utility;
using System.Windows.Input;
using StatTickerFive.Views;
using System.Windows.Media;
using Pointel.Statistics.Core;
using Pointel.Logger.Core;

namespace StatTickerFive.ViewModels
{
    public class TabValueViewModel : NotificationObject
    {
        #region Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsSupport objStatSupport = new StatisticsSupport();
        StatisticsBase objStatBase = new StatisticsBase();
        List<string> LstAgentStatistics = new List<string>();
        List<string> LstAgentGroupStatistics = new List<string>();
        Dictionary<string, List<string>> ObjectsDictionary = new Dictionary<string, List<string>>();

        public List<string> LstStatistics = new List<string>();
        public string ObjectId = string.Empty;
        string Objecttype = string.Empty;
        public ObjectValues SelectedStatistics = null;
        public ObjectConfigWindowViewModel ObjectConfigVM = null;
        public ObjectConfigWindowViewModel NewObjectConfigVM = null;
        #endregion

        #region Properties
        private SolidColorBrush _titleBackground;
        public SolidColorBrush TitleBackground
        {
            get
            {
                return _titleBackground;
            }
            set
            {
                if (value != null)
                {
                    _titleBackground = value;
                    RaisePropertyChanged(() => TitleBackground);
                }
            }
        }

        private ObservableCollection<ObjectValues> _ObjectStatistics;
        public ObservableCollection<ObjectValues> ObjectStatistics
        {
            get
            {
                return _ObjectStatistics;
            }
            set
            {
                _ObjectStatistics = value;
                RaisePropertyChanged(() => ObjectStatistics);
            }
        }

        private ObservableCollection<ObjectValues> _NewStatistics;
        public ObservableCollection<ObjectValues> NewStatistics
        {
            get
            {
                return _NewStatistics;
            }
            set
            {
                _NewStatistics = value;
                RaisePropertyChanged(() => NewStatistics);
            }
        }

        private ObservableCollection<ObjectValues> _TempNewStatistics;
        public ObservableCollection<ObjectValues> TempNewStatistics
        {
            get
            {
                return _TempNewStatistics;
            }
            set
            {
                _TempNewStatistics = value;
                RaisePropertyChanged(() => TempNewStatistics);
            }
        }

        #endregion

        #region Command
        public ICommand UndoNewStatisticsCommand { get { return new DelegateCommand(UndoNewStatistics); } }
        public ICommand EditObjectsCommand { get { return new DelegateCommand(EditObjects); } }
        public ICommand ObjectSelected { get { return new DelegateCommand(ObjectChecked); } }
        public ICommand SelectNewStatistics { get { return new DelegateCommand(SelectStatistics); } }
        public ICommand SaveObjectValuesCommand { get { return new DelegateCommand(SaveObjectValues); } }
        #endregion

        #region Constructor
        public TabValueViewModel()
        {
            try
            {
                logger.Debug("TabValueViewModel : constructor - Entry");
                //LoadWindowData(Uid);

            }
            catch (Exception GeneralException)
            {
                logger.Error("TabValueViewModel : constructor - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : constructor - Exit");
        }
        #endregion

        #region Methods

        #region EditObjects
        public void EditObjects()
        {

            try
            {
                Settings.GetInstance().IsApplicationObjectConfigWindow = false;
                logger.Debug("TabValueViewModel : EditObjects method - Entry");
                LstStatistics.Clear();
                foreach (ObjectValues value in ObjectStatistics)
                {
                    if ((value.ObjectName.StartsWith("acd")) || (value.ObjectName.StartsWith("dn")) || (value.ObjectName.StartsWith("vq")))
                    {
                        LstStatistics.Add(value.ObjectsNameTT.ToString());
                    }
                }
                ObjectConfigWindow Objectwindow = new ObjectConfigWindow();
                Objectwindow.Name = "EditObject";
                //if (Settings.GetInstance().ObjectConfigVM==null)
                //{
                //Settings.GetInstance().ObjectConfigVM = new ObjectConfigWindowViewModel();
                //}
                ObjectConfigVM = new ObjectConfigWindowViewModel();
                ObjectConfigVM.LoadObjectConfiguration(LstStatistics, ObjectId);
                Objectwindow.DataContext = ObjectConfigVM;
                Objectwindow.ShowDialog();
            }
            catch (Exception generalException)
            {
                logger.Error("TabValueViewModel : EditObjects method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : EditObjects method - Exit");
        }
        #endregion

        #region UndoNewStatistics
        public void UndoNewStatistics()
        {
            //try
            //{
            //    if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId) || Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
            //    {
            //        List<string> lstTempStats=new List<string>();
            //        lstTempStats = Settings.GetInstance().DictUpdatedStatistics[ObjectId];

            //        foreach(string stats in lstTempStats)
            //        {
            //            foreach (ObjectValues values in ObjectStatistics)
            //            {
            //                if (stats == values.ObjectName)
            //                {
            //                    ObjectStatistics.Remove(values);
            //                    break;
            //                }
            //            }
            //            List<string> lstExistingStats = new List<string>();

            //            if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
            //            {
            //                lstExistingStats = Settings.GetInstance().DictAgentStatisitics[ObjectId];
            //                if (lstExistingStats.Contains(stats))
            //                {
            //                    lstExistingStats.Remove(stats);
            //                    Settings.GetInstance().DictAgentStatisitics[ObjectId] = lstExistingStats;
            //                }
            //            }
            //            else if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
            //            {
            //                lstExistingStats = Settings.GetInstance().DictAgentStatisitics[ObjectId];
            //                if (lstExistingStats.Contains(stats))
            //                {
            //                    lstExistingStats.Remove(stats);
            //                    Settings.GetInstance().DictAgentGroupStatisitics[ObjectId] = lstExistingStats;
            //                }
            //            }
            //        }                   
            //    }               
            //}
            //catch(Exception GeneralException)
            //{
            //}

            try
            {
                logger.Debug("TabValueViewModel : UndoNewStatistics method - Entry");
                if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId) || Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                {


                    TempNewStatistics = new ObservableCollection<ObjectValues>();
                    TempNewStatistics = NewStatistics;
                    foreach (ObjectValues values in TempNewStatistics)
                    {
                        if (values.isGridChecked)
                        {
                            NewStatistics[NewStatistics.IndexOf(values)].isGridChecked = false;

                        }
                    }


                    if (Settings.GetInstance().DictUpdatedStatistics.ContainsKey(ObjectId))
                    {
                        if (Settings.GetInstance().DictUpdatedStatistics[ObjectId].Count > 0)
                        {
                            foreach (ObjectValues obj in Settings.GetInstance().DictUpdatedStatistics[ObjectId])
                            {
                                obj.isGridChecked = true;
                                ObjectStatistics.Add(obj);
                                NewStatistics.Remove(obj);
                                if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                                {
                                    Settings.GetInstance().DictAgentStatisitics[ObjectId].Add(obj.ObjectsNameTT.ToString());
                                }
                                else if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                                {
                                    Settings.GetInstance().DictAgentGroupStatisitics[ObjectId].Add(obj.ObjectsNameTT.ToString());
                                }
                                Settings.GetInstance().LstNewStatistics.Add(obj.ObjectsNameTT.ToString());
                            }
                        }
                    }

                    if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId))
                    {
                        Settings.GetInstance().CheckedStatList[ObjectId].Clear();
                    }
                    if (Settings.GetInstance().DictUpdatedStatistics.ContainsKey(ObjectId))
                    {
                        Settings.GetInstance().DictUpdatedStatistics[ObjectId].Clear();
                    }

                    //    List<string> lstTempStats = new List<string>();
                    //    lstTempStats = Settings.GetInstance().DictUpdatedStatistics[ObjectId];
                    //    foreach (string stats in lstTempStats)
                    //    {
                    //        foreach (ObjectValues values in ObjectStatistics)
                    //        {
                    //            if (stats == values.ObjectName)
                    //            {
                    //                ObjectStatistics.Remove(values);
                    //                break;
                    //            }
                    //        }

                    //        List<string> lstExistingStats = new List<string>();

                    //        if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                    //        {
                    //            lstExistingStats = Settings.GetInstance().DictAgentStatisitics[ObjectId];
                    //            if (lstExistingStats.Contains(stats))
                    //            {
                    //                lstExistingStats.Remove(stats);
                    //                Settings.GetInstance().DictAgentStatisitics[ObjectId] = lstExistingStats;
                    //            }
                    //        }
                    //        else if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                    //        {
                    //            lstExistingStats = Settings.GetInstance().DictAgentStatisitics[ObjectId];
                    //            if (lstExistingStats.Contains(stats))
                    //            {
                    //                lstExistingStats.Remove(stats);
                    //                Settings.GetInstance().DictAgentGroupStatisitics[ObjectId] = lstExistingStats;
                    //            }
                    //        }
                    //    }


                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("TabValueViewModel : UndoNewStatistics method - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : UndoNewStatistics method - Exit");
        }
        #endregion

        #region ObjectChecked
        public void ObjectChecked(object obj)
        {
            try
            {
                logger.Debug("TabValueViewModel : ObjectChecked method() - Entry");
                ObjectValues objectNew = obj as ObjectValues;
                ObjectStatistics.Remove(objectNew);
                NewStatistics.Add(objectNew);
                List<string> lstExistingStats = new List<string>();
                List<string> lstExistingObjects = new List<string>();


                if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                {
                    lstExistingStats = Settings.GetInstance().DictAgentStatisitics[ObjectId];
                    if (lstExistingStats.Contains(objectNew.ObjectsNameTT))
                    {
                        lstExistingStats.Remove(objectNew.ObjectsNameTT);
                        Settings.GetInstance().DictAgentStatisitics[ObjectId] = lstExistingStats;
                    }
                }
                else if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                {
                    lstExistingStats = Settings.GetInstance().DictAgentGroupStatisitics[ObjectId];
                    if (lstExistingStats.Contains(objectNew.ObjectsNameTT))
                    {
                        lstExistingStats.Remove(objectNew.ObjectsNameTT);
                        Settings.GetInstance().DictAgentGroupStatisitics[ObjectId] = lstExistingStats;
                    }
                }

                if (Settings.GetInstance().DictUpdatedStatistics.ContainsKey(ObjectId))
                {
                    Settings.GetInstance().DictUpdatedStatistics[ObjectId].Add(objectNew);
                }
                else
                {
                    Settings.GetInstance().DictUpdatedStatistics.Add(ObjectId, new List<ObjectValues>());
                    Settings.GetInstance().DictUpdatedStatistics[ObjectId].Add(objectNew);
                }

            }
            catch (Exception generalException)
            {
                logger.Error("TabValueViewModel : ObjectChecked method() - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : ObjectChecked method() - Exit");
        }
        #endregion

        #region SelectStatistics
        public void SelectStatistics(object obj)
        {
            try
            {
                logger.Debug("TabValueViewModel : SelectStatistics Method() - Entry");
                ObjectValues values = obj as ObjectValues;
                SelectedStatistics = values;
                if (values.isGridChecked)
                {
                    Settings.GetInstance().LstNewStatistics.Add(values.ObjectsNameTT);

                    if (values.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.acd.ToString()))
                    {
                        Views.MessageBox msgbox = new Views.MessageBox();
                        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();

                        if (Settings.GetInstance().IsConfigureLevelObjects)
                        {
                            Settings.GetInstance().IsConfigureLevelObjects = false;
                            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
                            if (NewObjectConfigVM == null)
                            {
                                NewObjectConfigVM = new ObjectConfigWindowViewModel();
                            }
                            NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.ACDQueue.ToString(), values.ObjectsNameTT.ToString(), ObjectId);
                            objSelectWindow.DataContext = NewObjectConfigVM;
                            objSelectWindow.ShowDialog();
                        }
                    }
                    else if (values.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.dn.ToString()))
                    {
                        Views.MessageBox msgbox = new Views.MessageBox();
                        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();

                        if (Settings.GetInstance().IsConfigureLevelObjects)
                        {
                            Settings.GetInstance().IsConfigureLevelObjects = false;
                            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
                            if (NewObjectConfigVM == null)
                            {
                                NewObjectConfigVM = new ObjectConfigWindowViewModel();
                            }
                            NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.DNGroup.ToString(), values.ObjectsNameTT.ToString(), ObjectId);
                            objSelectWindow.DataContext = NewObjectConfigVM;
                            objSelectWindow.ShowDialog();

                            //Settings.GetInstance().ObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.DNGroup.ToString(), values.ObjectsNameTT.ToString(), ObjectId);
                            //objSelectWindow.DataContext = Settings.GetInstance().ObjectConfigVM;
                            //objSelectWindow.ShowDialog();
                        }
                    }
                    else if (values.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.vq.ToString()))
                    {
                        Views.MessageBox msgbox = new Views.MessageBox();
                        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();

                        if (Settings.GetInstance().IsConfigureLevelObjects)
                        {
                            Settings.GetInstance().IsConfigureLevelObjects = false;
                            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
                            if (NewObjectConfigVM == null)
                            {
                                NewObjectConfigVM = new ObjectConfigWindowViewModel();
                            }
                            NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.VirtualQueue.ToString(), values.ObjectsNameTT.ToString(), ObjectId);
                            objSelectWindow.DataContext = NewObjectConfigVM;
                            objSelectWindow.ShowDialog();
                        }
                    }
                }
                else
                {
                    Settings.GetInstance().LstNewStatistics.Remove(values.ObjectsNameTT);
                    // Settings.GetInstance().NewStatisticsDictionary[object]
                }

                List<ObjectValues> CheckedStats = new List<ObjectValues>();
                foreach (ObjectValues Objvalues in NewStatistics)
                {
                    if (Objvalues.isGridChecked == true)
                    {
                        CheckedStats.Add(Objvalues);

                    }
                }
                if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId.ToString()))
                {
                    Settings.GetInstance().CheckedStatList[ObjectId].Clear();
                    foreach (ObjectValues objValue in CheckedStats)
                    {
                        Settings.GetInstance().CheckedStatList[ObjectId].Add(objValue);
                    }

                }
                else
                {
                    Settings.GetInstance().CheckedStatList.Add(ObjectId, CheckedStats);
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("TabValueViewModel : SelectStatistics Method() - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : SelectStatistics Method() - Exit");
            //try
            //{
            //    ObjectValues values = obj as ObjectValues;

            //    Settings.GetInstance().LstNewStatistics.Add(values.ObjectName);


            //    if (values.ObjectName.StartsWith(StatisticsEnum.SectionName.acd.ToString()))
            //    {
            //        Views.MessageBox msgbox = new Views.MessageBox();
            //        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
            //        msgbox.DataContext = mboxvmodel;
            //        msgbox.ShowDialog();

            //        if (Settings.GetInstance().IsConfigureLevelObjects)
            //        {
            //            Settings.GetInstance().IsConfigureLevelObjects = false;
            //            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
            //            ObjectConfigWindowViewModel objSelectVM = new ObjectConfigWindowViewModel(StatisticsEnum.ObjectType.ACDQueue.ToString(), values.ObjectName.ToString(), ObjectId);
            //            objSelectWindow.DataContext = objSelectVM;
            //            objSelectWindow.ShowDialog();
            //        }
            //    }
            //    else if (values.ObjectName.StartsWith(StatisticsEnum.SectionName.dn.ToString()))
            //    {
            //        Views.MessageBox msgbox = new Views.MessageBox();
            //        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
            //        msgbox.DataContext = mboxvmodel;
            //        msgbox.ShowDialog();

            //        if (Settings.GetInstance().IsConfigureLevelObjects)
            //        {
            //            Settings.GetInstance().IsConfigureLevelObjects = false;
            //            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
            //            ObjectConfigWindowViewModel objSelectVM = new ObjectConfigWindowViewModel(StatisticsEnum.ObjectType.DNGroup.ToString(), values.ObjectName.ToString(), ObjectId);
            //            objSelectWindow.DataContext = objSelectVM;
            //            objSelectWindow.ShowDialog();
            //        }
            //    }
            //    else if (values.ObjectName.StartsWith(StatisticsEnum.SectionName.vq.ToString()))
            //    {
            //        Views.MessageBox msgbox = new Views.MessageBox();
            //        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "This statistic requires a object to be configured. If no objects configured the default object from application will be used. Would you like to configure object ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
            //        msgbox.DataContext = mboxvmodel;
            //        msgbox.ShowDialog();

            //        if (Settings.GetInstance().IsConfigureLevelObjects)
            //        {
            //            Settings.GetInstance().IsConfigureLevelObjects = false;
            //            NewObjectConfigWindow objSelectWindow = new NewObjectConfigWindow();
            //            ObjectConfigWindowViewModel objSelectVM = new ObjectConfigWindowViewModel(StatisticsEnum.ObjectType.VirtualQueue.ToString(), values.ObjectName.ToString(), ObjectId);
            //            objSelectWindow.DataContext = objSelectVM;
            //            objSelectWindow.ShowDialog();
            //        }
            //    }
            //}
            //catch (Exception GeneralException)
            //{
            //}

        }
        #endregion

        #region SaveObjectValues
        public void SaveObjectValues()
        {
            List<string> lstUpdatedStats = new List<string>();
            try
            {
                logger.Debug("NewStatisticsWindowViewModel : SaveNewStatistics method() - Entry");

                foreach (string newStats in Settings.GetInstance().LstNewStatistics)
                {
                    lstUpdatedStats.Add(newStats);

                }
                Settings.GetInstance().LstNewStatistics.Clear();

                if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId.ToString()))
                {
                    if (Settings.GetInstance().CheckedStatList[ObjectId].Count > 0)
                    {

                        foreach (ObjectValues obj in Settings.GetInstance().CheckedStatList[ObjectId])
                        {
                            ObjectStatistics.Add(new ObjectValues() { isGridChecked = true, ObjectName = obj.ObjectName.Length > 38 ? obj.ObjectName.Substring(0, 33) + ".." : obj.ObjectName, ObjectsNameTT = obj.ObjectsNameTT, ObjectDescription = objStatSupport.GetDescription(obj.ObjectsNameTT).Length > 38 ? objStatSupport.GetDescription(obj.ObjectsNameTT).Substring(0, 36) + ".." : objStatSupport.GetDescription(obj.ObjectsNameTT), DescriptionTT = objStatSupport.GetDescription(obj.ObjectsNameTT) });

                            foreach (ObjectValues newObject in NewStatistics)
                            {
                                if (string.Compare(newObject.ObjectsNameTT, obj.ObjectsNameTT, true) == 0)
                                {
                                    NewStatistics.Remove(newObject);
                                    break;
                                }
                            }

                            if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                            {
                                Settings.GetInstance().DictAgentStatisitics[ObjectId].Add(obj.ObjectsNameTT.ToString());
                            }
                            if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                            {
                                Settings.GetInstance().DictAgentGroupStatisitics[ObjectId].Add(obj.ObjectsNameTT.ToString());
                            }
                        }

                    }

                }
                string Dilimitor = "_@";
                if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                {
                    if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                    {

                        List<string> ACDObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()])
                        {
                            ACDObjectListToRemove.Add(objectName);
                        }

                        if (ACDObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentStatisitics[ObjectId].Contains(tempArray[2].ToString()))
                                {
                                    ACDObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()].Clear();
                            foreach (string key in ACDObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()] = ACDObjectListToRemove;
                        }


                        List<string> DNObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()])
                        {
                            DNObjectListToRemove.Add(objectName);
                        }

                        if (DNObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentStatisitics[ObjectId].Contains(tempArray[1].ToString()))
                                {
                                    DNObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()].Clear();
                            foreach (string key in DNObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()] = DNObjectListToRemove;
                        }


                        List<string> VQObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()])
                        {
                            VQObjectListToRemove.Add(objectName);
                        }

                        if (VQObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentStatisitics[ObjectId].Contains(tempArray[2].ToString()))
                                {
                                    VQObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Clear();
                            foreach (string key in VQObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()] = VQObjectListToRemove;
                        }

                    }
                }
                else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                {
                    if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                    {
                        List<string> ACDObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()])
                        {
                            ACDObjectListToRemove.Add(objectName);
                        }

                        if (ACDObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentGroupStatisitics[ObjectId].Contains(tempArray[2].ToString()))
                                {
                                    ACDObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()].Clear();
                            foreach (string key in ACDObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.ACDQueue.ToString()] = ACDObjectListToRemove;
                        }


                        List<string> DNObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()])
                        {
                            DNObjectListToRemove.Add(objectName);
                        }

                        if (DNObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentGroupStatisitics[ObjectId].Contains(tempArray[1].ToString()))
                                {
                                    DNObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()].Clear();
                            foreach (string key in DNObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.DNGroup.ToString()] = DNObjectListToRemove;
                        }



                        List<string> VQObjectListToRemove = new List<string>();
                        foreach (string objectName in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()])
                        {
                            VQObjectListToRemove.Add(objectName);
                        }

                        if (VQObjectListToRemove.Count > 0)
                        {
                            foreach (string obj in Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()])
                            {
                                string[] tempArray = null;
                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                if (!Settings.GetInstance().DictAgentGroupStatisitics[ObjectId].Contains(tempArray[2].ToString()))
                                {
                                    VQObjectListToRemove.Remove(obj);
                                }

                            }
                            Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Clear();
                            foreach (string key in VQObjectListToRemove)
                            {
                                Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(key);
                            }
                            //Settings.GetInstance().DictAgentGroupObjects[ObjectId][StatisticsEnum.ObjectType.VirtualQueue.ToString()] = VQObjectListToRemove;
                        }

                    }
                }


                SaveNewStatistics();

                if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId))
                {
                    Settings.GetInstance().CheckedStatList[ObjectId].Clear();
                }
                if (Settings.GetInstance().DictUpdatedStatistics.ContainsKey(ObjectId))
                {
                    Settings.GetInstance().DictUpdatedStatistics[ObjectId].Clear();
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("NewStatisticsWindowViewModel : SaveNewStatistics method() - Exception caught" + GeneralException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("NewStatisticsWindowViewModel : SaveNewStatistics method() - Exit");
        }
        #endregion

        #region SaveNewStatistics
        public void SaveNewStatistics()
        {
            try
            {

                logger.Debug("TabValueViewModel : SaveNewStatistics method - Entry");
                Dictionary<string, Dictionary<string, List<string>>> TempStatObjectDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                Dictionary<string, List<string>> StatisticsDictionary = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> ObjectsDictionary = new Dictionary<string, List<string>>();
                StatisticsDictionary.Add(StatisticsEnum.ObjectType.Agent.ToString(), new List<string>());
                StatisticsDictionary.Add(StatisticsEnum.ObjectType.AgentGroup.ToString(), new List<string>());
                StatisticsDictionary.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                StatisticsDictionary.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                StatisticsDictionary.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());

                foreach (ObjectValues obj in Settings.GetInstance().ObjTabVm.ObjectStatistics)
                {
                    if (obj.isGridChecked)
                    {
                        if (obj.ObjectName.StartsWith("agent"))
                        {
                            StatisticsDictionary[StatisticsEnum.ObjectType.Agent.ToString()].Add(obj.ObjectsNameTT);
                        }
                        else if (obj.ObjectName.StartsWith("group"))
                        {
                            StatisticsDictionary[StatisticsEnum.ObjectType.AgentGroup.ToString()].Add(obj.ObjectsNameTT);
                        }
                        else if (obj.ObjectName.StartsWith("acd"))
                        {
                            StatisticsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(obj.ObjectsNameTT);
                        }
                        else if (obj.ObjectName.StartsWith("dn"))
                        {
                            StatisticsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Add(obj.ObjectsNameTT);
                        }
                        else if (obj.ObjectName.StartsWith("vq"))
                        {
                            StatisticsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(obj.ObjectsNameTT);
                        }

                    }
                }

                if (Settings.GetInstance().DictConfiguredStatisticsObjects.ContainsKey(ObjectId))
                {
                    Dictionary<string, Dictionary<string, List<string>>> ExistStatDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                    foreach (string key in Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId].Keys)
                    {
                        Dictionary<string, List<string>> dictConfigStatObjects = new Dictionary<string, List<string>>(Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId][key]);
                        ExistStatDictionary.Add(key, dictConfigStatObjects);
                    }
                    //ExistStatDictionary = Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId];
                    if (ExistStatDictionary.Keys.Contains("Statistics"))
                    {
                        ExistStatDictionary["Statistics"].Clear();
                        foreach (string key in StatisticsDictionary.Keys)
                        {
                            List<string> lstStats = new List<string>(StatisticsDictionary[key]);
                            ExistStatDictionary["Statistics"].Add(key, lstStats);
                        }
                        //ExistStatDictionary["Statistics"] = StatisticsDictionary;
                    }
                    else
                    {
                        Dictionary<string, List<string>> dictStats = new Dictionary<string, List<string>>(StatisticsDictionary);
                        ExistStatDictionary.Add("Statistics", dictStats);
                    }
                    if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                    {
                        if (ExistStatDictionary.Keys.Contains("Objects"))
                        {
                            ExistStatDictionary["Objects"].Clear();
                            foreach (string key in Settings.GetInstance().DictAgentObjects[ObjectId].Keys)
                            {
                                List<string> lstAgentObjects = new List<string>(Settings.GetInstance().DictAgentObjects[ObjectId][key]);
                                ExistStatDictionary["Objects"].Add(key, lstAgentObjects);
                            }
                        }
                        else
                        {
                            Dictionary<string, List<string>> dictAgentObjects = new Dictionary<string, List<string>>(Settings.GetInstance().DictAgentObjects[ObjectId]);
                            ExistStatDictionary.Add("Objects", dictAgentObjects);
                        }
                    }
                    else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                    {
                        if (ExistStatDictionary.Keys.Contains("Objects"))
                        {
                            ExistStatDictionary["Objects"].Clear();
                            foreach (string key in Settings.GetInstance().DictAgentGroupObjects[ObjectId].Keys)
                            {
                                List<string> lstAgentGroupObjects = new List<string>(Settings.GetInstance().DictAgentGroupObjects[ObjectId][key]);
                                ExistStatDictionary["Objects"].Add(key, lstAgentGroupObjects);
                            }
                        }
                        else
                        {
                            Dictionary<string, List<string>> dictAgentGroupObjects = new Dictionary<string, List<string>>(Settings.GetInstance().DictAgentGroupObjects[ObjectId]);
                            ExistStatDictionary.Add("Objects", dictAgentGroupObjects);
                        }
                    }


                    Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId].Clear();
                    foreach (string key in ExistStatDictionary.Keys)
                    {
                        Dictionary<string, List<string>> dictConfigStatObjects = new Dictionary<string, List<string>>(ExistStatDictionary[key]);
                        Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId].Add(key, dictConfigStatObjects);
                    }

                }
                else
                {
                    TempStatObjectDictionary.Add("Statistics", new Dictionary<string, List<string>>(StatisticsDictionary));
                    if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                    {
                        Dictionary<string, List<string>> dictAgentObjects = new Dictionary<string, List<string>>(Settings.GetInstance().DictAgentObjects[ObjectId]);
                        TempStatObjectDictionary.Add("Objects", dictAgentObjects);
                    }
                    else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                    {
                        Dictionary<string, List<string>> dictAgentGroupObjects = new Dictionary<string, List<string>>(Settings.GetInstance().DictAgentGroupObjects[ObjectId]);
                        TempStatObjectDictionary.Add("Objects", dictAgentGroupObjects);
                    }

                    Settings.GetInstance().DictConfiguredStatisticsObjects.Add(ObjectId, new Dictionary<string, Dictionary<string, List<string>>>(TempStatObjectDictionary));
                }


            }
            catch (Exception generalException)
            {
                logger.Error("TabValueViewModel : SaveNewStatistics method - Exception caught" + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("TabValueViewModel : SaveNewStatistics method - Exit");
        }
        #endregion

        #region LoadWindowData
        public void LoadWindowData(string Uid)
        {
            try
            {
                logger.Debug("TabValueViewModel : LoadWindowData Method- Entry");
                ObjectId = Uid;

                if (ObjectStatistics == null)
                    ObjectStatistics = new ObservableCollection<ObjectValues>();
                if (NewStatistics == null)
                    NewStatistics = new ObservableCollection<ObjectValues>();

                ObjectStatistics.Clear();
                NewStatistics.Clear();

                if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(Uid))
                {
                    Objecttype = StatisticsEnum.ObjectType.Agent.ToString();
                    LstAgentStatistics.Clear();
                    foreach (string key in Settings.GetInstance().DictAgentStatisitics[Uid])
                    {
                        LstAgentStatistics.Add(key);
                    }
                    //LstAgentStatistics = Settings.GetInstance().DictAgentStatisitics[Uid];

                    if (LstAgentStatistics == null || LstAgentStatistics.Count == 0)
                    {
                        foreach (string key in objStatBase.ReadStatistics(Uid, Objecttype))
                        {
                            LstAgentStatistics.Add(key);
                        }
                        //LstAgentStatistics = objStatBase.ReadStatistics(Uid, Objecttype);
                    }

                    foreach (string statistics in LstAgentStatistics.Distinct().ToList())
                    {

                        ObjectStatistics.Add(new ObjectValues() { isGridChecked = true, ObjectName = statistics.Length > 38 ? statistics.Substring(0, 35) + ".." : statistics, ObjectsNameTT = statistics, ObjectDescription = objStatSupport.GetDescription(statistics).Length > 38 ? objStatSupport.GetDescription(statistics).Substring(0, 36) + ".." : objStatSupport.GetDescription(statistics), DescriptionTT = objStatSupport.GetDescription(statistics) });

                    }
                    Settings.GetInstance().DictAgentStatisitics[Uid].Clear();
                    foreach (string key in LstAgentStatistics)
                    {
                        Settings.GetInstance().DictAgentStatisitics[Uid].Add(key);
                    }
                    //Settings.GetInstance().DictAgentStatisitics[Uid] = LstAgentStatistics;

                }
                else if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(Uid))
                {
                    Objecttype = StatisticsEnum.ObjectType.AgentGroup.ToString();
                    LstAgentGroupStatistics.Clear();
                    foreach (string key in Settings.GetInstance().DictAgentGroupStatisitics[Uid])
                    {
                        LstAgentGroupStatistics.Add(key);
                    }
                    //LstAgentGroupStatistics = Settings.GetInstance().DictAgentGroupStatisitics[Uid];

                    if (LstAgentGroupStatistics == null || LstAgentGroupStatistics.Count == 0)
                    {
                        foreach (string key in objStatBase.ReadStatistics(Uid, Objecttype))
                        {
                            LstAgentGroupStatistics.Add(key);
                        }
                        //LstAgentGroupStatistics = objStatBase.ReadStatistics(Uid, Objecttype);
                    }
                    foreach (string statistics in LstAgentGroupStatistics.Distinct().ToList())
                    {

                        ObjectStatistics.Add(new ObjectValues() { isGridChecked = true, ObjectName = statistics.Length > 38 ? statistics.Substring(0, 35) + ".." : statistics, ObjectsNameTT = statistics, ObjectDescription = objStatSupport.GetDescription(statistics).Length > 38 ? objStatSupport.GetDescription(statistics).Substring(0, 36) + ".." : objStatSupport.GetDescription(statistics), DescriptionTT = objStatSupport.GetDescription(statistics) });

                    }
                    Settings.GetInstance().DictAgentGroupStatisitics[Uid].Clear();
                    foreach (string key in LstAgentGroupStatistics)
                    {
                        Settings.GetInstance().DictAgentGroupStatisitics[Uid].Add(key);
                    }
                    //Settings.GetInstance().DictAgentGroupStatisitics[Uid] = LstAgentGroupStatistics;
                }


                List<string> lstExistingStats = new List<string>();
                foreach (ObjectValues objectNew in ObjectStatistics)
                {
                    lstExistingStats.Add(objectNew.ObjectsNameTT);

                }
                List<string> LstStatistics = new List<string>();

                foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                {
                    LstStatistics.Add(SectionName.ToString());
                }
                foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                {
                    foreach (string configuredStat in lstExistingStats)
                    {
                        if (string.Compare(SectionName, configuredStat, true) == 0)
                        {
                            LstStatistics.Remove(SectionName);
                        }
                    }
                }
                bool flag = true;
                foreach (string statistics in LstStatistics)
                {
                    if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId.ToString()))
                    {

                        foreach (ObjectValues value in Settings.GetInstance().CheckedStatList[ObjectId])
                        {
                            if (string.Compare(statistics, value.ObjectsNameTT, true) == 0)
                            {
                                NewStatistics.Add(new ObjectValues() { isGridChecked = true, ObjectName = statistics.Length > 38 ? statistics.Substring(0, 35) + ".." : statistics, ObjectsNameTT = statistics, ObjectDescription = objStatSupport.GetDescription(statistics).Length > 38 ? objStatSupport.GetDescription(statistics).Substring(0, 36) + ".." : objStatSupport.GetDescription(statistics), DescriptionTT = objStatSupport.GetDescription(statistics) });

                                flag = false;
                                break;
                            }

                        }
                        if (flag)

                            NewStatistics.Add(new ObjectValues() { isGridChecked = false, ObjectName = statistics.Length > 38 ? statistics.Substring(0, 35) + ".." : statistics, ObjectsNameTT = statistics, ObjectDescription = objStatSupport.GetDescription(statistics).Length > 38 ? objStatSupport.GetDescription(statistics).Substring(0, 36) + ".." : objStatSupport.GetDescription(statistics), DescriptionTT = objStatSupport.GetDescription(statistics) });

                        flag = true;

                    }
                    else
                    {
                        NewStatistics.Add(new ObjectValues() { isGridChecked = false, ObjectName = statistics.Length > 38 ? statistics.Substring(0, 35) + ".." : statistics, ObjectsNameTT = statistics, ObjectDescription = objStatSupport.GetDescription(statistics).Length > 38 ? objStatSupport.GetDescription(statistics).Substring(0, 36) + ".." : objStatSupport.GetDescription(statistics), DescriptionTT = objStatSupport.GetDescription(statistics) });

                    }


                }

                //if (Settings.GetInstance().CheckedStatList.ContainsKey(ObjectId.ToString()))
                //{
                //    foreach (ObjectValues value in Settings.GetInstance().CheckedStatList[ObjectId])
                //    {
                //       NewStatistics[NewStatistics.IndexOf(value)].isGridChecked = true;
                //    }
                //}


                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];

            }
            catch (Exception generalException)
            {
                logger.Debug("TabValueViewModel : LoadWindowData Method- Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("TabValueViewModel : LoadWindowData Method- Exit");
        }
        #endregion

        #endregion
    }
}

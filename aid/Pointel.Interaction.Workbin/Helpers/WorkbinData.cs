/*
 * ======================================================
 * Pointel.Interaction.Workbin.Helpers.WorkbinData
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class WorkbinData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<WorkbinMailDetails> _workbinMailDetails = new ObservableCollection<WorkbinMailDetails>();
        private List<WorkbinMailDetails> _selectedWorkbinMailDetails = new List<WorkbinMailDetails>();

        public ObservableCollection<WorkbinMailDetails> WorkbinDetails
        {
            get
            {
                return _workbinMailDetails;
            }
            set
            {
                if (value != null)
                {
                    _workbinMailDetails = value;
                }
            }
        }

        public List<WorkbinMailDetails> SelectedWorkbinDetails
        {
            get
            {
                return _selectedWorkbinMailDetails;
            }
            set
            {
                if (value != null)
                {
                    _selectedWorkbinMailDetails = value;
                    RaisePropertyChanged(() => SelectedWorkbinDetails);
                }
            }
        }

        private List<WorkbinMenuItem> workbinMenu = new List<WorkbinMenuItem>();

        public List<WorkbinMenuItem> WorkbinMenu
        {
            get
            {
                return workbinMenu;
            }
            set
            {
                if (value != null)
                {
                    workbinMenu = value;
                    RaisePropertyChanged(() => WorkbinMenu);
                }
            }
        }

        private List<WorkbinMenuItem> teamworkbinMenu = new List<WorkbinMenuItem>();

        public List<WorkbinMenuItem> MyTeamWorkbinMenu
        {
            get
            {
                return teamworkbinMenu;
            }
            set
            {
                if (value != null)
                {
                    teamworkbinMenu = value;
                    RaisePropertyChanged(() => MyTeamWorkbinMenu);
                }
            }
        }

        private List<IQMenuItem> listIQMenuItem;

        public List<IQMenuItem> ListIQMenuItem
        {
            get
            {
                return listIQMenuItem;
            }
            set
            {
                if (value != null)
                {
                    listIQMenuItem = value;
                    RaisePropertyChanged(() => ListIQMenuItem);
                }
            }
        }

        private Visibility iqVisiblity =Visibility.Collapsed;
        public Visibility IQVisiblity
        {
            get
            {
                return iqVisiblity;
            }
            set
            {
                if (value != null)
                {
                    iqVisiblity = value;
                    RaisePropertyChanged(() => iqVisiblity);
                }
            }
        }

    }
}

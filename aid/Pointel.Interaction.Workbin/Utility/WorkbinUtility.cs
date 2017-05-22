/*
 * ======================================================
 * Pointel.Interaction.Workbin.Utility.WorkbinUtility
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
//using Pointel.Interactions.Email.Control;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Pointel.Interactions.IPlugins;

namespace Pointel.Interaction.Workbin.Utility
{
    public class WorkbinUtility : INotifyPropertyChanged
    {
        private static Pointel.Interaction.Workbin.Utility.WorkbinUtility emailUtilityInstance = new Pointel.Interaction.Workbin.Utility.WorkbinUtility();
        public static  Pointel.Interaction.Workbin.Utility.WorkbinUtility Instance()
        {
            return emailUtilityInstance;
        }        
        
        public  IPluginCallBack messageToClientEmail = null;       
        //private ConfService comObject;
       // private string appName = string.Empty;
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
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

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private  string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        public int IxnProxyID
        {
            set;
            get;
        }

        public Dictionary<string, string> PersonalWorkbinList = new Dictionary<string, string>();

        public Dictionary<string, string> TeamWorkbinList = new Dictionary<string, string>();

        public System.Windows.Controls.ContextMenu contextMenuTransfer;
            
        public IPluginCallBack configListener;

        public string PlaceID
        {
            get;
            set;
        }

        public List<CfgAgentGroup> AgentGroup
        {
            get;
            set;
        }     

        public bool IsWorkbinEnable { get; set; }

        public bool IsAgentLoginIXN
        {
            get;
            set;
        }

        public Dictionary<string, string> Workbins
        {
            get;
            set;
        }
        public bool IsSuperadmin
        { 
            get;
            set; 
        }

        public bool IsContactServerActive
        {
            get;
            set;
        }

        public bool IsInteractionServerActive
        {
            get;
            set;
        }

        public string SelectedWorkbinName
        {
            get;
            set;
        }
    }
}
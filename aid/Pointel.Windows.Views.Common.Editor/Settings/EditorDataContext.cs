using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Pointel.HTML.Text.Editor.Settings
{
    public class EditorDataContext : INotifyPropertyChanged
    {
        //#region Single instance

        //private static EditorDataContext _instance = null;

        //public static EditorDataContext GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new EditorDataContext();
        //        return _instance;
        //    }
        //    return _instance;
        //}

        //#endregion Single instance

        #region Private Members
        private FlowDocument _rtbDocument = new FlowDocument();
        private string _htmlData = string.Empty;
        private System.Windows.Visibility _showorHideHTMLControls = System.Windows.Visibility.Visible;
        #endregion

        #region Public Members
        public event PropertyChangedEventHandler PropertyChanged;
        public ContextMenu contextMenuUC = new System.Windows.Controls.ContextMenu();
        #endregion

        #region INotifyPropertyChange

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

        //private void onPropertyChanged(EditorDataContext editorDataContext, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion INotifyPropertyChabge

        #region Properties
        public FlowDocument RTBDocument
        {
            get
            {
                return _rtbDocument;
            }
            set
            {
                if (_rtbDocument != value)
                {
                    _rtbDocument = value;
                    RaisePropertyChanged(() => RTBDocument);
                }
            }
        }
        public string HTMLData
        {
            get
            {
                return _htmlData;
            }
            set
            {
                if (_htmlData != value)
                {
                    _htmlData = value;
                    RaisePropertyChanged(() => HTMLData);
                }
            }
        }
        public System.Windows.Visibility ShoworHideHTMLControls
        {
            get
            {
                return _showorHideHTMLControls;
            }
            set
            {
                if (_showorHideHTMLControls != value)
                {
                    _showorHideHTMLControls = value;
                    RaisePropertyChanged(() => ShoworHideHTMLControls);
                }
            }
        }
        #endregion
    }
}

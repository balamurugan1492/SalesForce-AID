﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Pointel.Interactions.Email.DataContext
{
    public class EmailStateDataContext : INotifyPropertyChanged
    {
        private static EmailStateDataContext objEmailState = null;

        private EmailStateDataContext()
        {

        }

        public static EmailStateDataContext GetInstance()
        {
            if (objEmailState == null)
                objEmailState = new EmailStateDataContext();
            return objEmailState;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool isConsultEnabled = true;


        public bool IsConsultEnabled
        {
            get
            {
                return isConsultEnabled;
            }
            set
            {
                if (isConsultEnabled != value)
                {
                    isConsultEnabled = value;
                    RaisePropertyChanged(()=>IsConsultEnabled);
                }
            }
        }

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
    }
}

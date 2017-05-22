using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace StatTickerFive.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private Action<object> _action;
        private readonly Func<bool> _canExecute;
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public DelegateCommand(Action<object> action)
        {
            _action = action;
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if(_command==null)
                _action(parameter);
            else
                _command();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
       

    }


    #region EventBehaviourFactory Class

    /// <summary>
    /// 
    /// </summary>
    public class EventBehaviourFactory
    {
        public static DependencyProperty CreateCommandExecutionEventBehaviour(RoutedEvent routedEvent, string propertyName, Type ownerType)
        {
            DependencyProperty property = DependencyProperty.RegisterAttached(propertyName, typeof(ICommand), ownerType,
                                                               new PropertyMetadata(null,
                                                                   new ExecuteCommandOnRoutedEventBehaviour(routedEvent).PropertyChangedHandler));

            return property;
        }

        /// <summary>
        /// An internal class to handle listening for an event and executing a command,
        /// when a Command is assigned to a particular DependencyProperty
        /// </summary>
        private class ExecuteCommandOnRoutedEventBehaviour : ExecuteCommandBehaviour
        {
            private readonly RoutedEvent _routedEvent;

            public ExecuteCommandOnRoutedEventBehaviour(RoutedEvent routedEvent)
            {
                _routedEvent = routedEvent;
            }

            /// <summary>
            /// Handles attaching or Detaching Event handlers when a Command is assigned or unassigned
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            protected override void AdjustEventHandlers(DependencyObject sender, object oldValue, object newValue)
            {
                UIElement element = sender as UIElement;
                if (element == null) { return; }

                if (oldValue != null)
                {
                    element.RemoveHandler(_routedEvent, new RoutedEventHandler(EventHandler));
                }

                if (newValue != null)
                {
                    element.AddHandler(_routedEvent, new RoutedEventHandler(EventHandler));
                }
            }

            protected void EventHandler(object sender, RoutedEventArgs e)
            {
                HandleEvent(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal abstract class ExecuteCommandBehaviour
        {
            protected DependencyProperty _property;
            protected abstract void AdjustEventHandlers(DependencyObject sender, object oldValue, object newValue);

            protected void HandleEvent(object sender, EventArgs e)
            {
                DependencyObject dp = sender as DependencyObject;
                if (dp == null)
                {
                    return;
                }

                ICommand command = dp.GetValue(_property) as ICommand;

                if (command == null)
                {
                    return;
                }

                if (command.CanExecute(e))
                {
                    command.Execute(e);
                }
            }

            /// <summary>
            /// Listens for a change in the DependencyProperty that we are assigned to, and
            /// adjusts the EventHandlers accordingly
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void PropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                // the first time the property changes,
                // make a note of which property we are supposed
                // to be watching
                if (_property == null)
                {
                    _property = e.Property;
                }

                object oldValue = e.OldValue;
                object newValue = e.NewValue;

                AdjustEventHandlers(sender, oldValue, newValue);
            }
        }
    }

    #endregion


    #region Password Helper Class

    /// <summary>
    /// 
    /// </summary>
    public static class PasswordHelper
    {
        
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));


        /// <summary>
        /// Sets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Gets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">The value.</param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// Gets the is updating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        /// <summary>
        /// Sets the is updating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        /// <summary>
        /// Called when [password property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        /// <summary>
        /// Attaches the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Passwords the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
    
    #endregion


}
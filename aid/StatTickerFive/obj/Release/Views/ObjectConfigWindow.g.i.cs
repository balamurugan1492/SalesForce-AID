﻿#pragma checksum "..\..\..\Views\ObjectConfigWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DEEFA63731C9BC0978D3A439FA8DCF1D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using StatTickerFive.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace StatTickerFive.Views {
    
    
    /// <summary>
    /// ObjectConfigWindow
    /// </summary>
    public partial class ObjectConfigWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal StatTickerFive.Views.ObjectConfigWindow QueueConfig;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl AdminTabControl;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem ApplicationTabItem;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbObjectType;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid StatisticsGridView;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.FrameworkElement ProxyElement;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ObjectsGridView;
        
        #line default
        #line hidden
        
        
        #line 228 "..\..\..\Views\ObjectConfigWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SaveSettings;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/StatTickerFive;component/views/objectconfigwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\ObjectConfigWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.QueueConfig = ((StatTickerFive.Views.ObjectConfigWindow)(target));
            return;
            case 2:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.AdminTabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 4:
            this.ApplicationTabItem = ((System.Windows.Controls.TabItem)(target));
            return;
            case 5:
            this.cmbObjectType = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.StatisticsGridView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.ProxyElement = ((System.Windows.FrameworkElement)(target));
            return;
            case 8:
            this.ObjectsGridView = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            this.SaveSettings = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


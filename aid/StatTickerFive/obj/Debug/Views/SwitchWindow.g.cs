﻿#pragma checksum "..\..\..\Views\SwitchWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "64119B10AAC6C94D36DAABD6E445BEE8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// SwitchWindow
    /// </summary>
    public partial class SwitchWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 40 "..\..\..\Views\SwitchWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\Views\SwitchWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox SwitchListBox;
        
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
            System.Uri resourceLocater = new System.Uri("/StatTickerFive;component/views/switchwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\SwitchWindow.xaml"
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
            
            #line 9 "..\..\..\Views\SwitchWindow.xaml"
            ((StatTickerFive.Views.SwitchWindow)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.SwitchWindow_OnMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Views\SwitchWindow.xaml"
            ((StatTickerFive.Views.SwitchWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.SwitchWindow_OnLoaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Views\SwitchWindow.xaml"
            ((StatTickerFive.Views.SwitchWindow)(target)).Activated += new System.EventHandler(this.SwitchWindow_OnActivated);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Views\SwitchWindow.xaml"
            ((StatTickerFive.Views.SwitchWindow)(target)).Deactivated += new System.EventHandler(this.SwitchWindow_OnDeactivated);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            return;
            case 3:
            this.SwitchListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            
            #line 55 "..\..\..\Views\SwitchWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


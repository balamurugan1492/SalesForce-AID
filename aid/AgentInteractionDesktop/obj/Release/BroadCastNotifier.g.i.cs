﻿#pragma checksum "..\..\BroadCastNotifier.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5633BC3B14AAF553B9C6B4FFAAFF33A4"
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
using Pointel.TaskbarNotifier;
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


namespace Agent.Interaction.Desktop {
    
    
    /// <summary>
    /// BroadCastNotifier
    /// </summary>
    public partial class BroadCastNotifier : Pointel.TaskbarNotifier.TaskbarNotifier, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\BroadCastNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\BroadCastNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grd_Details;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\BroadCastNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel Details;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\BroadCastNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Show;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\BroadCastNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Dismiss;
        
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
            System.Uri resourceLocater = new System.Uri("/Agent.Interaction.Desktop;component/broadcastnotifier.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BroadCastNotifier.xaml"
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
            
            #line 6 "..\..\BroadCastNotifier.xaml"
            ((Agent.Interaction.Desktop.BroadCastNotifier)(target)).Loaded += new System.Windows.RoutedEventHandler(this.BroadCastNotifier_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.grd_Details = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.Details = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.btn_Show = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\BroadCastNotifier.xaml"
            this.btn_Show.Click += new System.Windows.RoutedEventHandler(this.Show_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btn_Dismiss = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\BroadCastNotifier.xaml"
            this.btn_Dismiss.Click += new System.Windows.RoutedEventHandler(this.Dismiss_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\..\WinForms\Notifier.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7673F0543839AB69A9A97F28F9EC4C04"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
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


namespace Pointel.Interactions.Outbound.WinForms {
    
    
    /// <summary>
    /// Notifier
    /// </summary>
    public partial class Notifier : Pointel.TaskbarNotifier.TaskbarNotifier, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\WinForms\Notifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\WinForms\Notifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid expGrid;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\WinForms\Notifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Windows.Controls.DataGrid DGChatCaseData;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\WinForms\Notifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRight;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\WinForms\Notifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLeft;
        
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
            System.Uri resourceLocater = new System.Uri("/Pointel.Interactions.Outbound;component/winforms/notifier.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WinForms\Notifier.xaml"
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
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 2:
            
            #line 30 "..\..\..\WinForms\Notifier.xaml"
            ((System.Windows.Controls.Expander)(target)).Expanded += new System.Windows.RoutedEventHandler(this.Expander_Expanded);
            
            #line default
            #line hidden
            
            #line 30 "..\..\..\WinForms\Notifier.xaml"
            ((System.Windows.Controls.Expander)(target)).Collapsed += new System.Windows.RoutedEventHandler(this.Expander_Collapsed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.expGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.DGChatCaseData = ((Microsoft.Windows.Controls.DataGrid)(target));
            return;
            case 5:
            this.btnRight = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\WinForms\Notifier.xaml"
            this.btnRight.Click += new System.Windows.RoutedEventHandler(this.Right_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnLeft = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\..\WinForms\Notifier.xaml"
            this.btnLeft.Click += new System.Windows.RoutedEventHandler(this.Left_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


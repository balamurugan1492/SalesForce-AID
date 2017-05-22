﻿#pragma checksum "..\..\..\Forms\EmailNotifier.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E34574F31F4228B7A1FA5D2621AA2BEA"
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


namespace Pointel.Interactions.Email.Forms {
    
    
    /// <summary>
    /// EmailNotifier
    /// </summary>
    public partial class EmailNotifier : Pointel.TaskbarNotifier.TaskbarNotifier, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition grdTitle;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid expGrid;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Windows.Controls.DataGrid dgEmailCaseData;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAccept;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Forms\EmailNotifier.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReject;
        
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
            System.Uri resourceLocater = new System.Uri("/Pointel.Interactions.Email;component/forms/emailnotifier.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\EmailNotifier.xaml"
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
            
            #line 7 "..\..\..\Forms\EmailNotifier.xaml"
            ((Pointel.Interactions.Email.Forms.EmailNotifier)(target)).Loaded += new System.Windows.RoutedEventHandler(this.TaskbarNotifier_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\Forms\EmailNotifier.xaml"
            ((Pointel.Interactions.Email.Forms.EmailNotifier)(target)).KeyUp += new System.Windows.Input.KeyEventHandler(this.TaskbarNotifier_KeyUp);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\Forms\EmailNotifier.xaml"
            ((Pointel.Interactions.Email.Forms.EmailNotifier)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TaskbarNotifier_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.grdTitle = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 4:
            
            #line 35 "..\..\..\Forms\EmailNotifier.xaml"
            ((System.Windows.Controls.Expander)(target)).Expanded += new System.Windows.RoutedEventHandler(this.Expander_Expanded);
            
            #line default
            #line hidden
            
            #line 35 "..\..\..\Forms\EmailNotifier.xaml"
            ((System.Windows.Controls.Expander)(target)).Collapsed += new System.Windows.RoutedEventHandler(this.Expander_Collapsed);
            
            #line default
            #line hidden
            return;
            case 5:
            this.expGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.dgEmailCaseData = ((Microsoft.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.btnAccept = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\Forms\EmailNotifier.xaml"
            this.btnAccept.Click += new System.Windows.RoutedEventHandler(this.btnAccept_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnReject = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\..\Forms\EmailNotifier.xaml"
            this.btnReject.Click += new System.Windows.RoutedEventHandler(this.btnReject_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


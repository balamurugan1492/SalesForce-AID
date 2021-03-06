﻿#pragma checksum "..\..\..\Forms\MessageBox.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D48F849A37F234B2BE37256CCA3F235E"
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
    /// MessageBox
    /// </summary>
    public partial class MessageBox : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainBorder;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTitle;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RowDefinition growFwd;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtblockContent;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblForward;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ForwardError;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_right;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Forms\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_left;
        
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
            System.Uri resourceLocater = new System.Uri("/Pointel.Interactions.Email;component/forms/messagebox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\MessageBox.xaml"
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
            
            #line 5 "..\..\..\Forms\MessageBox.xaml"
            ((Pointel.Interactions.Email.Forms.MessageBox)(target)).Activated += new System.EventHandler(this.Window_Activated);
            
            #line default
            #line hidden
            
            #line 6 "..\..\..\Forms\MessageBox.xaml"
            ((Pointel.Interactions.Email.Forms.MessageBox)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 6 "..\..\..\Forms\MessageBox.xaml"
            ((Pointel.Interactions.Email.Forms.MessageBox)(target)).Deactivated += new System.EventHandler(this.Window_Deactivated);
            
            #line default
            #line hidden
            
            #line 6 "..\..\..\Forms\MessageBox.xaml"
            ((Pointel.Interactions.Email.Forms.MessageBox)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.lblTitle = ((System.Windows.Controls.Label)(target));
            
            #line 22 "..\..\..\Forms\MessageBox.xaml"
            this.lblTitle.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.lblTitle_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.growFwd = ((System.Windows.Controls.RowDefinition)(target));
            return;
            case 5:
            this.txtblockContent = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.lblForward = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.ForwardError = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.btn_right = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\Forms\MessageBox.xaml"
            this.btn_right.Click += new System.Windows.RoutedEventHandler(this.btn_right_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btn_left = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\..\Forms\MessageBox.xaml"
            this.btn_left.Click += new System.Windows.RoutedEventHandler(this.btn_left_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


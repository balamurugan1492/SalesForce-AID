﻿#pragma checksum "..\..\..\..\Controls\HyperLinkPicker.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E76770A93D25279D10EC12BA99B8B2CD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
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


namespace Pointel.Windows.Views.Common.Editor.Controls {
    
    
    /// <summary>
    /// HyperLinkPicker
    /// </summary>
    public partial class HyperLinkPicker : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 75 "..\..\..\..\Controls\HyperLinkPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\..\Controls\HyperLinkPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Controls\HyperLinkPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblError;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\..\Controls\HyperLinkPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtURL;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\Controls\HyperLinkPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtText;
        
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
            System.Uri resourceLocater = new System.Uri("/Pointel.Windows.Views.Common.Editor;component/controls/hyperlinkpicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\HyperLinkPicker.xaml"
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
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 75 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 76 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblError = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.txtURL = ((System.Windows.Controls.TextBox)(target));
            
            #line 96 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.txtURL.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp);
            
            #line default
            #line hidden
            
            #line 96 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.txtURL.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtText = ((System.Windows.Controls.TextBox)(target));
            
            #line 99 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.txtText.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp);
            
            #line default
            #line hidden
            
            #line 99 "..\..\..\..\Controls\HyperLinkPicker.xaml"
            this.txtText.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


﻿#pragma checksum "..\..\StartGpssWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "20AEFCB2EAA6ED85792102BE5EDD97D8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
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


namespace MyFirstWPF {
    
    
    /// <summary>
    /// StartGpssWindow
    /// </summary>
    public partial class StartGpssWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\StartGpssWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StartGenButton;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\StartGpssWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelGenButton;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\StartGpssWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ModelingTimeTextBox;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\StartGpssWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ObservationTimeTextBox;
        
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
            System.Uri resourceLocater = new System.Uri("/MyFirstWPF;component/startgpsswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\StartGpssWindow.xaml"
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
            this.StartGenButton = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\StartGpssWindow.xaml"
            this.StartGenButton.Click += new System.Windows.RoutedEventHandler(this.StartGenButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CancelGenButton = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\StartGpssWindow.xaml"
            this.CancelGenButton.Click += new System.Windows.RoutedEventHandler(this.CancelGenButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ModelingTimeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 37 "..\..\StartGpssWindow.xaml"
            this.ModelingTimeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.IntNumberValidation);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ObservationTimeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 43 "..\..\StartGpssWindow.xaml"
            this.ObservationTimeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.IntNumberValidation);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


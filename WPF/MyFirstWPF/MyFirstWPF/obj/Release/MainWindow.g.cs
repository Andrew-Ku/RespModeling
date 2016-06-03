﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F87269DF07F261FA4881C87E6075A45F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MyFirstWPF.Models;
using MyFirstWPF.Validation;
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
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 29 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel dockPanel1;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NewButton;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenButton;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveButton;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MoveXPosLabel;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MoveYPosLabel;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid EditNodeGrid;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NodeNumberTextBox;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox StartNodeCheckBox;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox RejectionNodeCheckBox;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid NodeRelationDataGrid;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel RightStackPanel;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label StateCountLabel;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label EdgeCountLabel;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GenerateButton;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer WorkPlaceScrollViewer;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas WorkPlaceCanvas;
        
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
            System.Uri resourceLocater = new System.Uri("/MyFirstWPF;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
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
            this.dockPanel1 = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 2:
            this.NewButton = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\MainWindow.xaml"
            this.NewButton.Click += new System.Windows.RoutedEventHandler(this.NewButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 3:
            this.OpenButton = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\MainWindow.xaml"
            this.OpenButton.Click += new System.Windows.RoutedEventHandler(this.OpenButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 4:
            this.SaveButton = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\MainWindow.xaml"
            this.SaveButton.Click += new System.Windows.RoutedEventHandler(this.SaveButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MoveXPosLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.MoveYPosLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.EditNodeGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            this.NodeNumberTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 64 "..\..\MainWindow.xaml"
            this.NodeNumberTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NodeNumberValidation);
            
            #line default
            #line hidden
            return;
            case 9:
            this.StartNodeCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.RejectionNodeCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            
            #line 72 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveEditNodeButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 12:
            this.NodeRelationDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 94 "..\..\MainWindow.xaml"
            this.NodeRelationDataGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.NodeRelationDataGrid_OnPreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 13:
            this.RightStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 14:
            this.StateCountLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 15:
            this.EdgeCountLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.GenerateButton = ((System.Windows.Controls.Button)(target));
            
            #line 133 "..\..\MainWindow.xaml"
            this.GenerateButton.Click += new System.Windows.RoutedEventHandler(this.GenerateButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 17:
            this.WorkPlaceScrollViewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 18:
            this.WorkPlaceCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 142 "..\..\MainWindow.xaml"
            this.WorkPlaceCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.WorkPlaceCanvas_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 143 "..\..\MainWindow.xaml"
            this.WorkPlaceCanvas.MouseMove += new System.Windows.Input.MouseEventHandler(this.WorkPlaceCanvas_MouseMove);
            
            #line default
            #line hidden
            
            #line 144 "..\..\MainWindow.xaml"
            this.WorkPlaceCanvas.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.WorkPlaceCanvas_OnMouseWheel);
            
            #line default
            #line hidden
            
            #line 145 "..\..\MainWindow.xaml"
            this.WorkPlaceCanvas.SizeChanged += new System.Windows.SizeChangedEventHandler(this.WorkPlaceCanvas_OnSizeChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

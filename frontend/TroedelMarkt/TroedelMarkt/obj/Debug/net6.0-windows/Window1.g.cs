﻿#pragma checksum "..\..\..\Window1.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A0E198FD1A33751995426DA56CA8FE42A9FB9668"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using TroedelMarkt;


namespace TroedelMarkt {
    
    
    /// <summary>
    /// Window1
    /// </summary>
    public partial class Window1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DGTrader;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ExportCSV;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TBoxTraderID;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnAddTdr;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnDelTdr;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TBlockSumm;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TBlockProvSumm;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TBlockDebug;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnUpdateData;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Window1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnUpdateTraders;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TroedelMarkt;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Window1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\Window1.xaml"
            ((TroedelMarkt.Window1)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DGTrader = ((System.Windows.Controls.DataGrid)(target));
            
            #line 18 "..\..\..\Window1.xaml"
            this.DGTrader.CellEditEnding += new System.EventHandler<System.Windows.Controls.DataGridCellEditEndingEventArgs>(this.DGCellEditEnd);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ExportCSV = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\..\Window1.xaml"
            this.ExportCSV.Click += new System.Windows.RoutedEventHandler(this.ExportCSV_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.TBoxTraderID = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.BtnAddTdr = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\Window1.xaml"
            this.BtnAddTdr.Click += new System.Windows.RoutedEventHandler(this.BtnAddTdr_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnDelTdr = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\Window1.xaml"
            this.BtnDelTdr.Click += new System.Windows.RoutedEventHandler(this.BtnDelTdr_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.TBlockSumm = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.TBlockProvSumm = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.TBlockDebug = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.BtnUpdateData = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\Window1.xaml"
            this.BtnUpdateData.Click += new System.Windows.RoutedEventHandler(this.BtnUpdateData_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.BtnUpdateTraders = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\Window1.xaml"
            this.BtnUpdateTraders.Click += new System.Windows.RoutedEventHandler(this.BtnUpdateTraders_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


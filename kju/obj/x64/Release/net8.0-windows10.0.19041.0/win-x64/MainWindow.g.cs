﻿#pragma checksum "C:\git\Kyoo\Kyoo\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "00ADCF90045E058588254B29123A00F2B59999D5ECC50BCFFCCD30F716F650EF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kju
{
    partial class MainWindow : 
        global::Microsoft.UI.Xaml.Window, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_WinUI_TableView_TableView_ItemsSource(global::WinUI.TableView.TableView obj, global::System.Collections.IList value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Collections.IList) global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Collections.IList), targetNullValue);
                }
                obj.ItemsSource = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private partial class MainWindow_obj1_Bindings :
            global::Microsoft.UI.Xaml.Markup.IComponentConnector,
            IMainWindow_Bindings
        {
            private global::kju.MainWindow dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::WinUI.TableView.TableView obj8;

            private MainWindow_obj1_BindingsTracking bindingsTracking;

            public MainWindow_obj1_Bindings()
            {
                this.bindingsTracking = new MainWindow_obj1_BindingsTracking(this);
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 8: // MainWindow.xaml line 61
                        this.obj8 = global::WinRT.CastExtensions.As<global::WinUI.TableView.TableView>(target);
                        break;
                    default:
                        break;
                }
            }
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
                        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target) 
                        {
                            return null;
                        }

            // IMainWindow_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
                this.bindingsTracking.ReleaseAllListeners();
                this.initialized = false;
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                this.bindingsTracking.ReleaseAllListeners();
                if (newDataRoot != null)
                {
                    this.dataRoot = global::WinRT.CastExtensions.As<global::kju.MainWindow>(newDataRoot);
                    return true;
                }
                return false;
            }

            public void Activated(object obj, global::Microsoft.UI.Xaml.WindowActivatedEventArgs data)
            {
                this.Initialize();
            }

            public void Loading(global::Microsoft.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::kju.MainWindow obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_Cues(obj.Cues, phase);
                    }
                }
            }
            private void Update_Cues(global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue> obj, int phase)
            {
                this.bindingsTracking.UpdateChildListeners_Cues(obj);
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // MainWindow.xaml line 61
                    XamlBindingSetters.Set_WinUI_TableView_TableView_ItemsSource(this.obj8, obj, null);
                }
            }

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            private class MainWindow_obj1_BindingsTracking
            {
                private global::System.WeakReference<MainWindow_obj1_Bindings> weakRefToBindingObj; 

                public MainWindow_obj1_BindingsTracking(MainWindow_obj1_Bindings obj)
                {
                    weakRefToBindingObj = new global::System.WeakReference<MainWindow_obj1_Bindings>(obj);
                }

                public MainWindow_obj1_Bindings TryGetBindingObject()
                {
                    MainWindow_obj1_Bindings bindingObject = null;
                    if (weakRefToBindingObj != null)
                    {
                        weakRefToBindingObj.TryGetTarget(out bindingObject);
                        if (bindingObject == null)
                        {
                            weakRefToBindingObj = null;
                            ReleaseAllListeners();
                        }
                    }
                    return bindingObject;
                }

                public void ReleaseAllListeners()
                {
                    UpdateChildListeners_Cues(null);
                }

                public void PropertyChanged_Cues(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
                {
                    MainWindow_obj1_Bindings bindings = TryGetBindingObject();
                    if (bindings != null)
                    {
                        string propName = e.PropertyName;
                        global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue> obj = sender as global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue>;
                        if (global::System.String.IsNullOrEmpty(propName))
                        {
                        }
                        else
                        {
                            switch (propName)
                            {
                                default:
                                    break;
                            }
                        }
                    }
                }
                public void CollectionChanged_Cues(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                {
                    MainWindow_obj1_Bindings bindings = TryGetBindingObject();
                    if (bindings != null)
                    {
                        global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue> obj = sender as global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue>;
                    }
                }
                private global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue> cache_Cues = null;
                public void UpdateChildListeners_Cues(global::System.Collections.ObjectModel.ObservableCollection<global::kju.AudioCue> obj)
                {
                    if (obj != cache_Cues)
                    {
                        if (cache_Cues != null)
                        {
                            ((global::System.ComponentModel.INotifyPropertyChanged)cache_Cues).PropertyChanged -= PropertyChanged_Cues;
                            ((global::System.Collections.Specialized.INotifyCollectionChanged)cache_Cues).CollectionChanged -= CollectionChanged_Cues;
                            cache_Cues = null;
                        }
                        if (obj != null)
                        {
                            cache_Cues = obj;
                            ((global::System.ComponentModel.INotifyPropertyChanged)obj).PropertyChanged += PropertyChanged_Cues;
                            ((global::System.Collections.Specialized.INotifyCollectionChanged)obj).CollectionChanged += CollectionChanged_Cues;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainWindow.xaml line 107
                {
                    this.ElapsedTimeText = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 3: // MainWindow.xaml line 112
                {
                    this.ProgressBar = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.ProgressBar).ValueChanged += this.ProgressBar_ValueChanged;
                }
                break;
            case 4: // MainWindow.xaml line 119
                {
                    this.RemainingTimeText = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 5: // MainWindow.xaml line 89
                {
                    this.GoMinusButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.GoMinusButton).Click += this.GoMinus_Click;
                }
                break;
            case 6: // MainWindow.xaml line 92
                {
                    this.PlayPauseButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.PlayPauseButton).Click += this.PlayPause_Click;
                }
                break;
            case 7: // MainWindow.xaml line 95
                {
                    this.GoPlusButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.GoPlusButton).Click += this.GoPlus_Click;
                }
                break;
            case 8: // MainWindow.xaml line 61
                {
                    this.CuesTableView = global::WinRT.CastExtensions.As<global::WinUI.TableView.TableView>(target);
                }
                break;
            case 9: // MainWindow.xaml line 43
                {
                    global::Microsoft.UI.Xaml.Controls.Button element9 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element9).Click += this.Settings_Click;
                }
                break;
            case 10: // MainWindow.xaml line 44
                {
                    global::Microsoft.UI.Xaml.Controls.Button element10 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element10).Click += this.Help_Click;
                }
                break;
            case 11: // MainWindow.xaml line 31
                {
                    this.ImportFilesButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.ImportFilesButton).Click += this.ImportFiles_Click;
                }
                break;
            case 12: // MainWindow.xaml line 34
                {
                    this.AddCueButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.AddCueButton).Click += this.AddCue_Click;
                }
                break;
            case 13: // MainWindow.xaml line 37
                {
                    this.RemoveCueButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.RemoveCueButton).Click += this.RemoveCue_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2411")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1: // MainWindow.xaml line 1
                {                    
                    global::Microsoft.UI.Xaml.Window element1 = (global::Microsoft.UI.Xaml.Window)target;
                    MainWindow_obj1_Bindings bindings = new MainWindow_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Activated += bindings.Activated;
                }
                break;
            }
            return returnValue;
        }
    }
}


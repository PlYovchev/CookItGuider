﻿

#pragma checksum "C:\Users\plt3ch\Documents\GitHub\CookItGuider\CookItUniversal\CookItUniversal\CookItUniversal.WindowsPhone\Pages\RecipeStepPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2BBB42ED42A35ED51205AEAF79F91033"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CookItUniversal.Pages
{
    partial class RecipeStepPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 43 "..\..\..\Pages\RecipeStepPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).ManipulationDelta += this.OnSwipeListener;
                 #line default
                 #line hidden
                #line 44 "..\..\..\Pages\RecipeStepPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).DoubleTapped += this.MediaElement_DoubleTapped;
                 #line default
                 #line hidden
                #line 45 "..\..\..\Pages\RecipeStepPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Holding += this.OnHoldEvent;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}



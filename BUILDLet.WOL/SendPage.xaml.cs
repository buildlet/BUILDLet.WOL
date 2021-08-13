/***************************************************************************************************
The MIT License (MIT)

Copyright 2021 Daiki Sakamoto

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

namespace BUILDLet.WOL
{
    public sealed partial class SendPage : Page
    {
        public SendPage()
        {
            this.InitializeComponent();
        }


        private void MacAddress_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Set ItemSource
                sender.ItemsSource = GetMacAddressHistory(sender.Text);
            }
        }

        private void MacAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            // Set ItemSource
            (sender as AutoSuggestBox).ItemsSource = GetMacAddressHistory((sender as AutoSuggestBox).Text);
        }


        private object GetMacAddressHistory(string text)
        {
            // Get History
            var address_list = ((App)App.Current).ViewModel.MacAddressHistoryList;

            // object to be Returned
            object source = null;

            // Set ItemSource
            if (address_list != null && text.Length < "FF:FF:FF:FF:FF:EF".Length)
            {
                // RETURN: Hintory as ItemSource (Upper Case)
                source = address_list.FindAll(address => address.StartsWith(text.ToUpper(), StringComparison.OrdinalIgnoreCase)); ;
            }

            // RETURN: ItemSource or NULL
            return source;
        }
    }
}

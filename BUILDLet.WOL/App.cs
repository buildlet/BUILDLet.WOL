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
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;  // for ICommand

namespace BUILDLet.WOL
{
    sealed partial class App : Application
    {
        // Set ViewModel to App
        public MagicPacketViewModel<InfoBarSeverity> ViewModel { get; set; }
            = new MagicPacketViewModel<InfoBarSeverity>(
                2304, // Port (Default)
                3,    // Count (Default)
                100,  // Interval (Default)
                3,    // History (Default)
                success => success ? InfoBarSeverity.Success : InfoBarSeverity.Error);

        // SendComamand
        public ICommand SendComamand { get; private set; } = new MagicPacketSendCommand<InfoBarSeverity>();
    }
}

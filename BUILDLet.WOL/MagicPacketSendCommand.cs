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
using System.Windows.Input;
using Windows.Storage;  // for ApplicationData

using BUILDLet.Standard.Utilities.Network;
using System.Linq;

namespace BUILDLet.WOL
{
    public class MagicPacketSendCommand<TSeverity> : ICommand
    {
        private bool can_execute = true;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => this.can_execute;

        public async void Execute(object parameter)
        {
            // Change State (Disabled)
            this.can_execute = false;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());


            // Get ViewModel from parameter
            var view_model = parameter as MagicPacketViewModel<TSeverity>;

            // Change Property (CanSend)
            view_model.CanSend = false;

            // Change Property (CommandExecuted)
            view_model.CommandExecuted = false;

            // for Null Result Check
            string[] result = null;

            try
            {
                // Send Magic Packet (Async)
                result = await MagicPacket.SendAsync(
                    view_model.MacAddress,
                    view_model.Port,
                    view_model.Count,
                    view_model.Interval);

                // Set Message (Success)
                view_model.ResultMessage = $"Magic Packet [{view_model.MacAddress}] has been sent successfully.";

                // Set Severity (Success)
                view_model.ResultSeverity = view_model.GetSeverity(true);

                // Update MAC Address History List
                view_model.UpdateMacAddressHistoryList();

                // Save Current Settings:
                view_model.SaveMacAddressHistoryList();
            }
            catch (Exception e)
            {
                // Set Message (Error)
                view_model.ResultMessage = e.Message;

                // Set Severity (Error)
                view_model.ResultSeverity = view_model.GetSeverity(false);
            }
            finally
            {
                // Change Property (CanSend)
                view_model.CanSend = true;

                // Change Property (CommandExecuted)
                if (result == null || result.Length > 0)
                {
                    // Set true only when Magic Packet has been really sent.
                    view_model.CommandExecuted = true;
                }


                // Restore State (Enabled)
                this.can_execute = true;
                this.CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }
    }
}

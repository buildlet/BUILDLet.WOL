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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

using BUILDLet.Standard.Utilities.Network;

namespace BUILDLet.WOL
{
    public class MagicPacketViewModel<TSeverity>: INotifyPropertyChanged
    {
        private string address;
        private int port;
        private int count;
        private int interval;

        private bool can_send = true;
        private bool executed = false;
        private string message;
        private TSeverity severity;


        public string MacAddress
        {
            get => this.address;
            set
            {
                if (string.CompareOrdinal(this.address, value) != 0)
                {
                    this.address = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public int Port
        {
            get => this.port;
            set
            {
                if (this.port != value)
                {
                    this.port = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public int Count
        {
            get => this.count;
            set
            {
                if (this.count != value)
                {
                    this.count = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public int Interval
        {
            get => this.interval;
            set
            {
                if (this.interval != value)
                {
                    this.interval = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public bool CanSend
        {
            get => this.can_send;
            set
            {
                if (this.can_send != value)
                {
                    this.can_send = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public bool CommandExecuted
        {
            get => this.executed;
            set
            {
                // Always raise the event (w/o Check)

                this.executed = value;
                this.On_PropertyChanged();
            }
        }

        public string ResultMessage
        {
            get => this.message;
            set
            {
                if (string.CompareOrdinal(this.message, value) != 0)
                {
                    this.message = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public TSeverity ResultSeverity
        {
            get => this.severity;
            set
            {
                if (!this.severity.Equals(value))
                {
                    this.severity = value;
                    this.On_PropertyChanged();
                }
            }
        }

        public delegate TSeverity GetSeverityDelegate(bool success);

        public GetSeverityDelegate GetSeverity;


        public event PropertyChangedEventHandler PropertyChanged;

        protected void On_PropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public ICommand SendComamand { get; private set; } = new SendCommandContent();

        private class SendCommandContent : ICommand
        {
            private bool can_execute = true;


            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter) => this.can_execute;

            public async void Execute(object parameter)
            {
                // Convert ViewModel
                var view_model = (MagicPacketViewModel<TSeverity>)parameter;

                // Change State (Disabled)
                this.can_execute = false;
                this.CanExecuteChanged?.Invoke(this, new EventArgs());

                // Change Property (CanSend)
                view_model.CanSend = false;

                // Change Property (CommandExecuted)
                view_model.CommandExecuted = false;

                // for Null Result Check
                string[] result = null;

                try
                {
                    // Send Magic Packet (Async)
                    result = await MagicPacket.SendAsync(view_model.MacAddress, view_model.Port, view_model.Count, view_model.Interval);

                    // Set Message (Success)
                    view_model.ResultMessage = $"Magic Packet [{view_model.MacAddress}] has been sent successfully.";

                    // Set Severity (Success)
                    view_model.ResultSeverity = view_model.GetSeverity(true);
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
                    // Restore State (Enabled)
                    this.can_execute = true;
                    this.CanExecuteChanged?.Invoke(this, new EventArgs());

                    // Change Property (CanSend)
                    view_model.CanSend = true;

                    // Change Property (CommandExecuted)
                    if (result == null || result.Length > 0)
                    {
                        view_model.CommandExecuted = true;
                    }
                }
            }
        }
    }
}

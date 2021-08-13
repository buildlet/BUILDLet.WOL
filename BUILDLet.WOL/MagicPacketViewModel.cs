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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Storage;  // for ApplicationData
using Microsoft.UI.Xaml.Controls;

using BUILDLet.Standard.Utilities.Network;

namespace BUILDLet.WOL
{
    public class MagicPacketViewModel<TSeverity> : INotifyPropertyChanged
    {
        private string address;
        private int port;
        private int count;
        private int interval;

        private bool can_send = true;
        private bool executed = false;
        private string message;
        private TSeverity severity;


        // Constructor
        public MagicPacketViewModel(int defaultPort, int defaultCount, int defaultInterval, int history, GetSeverityDelegate getSeverity)
        {
            // Get Roaming Settings
            this.RoamingSettingsValues = new MagicPacketRoamingSettingsValues(defaultPort, defaultCount, defaultInterval, history);

            // Set Port, Count, Interval and MacAddressHistoryList from RoamingSettingsValues
            this.Port = this.RoamingSettingsValues.Port;
            this.Count = this.RoamingSettingsValues.Count;
            this.Interval = this.RoamingSettingsValues.Interval;
            this.MacAddressHistoryList = this.RoamingSettingsValues.GetMacAddressHistoryList() as List<string>;

            // Set GetSeverityDelegate
            this.GetSeverity = getSeverity;
        }


        // for Roaming Settings
        public MagicPacketRoamingSettingsValues RoamingSettingsValues { get; }

        // for List of MAC Address History
        public List<string> MacAddressHistoryList { get; private set; }


        // Mac Address Property
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

        // Port Property
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

        // Count Property
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

        // Interval Property
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

        // CanSend Property
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

        // CommandExecuted Property
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

        // ResultMessage Property
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

        // ResultSeverity Property
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


        // for GetSeverity Delegate
        public delegate TSeverity GetSeverityDelegate(bool success);
        public GetSeverityDelegate GetSeverity;


        // for PropertyChanged Event
        public event PropertyChangedEventHandler PropertyChanged;
        protected void On_PropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        // Update MacAddressHistoryList
        public void UpdateMacAddressHistoryList()
        {
            // Check Existence (Upper Case)
            if (!this.MacAddressHistoryList.Contains(this.MacAddress.ToUpper()))
            {
                // Check limit
                if (this.MacAddressHistoryList.Count >= this.RoamingSettingsValues.MaxNumberOfMacAddressHistory)
                {
                    // Remove last item
                    this.MacAddressHistoryList.RemoveAt(this.MacAddressHistoryList.Count - 1);
                }

                // Insert to 1st (Upper Case)
                this.MacAddressHistoryList.Insert(0, this.MacAddress.ToUpper());
            }
        }


        // Save Reaming Settings
        public void SaveMacAddressHistoryList()
        {
            // Save Port, Count, Interval and MacAddressHitsoryList
            this.RoamingSettingsValues.Port = this.Port;
            this.RoamingSettingsValues.Count = this.Count;
            this.RoamingSettingsValues.Interval = this.Interval;
            this.RoamingSettingsValues.MacAddressHistoryList = this.MacAddressHistoryList;

            // Save ApplicationDataCompositeValue
            ApplicationData.Current.RoamingSettings.Values["BUILDLet.WOL"] = this.RoamingSettingsValues.CompositeValue;
        }


        // Clear Method
        public void Clear()
        {
            // Clear RoamingSettingsValues
            this.RoamingSettingsValues.Clear();

            // Reset Port, Count, Interval and MacAddressHistoryList from RoamingSettingsValues
            this.Port = this.RoamingSettingsValues.Port;
            this.Count = this.RoamingSettingsValues.Count;
            this.Interval = this.RoamingSettingsValues.Interval;
            this.MacAddressHistoryList = this.RoamingSettingsValues.GetMacAddressHistoryList() as List<string>;
        }
    }
}

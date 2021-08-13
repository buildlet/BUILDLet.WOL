﻿/***************************************************************************************************
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
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;  // for ApplicationData

namespace BUILDLet.WOL
{
    public class MagicPacketRoamingSettingsValues
    {
        // for History of MAC Address List
        private List<string> address_list;

        // for Default Value
        private readonly int default_port;
        private readonly int default_count;
        private readonly int default_interval;


        // for Roaming Settings
        public ApplicationDataCompositeValue CompositeValue { get; }

        // Maxium Number of MAC Address List
        public int MaxNumberOfMacAddressHistory { get; }

        // Port
        public int Port
        {
            get => (int)this.CompositeValue["Port"];
            set { this.CompositeValue["Port"] = value; }
        }

        // Count
        public int Count
        {
            get => (int)this.CompositeValue["Count"];
            set { this.CompositeValue["Count"] = value; }
        }

        // Interval
        public int Interval
        {
            get => (int)this.CompositeValue["Interval"];
            set { this.CompositeValue["Interval"] = value; }
        }

        // MAC Address (Setter Only)
        public List<string> MacAddressHistoryList
        {
            set { this.CompositeValue["MacAddressHistoryList"] = (this.address_list = value).ToArray(); }
        }

        // MAC Address (Getter)
        public IReadOnlyList<string> GetMacAddressHistoryList() => this.address_list;


        // Constructor
        public MagicPacketRoamingSettingsValues(int defaultPort, int defaultCount, int defaultInterval, int history)
        {
            // Store Default Value
            this.default_port = defaultPort;
            this.default_count = defaultCount;
            this.default_interval = defaultInterval;

            // Get OR New Roaming Settings
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("BUILDLet.WOL"))
            {
                // Get Roaming Settings
                this.CompositeValue = ApplicationData.Current.RoamingSettings.Values["BUILDLet.WOL"] as ApplicationDataCompositeValue;

                // Set Port, Count and Interval
                if (this.Port < 0 || this.Port > 65535) { this.Port = defaultPort; }
                if (this.Count < 1 || this.Count > 100) { this.Count = defaultCount; }
                if (this.Interval < 0 || this.Interval > 999) { this.Interval = defaultInterval; }

                // Set Address List
                this.address_list = (this.CompositeValue["MacAddressHistoryList"] as string[])?.ToList() ?? new List<string>();
            }
            else
            {
                // New Roaming Settings
                this.CompositeValue = new ApplicationDataCompositeValue();

                // Set Port, Count and Interval
                this.Port = this.default_port;
                this.Count = this.default_count;
                this.Interval = this.default_interval;

                // New Address List
                this.address_list = new List<string>();
            }

            // Set History
            if (history >= 0 && history <= 10) { this.MaxNumberOfMacAddressHistory = history; }
        }


        // Clear Method
        public void Clear()
        {
            this.Port = this.default_port;
            this.Count = this.default_count;
            this.Interval = this.default_interval;
            this.address_list.Clear();
        }
    }
}

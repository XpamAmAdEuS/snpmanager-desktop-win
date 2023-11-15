using System;

namespace SnpApp.Common;

    public class TokenChangedEventArgs : EventArgs
    {
        public string NewValue { get; }

        public TokenChangedEventArgs(string newValue)
        {
            NewValue = newValue;
        }
    }

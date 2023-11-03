using System;

namespace Contoso.App.Common;


    public class TokenChangedEventArgs : EventArgs
    {
        public string NewValue { get; }

        public TokenChangedEventArgs(string newValue)
        {
            NewValue = newValue;
        }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elma.SitecoreUtil.Events {
    /// <summary>Event agruments for pager control</summary>
    public class PagerEventArgs : EventArgs {
        /// <summary>Gets or sets the page number that is fired</summary>
        public int Number {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the first button has been click</summary>
        public bool First {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the previous button has been click</summary>
        public bool Previous {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the next button has been click</summary>
        public bool Next {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the last button has been click</summary>
        public bool Last {
            get;
            set;
        }

        /// <summary>Gets or sets the number of rows each page contains</summary>
        public int PageSize {
            get;
            set;
        }

    }
}

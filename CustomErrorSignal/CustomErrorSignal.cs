#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

#region License, Terms and Author(s) on Updates
//
// A slightly modified version of Atif Aziz's ErrorLogModule for ELMAH
// Copyright (c) 2013 Dragos Hont
//
//  Author(s):
//
//      dhont   https://github.com/dhont
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections;
using System.Web;
using Elmah;

namespace ElmahExtensions
{
    #region Imports

    

    #endregion

    // ref: http://stackoverflow.com/questions/841451/using-elmah-in-a-console-application/1068378#1068378
    // ref: http://robrich.org/archive/2012/05/02/elmah-return-the-errorid-from-signal-raise.aspx

    public sealed class CustomErrorSignal
    {
        public event CustomErrorSignalEventHandler Raised;

        private static Hashtable _signalByApp;
        private static readonly object Lock = new object();


        public ErrorLogEntry Raise(Exception e)
        {
            return Raise(e, null);
        }
        public static ErrorLogEntry Handle(Exception ex, string applicationName = "ConsoleApplication")
        {
            ErrorLogEntry x;

            if (HttpContext.Current != null)
                x = FromCurrentContext().Raise(ex, null);
            else
            {
                ErrorLog errorLog = ErrorLog.GetDefault(null);
                errorLog.ApplicationName = applicationName;
                var err = errorLog.Log(new Error(ex));
                x = errorLog.GetError(err);
            }
            return x;
        }

        //void OurRaise(this ErrorSignal signal, Exception ex)
        //{
        //    if (ex is DbEntityValidationException)
        //    {
        //        var ve = ex as DbEntityValidationException;
        //        foreach (var error in ve.EntityValidationErrors)
        //        {
        //            var v = error.ValidationErrors.FirstOrDefault();
        //            if (v != null)
        //                signal.Raise(new DbEntityValidationException(
        //                                 string.Format("Validation Error :: {0} - {1}",
        //                                               v.PropertyName, v.ErrorMessage)));
        //        };
        //        return;
        //    }
        //    signal.Raise(ex);
        //}

        public ErrorLogEntry Raise(Exception e, HttpContext context)
        {
            if (context == null)
                context = HttpContext.Current;

            CustomErrorSignalEventHandler handler = Raised;

            if (handler != null)
            {
                var x = new CustomErrorSignalEventArgs(e, context);
                handler(this, x);

                return x.ErrorLogEntry;
            }

            return null;
        }

        public static CustomErrorSignal FromCurrentContext()
        {
            return FromContext(HttpContext.Current);
        }

        public static CustomErrorSignal FromContext(HttpContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            return Get(context.ApplicationInstance);
        }

        public static CustomErrorSignal Get(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            lock (Lock)
            {
                //
                // Allocate map of object per application on demand.
                //

                if (_signalByApp == null)
                    _signalByApp = new Hashtable();

                //
                // Get the list of modules fot the application. If this is
                // the first registration for the supplied application object
                // then setup a new and empty list.
                //

                var signal = (CustomErrorSignal) _signalByApp[application];

                if (signal == null)
                {
                    signal = new CustomErrorSignal();
                    _signalByApp.Add(application, signal);
                    application.Disposed += OnApplicationDisposed;
                }

                return signal;
            }
        }

        private static void OnApplicationDisposed(object sender, EventArgs e)
        {
            var application = (HttpApplication) sender;

            lock (Lock)
            {
                if (_signalByApp == null)
                    return;

                _signalByApp.Remove(application);
                
                if (_signalByApp.Count == 0)
                    _signalByApp = null;
            }
        }
    }

    public delegate void CustomErrorSignalEventHandler(object sender, CustomErrorSignalEventArgs args);

    [ Serializable ]
    public sealed class CustomErrorSignalEventArgs : EventArgs
    {
        [ NonSerialized ]
        private readonly HttpContext _context;

        public CustomErrorSignalEventArgs(Exception e, HttpContext context)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            Exception = e;
            _context = context;
        }

        public ErrorLogEntry ErrorLogEntry { get; set; }

        public Exception Exception { get; private set; }

        public HttpContext Context
        {
            get { return _context; }
        }

        public override string ToString()
        {
            return Exception.Message.Length == 0 ? Exception.GetType().FullName : Exception.Message;
        }
    }
}
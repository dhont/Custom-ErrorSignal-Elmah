Custom-ErrorSignal-Elmah
========================

About ELMAH:
----------------------
ELMAH (Error Logging Modules and Handlers) is an application-wide error logging facility that is completely pluggable. It can be dynamically added to a running ASP.NET web application, or even all ASP.NET web applications on a machine, without any need for re-compilation or re-deployment.

About ErrorSignal
----------------------
Is a flexible API introduce in ELMAH to manually log errors, along with the entire application context (even FORM Data  and server/client variables)

About CustomErrorSignal (available on NuGet as well)
-----------------------------
This is an alternative to ErrorSignal component. It provides a wrapper for manually logging errors even in console applications or automated tests and returns the ErrorId of the raised exception.

Usage:
-------------------
If the library is added using NuGet in a ASP.NET project, then all you have to do is call:


        var errorInfo = CustomErrorSignal.Handle(ex);
  
        var errorInfo = CustomErrorSignal.GetCurrentContext().Raise(ex); // when the application is web only
  


If NuGet is not used, compile the library (.NET 2.0) and add the library to your project. It depends only on elmah.core to run. When used in a web application, In web.config modifiy the module AND httpmodule ErrorLog to use ElmahExtensions.CustomErrorLogModule, ElmahExtensions.

References
-----------------

    https://code.google.com/p/elmah/source/checkout

    http://stackoverflow.com/questions/841451/using-elmah-in-a-console-application/1068378#1068378
    
    http://robrich.org/archive/2012/05/02/elmah-return-the-errorid-from-signal-raise.aspx

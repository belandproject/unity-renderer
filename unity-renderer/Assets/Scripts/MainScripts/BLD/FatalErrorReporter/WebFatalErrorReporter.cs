using System;
using BLD.Interface;

namespace BLD.FatalErrorReporter
{
    public class WebFatalErrorReporter : IFatalErrorReporter
    {
        public void Report(Exception exception)
        {
            WebInterface.ReportAvatarFatalError();
        }
    }
}
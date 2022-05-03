using System;

namespace BLD.FatalErrorReporter
{
    public interface IFatalErrorReporter
    {
        void Report(Exception exception);
    }
}
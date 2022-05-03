using System;

namespace BLD.Builder
{
    internal interface ISectionOpenSectionRequester
    {
        event Action<SectionId> OnRequestOpenSection;
    }
}
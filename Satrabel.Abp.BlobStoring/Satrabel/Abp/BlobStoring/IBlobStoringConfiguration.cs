using Abp.Collections;

namespace Satrabel.Abp.BlobStoring
{
    public interface IBlobStoringConfiguration
    {
        ITypeList<IBlobProvider> Providers { get; }
    }
}
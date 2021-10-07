using Abp.Collections;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public interface IBlobStoringConfiguration
    {
        ITypeList<IBlobProvider> Providers { get; }
    }
}
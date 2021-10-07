using System;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public interface xICurrentTenant
    {
        int? Id { get; }

        IDisposable Change(int? v);
    }
}
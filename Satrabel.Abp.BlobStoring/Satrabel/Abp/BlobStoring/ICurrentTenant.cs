using System;

namespace Satrabel.Abp.BlobStoring
{
    public interface xICurrentTenant
    {
        int? Id { get; }

        IDisposable Change(int? v);
    }
}
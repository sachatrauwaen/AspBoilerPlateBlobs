using Abp.Collections;
using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    internal class BlobStoringConfiguration : IBlobStoringConfiguration
    {

        public ITypeList<IBlobProvider> Providers { get; private set; }

        public BlobStoringConfiguration()
        {
            Providers = new TypeList<IBlobProvider>();
        }
    }
}

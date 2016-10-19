using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Packaging
{
    public interface IPackageable <T>
    {
        void Package(T type, string filename);

        T Unpackage(string filename);
    }
}

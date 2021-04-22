using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kara.Services
{
    public interface IDevice
    {
        string GetIdentifier();

        bool PhonePermissionGranted();
    }
}

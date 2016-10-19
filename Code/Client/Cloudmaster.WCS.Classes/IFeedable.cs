using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Classes
{
	/// <summary>
	/// Describes a type that is comes in and off a feed?
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public interface IFeedable<T>
    {
		T GetIdentifier();

        bool IsUserModified { set; get; }

        bool IsUpdatingServer { set; get; }

        bool HasErrorOccuredOnServerUpdate { set; get; }
    }
}

using System.Collections.Generic;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.IO
{
	/// <summary>
	/// Describes the ability of a type to get and set forms
	/// </summary>
    public interface IFormManager
    {
        IList<FormDefinition> GetFormDefinitions();

        IList<FormMetadata> GetFormInstancesMetadata();

        FormMetadata StoreFormInstance(FormMetadata formInstance, string filename);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Registry;
public class InvalidRegistryItemException : Exception
{
    public RegistryItem RegistryItem { get; private set; }
    public InvalidRegistryItemException(RegistryItem registryItem)
    {
        RegistryItem = registryItem;
    }

    public InvalidRegistryItemException(RegistryItem registryItem, string message) : base(message)
    {
        RegistryItem = registryItem;
    }
    public InvalidRegistryItemException(RegistryItem registryItem, string message, Exception innerException) : base(message, innerException)
    {
        RegistryItem = registryItem;
    }
}

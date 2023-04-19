using System;

namespace Conna.Inventory;

public class ItemClassAttribute : Attribute
{
	public Type Type { get; private set; }

	public ItemClassAttribute( Type type )
	{
		Type = type;
	}
}

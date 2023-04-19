using System;
using System.Collections.Generic;
using Sandbox;

namespace Conna.Inventory;

public class ResourceItem<A,T> : InventoryItem, IResourceItem where A : ItemResource where T : ResourceItem<A,T>
{
	public override string Name => Resource?.ItemName ?? string.Empty;
	public override string Description => Resource?.Description ?? string.Empty;
	public override string WorldModel => Resource?.WorldModel ?? string.Empty;
	public override string UniqueId => Resource?.UniqueId ?? string.Empty;
	public override string Icon => Resource?.Icon ?? string.Empty;

	public A Resource { get; set; }

	ItemResource IResourceItem.Resource => Resource;

	public void LoadResource( ItemResource resource )
	{
		if ( resource is not A )
		{
			Log.Error( $"Unable to load resource for { GetType().Name }. Resource is not of type: {typeof( A ).Name}!" );
			return;
		}

		InternalTags = new HashSet<string>( resource.Tags, StringComparer.OrdinalIgnoreCase );
		Resource = resource as A;

		BuildTags( InternalTags );
	}
}

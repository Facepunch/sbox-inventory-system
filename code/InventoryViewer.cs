using Sandbox;
using System.Collections.Generic;

namespace Conna.Inventory;

public partial class InventoryViewer : EntityComponent, IValid
{
	public bool IsValid => Entity.IsValid();

	[Net] public IList<ulong> ContainerIds { get; private set; } = new List<ulong>();

	/// <summary>
	/// The container that this viewer is currently viewing.
	/// </summary>
	public IEnumerable<InventoryContainer> Containers
	{
		get
		{
			foreach ( var id in ContainerIds )
			{
				yield return InventorySystem.Find( id );
			}
		}
	}

	/// <summary>
	/// Set the container this viewer is currently viewing.
	/// </summary>
	public void AddContainer( InventoryContainer container )
	{
		if ( !ContainerIds.Contains( container.ContainerId ) )
		{
			ContainerIds.Add( container.ContainerId );
		}
	}

	/// <summary>
	/// Clear the container this viewer is currently viewing.
	/// </summary>
	public void ClearContainers()
	{
		ContainerIds.Clear();
	}
}

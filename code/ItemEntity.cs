using Sandbox;

namespace Conna.Inventory;

public partial class ItemEntity : ModelEntity
{
	public TimeUntil TimeUntilCanPickup { get; set; }
	public TimeSince TimeSinceSpawned { get; set; }

	[Net] private NetInventoryItem InternalItem { get; set; }
	public InventoryItem Item => InternalItem.Value;

	public void SetItem( InventoryItem item )
	{
		var worldModel = !string.IsNullOrEmpty( item.WorldModel ) ? item.WorldModel : "models/sbox_props/burger_box/burger_box.vmdl";

		if ( !string.IsNullOrEmpty( worldModel ) )
		{
			SetModel( worldModel );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}

		InternalItem = new NetInventoryItem( item );
		item.SetWorldEntity( this );
	}

	public InventoryItem Take()
	{
		if ( IsValid && Item.IsValid() )
		{
			var item = Item;

			item.ClearWorldEntity();
			InternalItem = null;
			Delete();

			return item;
		}

		return null;
	}

	public virtual void Reset()
	{
		Delete();
	}

	public override void Spawn()
	{
		TimeUntilCanPickup = 1f;
		TimeSinceSpawned = 0f;
		Transmit = TransmitType.Always;

		Tags.Add( "hover", "solid", "passplayers" );

		base.Spawn();
	}
}


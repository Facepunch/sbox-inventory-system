using Sandbox;
using System.IO;
using System.Collections.Generic;
using System;
using Sandbox.UI;

namespace Conna.Inventory;

public class InventoryItem : IValid
{
	public InventoryContainer Parent { get; set; }
	public ItemEntity WorldEntity { get; private set; }
	public bool IsWorldEntity { get; private set; }

	public virtual ushort DefaultStackSize => 1;
	public virtual ushort MaxStackSize => 1;
	public virtual string WorldModel => "models/sbox_props/burger_box/burger_box.vmdl";
	public virtual string Description => string.Empty;
	public virtual bool DropOnDeath => false;
	public virtual Color Color => Color.White;
	public virtual string Name => string.Empty;
	public virtual Color IconTintColor => Color.White;
	public virtual string PrimaryUseHint => string.Empty;
	public virtual string SecondaryUseHint => string.Empty;
	public virtual string UniqueId => string.Empty;
	public virtual string Icon => string.Empty;

	public virtual IReadOnlySet<string> Tags => InternalTags;
	public virtual Dictionary<string, int> RequiredItems => null;
	public virtual bool IsCraftable => false;

	protected HashSet<string> InternalTags = new( StringComparer.OrdinalIgnoreCase );

	public InventoryItem()
	{
		BuildTags( InternalTags );
	}

	public static InventoryItem Deserialize( byte[] data )
	{
		using ( var stream = new MemoryStream( data ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				return reader.ReadInventoryItem();
			}
		}
	}

	public byte[] Serialize()
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( this );
				return stream.ToArray();
			}
		}
	}

	private ushort InternalStackSize;
	private bool InternalIsDirty;

	public ushort StackSize
	{
		get => InternalStackSize;

		set
		{
			if ( InternalStackSize != value )
			{
				InternalStackSize = value;
				IsDirty = true;

				if ( InternalStackSize <= 0 )
					Remove();
			}
		}
	}

	public bool IsInstance => ItemId > 0;

	public bool IsDirty
	{
		get => InternalIsDirty;

		set
		{
			if ( Game.IsServer )
			{
				if ( Parent == null )
				{
					InternalIsDirty = false;
					return;
				}

				InternalIsDirty = value;

				if ( InternalIsDirty )
				{
					Parent.IsDirty = true;
				}
			}
		}
	}

	public bool IsValid { get; set; }
	public ulong ItemId { get; private set; }
	public ushort SlotId { get; set; }

	public void SetWorldEntity( ItemEntity entity )
	{
		WorldEntity = entity;
		IsWorldEntity = entity.IsValid();
		IsDirty = true;
		Remove();
	}

	public void SetItemId( ulong itemId )
	{
		ItemId = itemId;
	}

	public void ClearWorldEntity()
	{
		WorldEntity = null;
		IsWorldEntity = false;
		IsDirty = true;
	}

	public void Remove()
	{
		if ( Parent.IsValid() )
		{
			Parent.Remove( this );
		}
	}

	public void Replace( InventoryItem other )
	{
		if ( Parent.IsValid() )
		{
			Parent.Replace( SlotId, other );
		}
	}

	public virtual bool OnTrySwap( InventoryItem other )
	{
		return true;
	}

	public virtual bool IsSameType( InventoryItem other )
	{
		return (GetType() == other.GetType() && UniqueId == other.UniqueId);
	}

	public virtual bool CanStackWith( InventoryItem other )
	{
		return true;
	}

	public virtual void AddTooltipInfo( Panel container )
	{

	}

	public virtual void Write( BinaryWriter writer )
	{
		if ( WorldEntity.IsValid() )
		{
			writer.Write( true );
			writer.Write( WorldEntity.NetworkIdent );
		}
		else
		{
			writer.Write( false );
		}

	}

	public virtual void Read( BinaryReader reader )
	{
		IsWorldEntity = reader.ReadBoolean();

		if ( IsWorldEntity )
		{
			WorldEntity = (Entity.FindByIndex( reader.ReadInt32() ) as ItemEntity);
			return;
		}

		if ( WorldEntity.IsValid() )
		{
			if ( Game.IsServer )
			{
				WorldEntity.Delete();
			}

			WorldEntity = null;
		}
	}

	public virtual void OnRemoved()
	{

	}

	public virtual void OnCreated()
	{

	}

	protected virtual void BuildTags( HashSet<string> tags )
	{

	}

	public override int GetHashCode()
	{
		return HashCode.Combine( IsValid, UniqueId, ItemId, StackSize );
	}
}

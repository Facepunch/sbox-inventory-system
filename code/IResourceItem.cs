namespace Conna.Inventory;

public interface IResourceItem
{
	public ItemResource Resource { get; }
	public void LoadResource( ItemResource resource );
}

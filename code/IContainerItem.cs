namespace Conna.Inventory;

public interface IContainerItem
{
	public InventoryContainer Container { get; }
	public string ContainerName { get; }
}

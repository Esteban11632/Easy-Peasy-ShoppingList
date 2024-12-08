using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Services
{
    public interface IShoppingListService
    {
        Task<List<ShoppingItem>> GetShoppingItemsAsync(string familyGroup);
        Task<ShoppingItem> AddShoppingItemAsync(ShoppingItem item);
        Task UpdateShoppingItemAsync(ShoppingItem item);
        Task DeleteShoppingItemAsync(int itemId);
    }
}

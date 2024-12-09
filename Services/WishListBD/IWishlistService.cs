using Easy_Peasy_ShoppingList.Models;

namespace Easy_Peasy_ShoppingList.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetWishlistItemsAsync(string familyGroup);
        Task<WishlistItem> AddWishlistItemAsync(WishlistItem item);
        Task RemoveWishlistItemAsync(int itemId);
    }
}

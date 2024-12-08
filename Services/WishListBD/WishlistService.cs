using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;

namespace Easy_Peasy_ShoppingList.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ShoppingListDbContext _context;

        public WishlistService(ShoppingListDbContext context)
        {
            _context = context;
        }

        public async Task<List<WishlistItem>> GetWishlistItemsAsync(string familyGroup)
        {
            return await _context.WishlistItems
                .Where(item => item.FamilyGroup == familyGroup)
                .ToListAsync();
        }

        public async Task<WishlistItem> AddWishlistItemAsync(WishlistItem item)
        {
            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task RemoveWishlistItemAsync(int itemId)
        {
            var item = await _context.WishlistItems.FindAsync(itemId);
            if (item != null)
            {
                _context.WishlistItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
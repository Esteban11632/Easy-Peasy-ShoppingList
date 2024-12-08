using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;

namespace Easy_Peasy_ShoppingList.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly ShoppingListDbContext _context;

        public ShoppingListService(ShoppingListDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShoppingItem>> GetShoppingItemsAsync(string familyGroup)
        {
            return await _context.ShoppingItems
                .Where(item => item.FamilyGroup == familyGroup)
                .ToListAsync();
        }

        public async Task<ShoppingItem> AddShoppingItemAsync(ShoppingItem item)
        {
            _context.ShoppingItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateShoppingItemAsync(ShoppingItem item)
        {
            _context.ShoppingItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShoppingItemAsync(int itemId)
        {
            var item = await _context.ShoppingItems.FindAsync(itemId);
            if (item != null)
            {
                _context.ShoppingItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
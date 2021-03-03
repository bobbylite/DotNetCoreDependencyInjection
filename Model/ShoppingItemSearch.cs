using System;

namespace DependencyInjectionApp.Model
{
    public class ShoppingItemSearch
    {
        string _s;

        public ShoppingItemSearch(string s)
        {
            _s = s;
        }

        public bool StartsWith(ShoppingItem e)
        {
            return e.ItemName.Contains(_s, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
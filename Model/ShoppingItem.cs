
using System;
using System.Diagnostics.CodeAnalysis;

namespace DependencyInjectionApp.Model
{
    public class ShoppingItem : IComparable<ShoppingItem>, IEquatable<ShoppingItem>
    {
        public string ItemName {get; set;}
        public int ItemId {get; set; }
        
        public int CompareTo([AllowNull] ShoppingItem other)
        {
            return other.ItemName.CompareTo(this.ItemName);
        }

        public bool Equals([AllowNull] ShoppingItem other)
        {
            return other.ItemName == this.ItemName;
        }
    }
}
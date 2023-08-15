using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class StockInt
    {
        public Vector2Int _stock;
        
        public int Count
        {
            get => _stock.x;
            set => _stock.x = value.Clamp(0, _stock.y);
        } 
        
        public int Limit
        {
            get => _stock.y;
            set => _stock.y = value;
        }
        
        // =======================================================================
        public static implicit operator Vector2Int(StockInt stock)
        {
            return stock._stock;
        }
        public static implicit operator float(StockInt stock)
        {
            return stock._stock.x;
        }
    }
}
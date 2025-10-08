﻿namespace Catalog.Specifications
{
    public class CatalogSpecParams
    {
        private const int MaxPageSize = 50;

        private int _pageSize = 10;
        public int PageIndex { get; set; } = 1;
        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? BrandId { get; set; }
        public string? TypeId { get; set; }
        public string? Search { get; set; }
    }
}

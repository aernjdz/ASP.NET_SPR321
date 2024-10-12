﻿namespace WebHulk.Areas.Admin.Models.Products
{
    public class ProductItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public List<string>? Images { get; set; }
    }
}
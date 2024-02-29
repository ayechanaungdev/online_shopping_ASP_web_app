using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfraStructure.Data.Seeding
{
    public static class SeedData
    {        
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cosmetics", IsDelete = false },
                new Category { Id = 2, Name = "Perfume", IsDelete = false },
                new Category { Id = 3, Name = "Women Dress", IsDelete = false },
                new Category { Id = 4, Name = "Sport Wear", IsDelete = false },
                new Category { Id = 5, Name = "Men Causal Wear", IsDelete = false }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductName = "Chanel Lipstick Color 23", Price = 200, ImgPath = "image3_370x403.jpg", CategoryId = 1, Information = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.", IsDelete = false },
                new Product { Id = 2, ProductName = "Chanel Co Co Perfume", Price = 300, ImgPath = "perfume1_370x403.jpg", CategoryId = 2, Information = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.", IsDelete = false },
                new Product { Id = 3, ProductName = "Women White Dress", Price = 400, ImgPath = "cat2.jpg", CategoryId = 3, Information = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.", IsDelete = false },
                new Product { Id = 4, ProductName = "Men Sport Shirts", Price = 500, ImgPath = "category_2.png", CategoryId = 4, Information = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.", IsDelete = false },
                new Product { Id = 5, ProductName = "Men Long Army Shirt", Price = 600, ImgPath = "product4.png", CategoryId = 5, Information = "Lorem ipsum is placeholder text commonly used in the graphic, print, and publishing industries for previewing layouts and visual mockups.", IsDelete = false }
                );
        }

    }
}

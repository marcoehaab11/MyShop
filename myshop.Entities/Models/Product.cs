using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Image")]
        [ValidateNever]
        public string Img { get; set; }
        public decimal Price { get; set; } 
        [ForeignKey("Category")]
        [DisplayName("Category")]

        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
    }
}

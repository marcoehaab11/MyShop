using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Models
{
    public class ShoppingCard
    {   public int Id { get; set; }
        public int ProductId {  get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]

        public Product product { get; set; }
        [Range(1, 100, ErrorMessage = "Maximum 100 Product")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]

        public ApplicationUser ApplicationUser { get; set; }
    
    }
}

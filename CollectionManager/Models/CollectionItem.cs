using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionManager.Models
{
    public class CollectionItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Other { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsSold { get; set; }
        public string ImagePath { get; set; }

        public override string ToString() =>
            $"{Name}|{Description}|{Other}|{Price}|{Status}|{Rating}|{Comment}|{IsSold}|{ImagePath}";

        public static CollectionItem FromString(string line)
        {
            var parts = line.Split('|');
            return new CollectionItem
            {
                Name = parts[0],
                Description = parts.Length > 1 ? parts[1] : "",
                Other = parts.Length > 2 ? parts[2] : "",
                Price = parts.Length > 3 ? decimal.Parse(parts[3]) : 0,
                Status = parts.Length > 4 ? parts[4] : "",
                Rating = parts.Length > 5 ? int.Parse(parts[5]) : 0,
                Comment = parts.Length > 6 ? parts[6] : "",
                IsSold = parts.Length > 7 ? bool.Parse(parts[7]) : false,
                ImagePath = parts.Length > 8 ? parts[8] : ""  
            };
        }
    }

}

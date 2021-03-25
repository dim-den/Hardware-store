using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPLab6
{
    public class Device
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Producer { get; set; }
        public string Country { get; set; }
        public int Quantity { get; set; }
        public int Purhased { get; set; }
        public int Price { get; set; }

        public Device() { }
        public Device(int id, string name, string imagePath, string description, string producer, string country, int quantity, int purchased, int price)
        {
            ID = id;
            Name = name;
            ImagePath = imagePath;
            Description = description;
            Producer = producer;
            Country = country;
            Quantity = quantity;
            Purhased = purchased;
            Price = price;
        }

        public bool ProducerContains(string producer)
        {
            if (producer == null || producer == "")
                return true;
            return Producer.ToLower().Contains(producer.ToLower());
        }
        public bool CountryContains(string country)
        {
            if (country == null || country == "")
                return true;
            return Country.ToLower().Contains(country.ToLower());

        }
        public bool NameContains(string name)
        {
            if (name == null || name == "")
                return true;
            return Name.ToLower().Contains(name.ToLower());

        }
        public bool HaveEmptyFields()
        {
            return  Name == null        || Name == ""        || 
                    ImagePath == null   || ImagePath == ""   ||
                    Description == null || Description == "" || 
                    Producer == null    || Producer == ""    ||
                    Country == null     || Country == ""     || 
                    Price <= 0;
        }
        public override bool Equals(object obj)
        {
            return obj is Device device &&
                   ID == device.ID &&
                   Name == device.Name &&
                   ImagePath == device.ImagePath &&
                   Description == device.Description &&
                   Producer == device.Producer &&
                   Country == device.Country &&
                   Quantity == device.Quantity &&
                   Purhased == device.Purhased &&
                   Price == device.Price;
        }

        public override int GetHashCode()
        {
            var hashCode = -1470363073;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ImagePath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Producer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Country);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + Purhased.GetHashCode();
            hashCode = hashCode * -1521134295 + Price.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"ID:{ID}\nName:{Name}\nImagePath:{ImagePath}\nDescription:{Description}\n" +
                $"Producer:{Producer}\nCountry:{Country}\nQuantity:{Quantity}\nPurchased:{Purhased}\nPrice:{Price}";
        }
    }
}

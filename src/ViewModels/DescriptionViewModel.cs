using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DescriptionViewModel
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public DescriptionViewModel(string description, string name, string id)
        {
            Description = description;
            Name = name;
            Id = id;
        }

        public DescriptionViewModel() 
        {
        }
    }
}

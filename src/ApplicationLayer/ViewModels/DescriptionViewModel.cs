﻿namespace ApplicationLayer.ViewModels;

public class DescriptionViewModel
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public string[] GeneratedDescription { get; set; }

    public DescriptionViewModel(string description, string name, string id, string[] generateDescription)
    {
        Description = description;
        Name = name;
        Id = id;
        GeneratedDescription = generateDescription;
    }

    public DescriptionViewModel()
    {
    }
}

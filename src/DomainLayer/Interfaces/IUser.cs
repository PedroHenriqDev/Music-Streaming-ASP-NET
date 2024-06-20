namespace DomainLayer.Interfaces;

public interface IUser<T> : IEntityWithDescription<T>, IEntityWithEmail<T>
{
    string Id { get; set; }
    string Name { get; set; }
    string Password { get; set; }
    string Email { get; set; }
    string? PhoneNumber { get; set; }
    string? Description { get; set; }
    byte[] PictureProfile { get; set; }
    DateTime BirthDate { get; set; }
    DateTime DateCreation { get; set; }
}

namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string HashPassword { get; private set; }

    public User(string name, string email, string hashPassword)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        HashPassword = hashPassword;
    }

    public void Update(string name, string email, string hashPassword)
    {
        Name = name;
        Email = email;
    }
}
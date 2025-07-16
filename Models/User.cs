using System;
using System.Collections.Generic;

namespace LibraryAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;
}

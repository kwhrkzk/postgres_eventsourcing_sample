using System;

namespace Domain;
public record UserName
{
    public String NameString { get; }

    public UserName(String name) => this.NameString = name.Trim();
}

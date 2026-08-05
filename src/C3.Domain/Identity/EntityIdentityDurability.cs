namespace C3.Domain.Identity
{
    /// <summary>
    /// Declares whether an entity identity can be expected to survive a profile
    /// save and reopen. Imported aliases are compatibility metadata, not a third
    /// identity durability class.
    /// </summary>
    public enum EntityIdentityDurability
    {
        SessionScoped = 0,
        Durable = 1
    }
}

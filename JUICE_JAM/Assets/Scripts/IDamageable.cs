namespace JuiceJam
{
    public interface IDamageable
    {
        bool CanBeDamaged { get; }

        void TakeDamage(DamageData damageData);
    }
}
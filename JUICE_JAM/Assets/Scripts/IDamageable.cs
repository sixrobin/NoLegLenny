namespace JuiceJam
{
    public interface IDamageable
    {
        bool CanBeDamaged { get; }
        bool DontDestroyDamageSource { get; }

        void TakeDamage(DamageData damageData);
    }
}
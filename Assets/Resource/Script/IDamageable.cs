public interface IDamageable
{
	void TakeDamage(float damage, EDamageType damageType);
}

public enum EDamageType
{
	Normal,
	Push,
}
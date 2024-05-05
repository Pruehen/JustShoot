using UnityEngine;


public interface IDamagable
{
    void TakeDamage(float damage);
}

//Todo 이름규칙 적용
[System.Serializable]
public class Combat
{
    public Transform _owner;
    public float initalMaxHp;
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;
    [SerializeField] private float _invincibleTime = .1f;
    [SerializeField] private float _prevHitTime = 0f;
    private bool _defalutEffectOnDamaged;

    public System.Action OnDamaged { get; set; }
    public System.Action<float> OnDamagedWDamage { get; set; }
    public System.Action<Combat> OnDealWTarget { get; set; }
    public System.Action OnDead { get; set; }
    //추가적인 조건 체크 후 false면 데미지 적용 안함
    public System.Func<bool> AdditionalDamageableCondition { get; set; }
    public System.Action OnHeal { get; set; }

    public GameObject[] additionalEffectOnHit;
    public Vector3 prevAttackersPos { get; internal set; }
    public Combat(Transform owner, float maxHp, bool defaultEffectOnDamaged = true)
    {
        _owner = owner;
        _maxHp = maxHp;
        initalMaxHp = _maxHp;
        _hp = _maxHp;
        _defalutEffectOnDamaged = defaultEffectOnDamaged;
    }
    public float GetHp() { return _hp; }
    public void SetMaxHp(float maxHp)
    {
        _maxHp = maxHp;
        ResetDead();
    }
    public float GetMaxHp()
    {
        return _maxHp;
    }
    public void AddMaxHp(float add)
    {
        _maxHp = initalMaxHp + add;
    }
    public void ResetHp()
    {
        _hp = _maxHp;
        _dead = false;
    }
    public bool DealDamage(Combat target, float damage)
    {
        bool isAttackSucceeded = target.TakeDamage(damage);
        if (isAttackSucceeded)
        {
            OnDealWTarget?.Invoke(target);
            return false;
        }
        return true;
    }
    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTime)
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }
        bool result = true;
        if (AdditionalDamageableCondition != null)
        {
            result = result && AdditionalDamageableCondition.Invoke();
        }
        if (!result)
        {
            return false;
        }
        return true;
    }

    private void CalcTakeDamage(float damage)
    {
        _prevHitTime = Time.time; 
        damage = Mathf.Min(damage, _hp);//hp 0이하의 데미지를 주지 못하게

        _hp -= damage;
        OnDamaged?.Invoke();
        OnDamagedWDamage?.Invoke(damage);
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke();
        }
    }
    public bool TakeDamage(float damage)
    {
        return TakeDamage(_owner.position, damage);
    }
    public bool TakeDamage(Vector3 position, float damage, int effectType = -1)
    {
        if (!IsDamageable())
            return false;
        CalcTakeDamage(damage);
        prevAttackersPos = position;
        if (effectType != -1)
            EffectManager.Instance.HitEffectGenenate(position, effectType);
        return true;
    }
    public void Heal(int v)
    {
        if (_hp < _maxHp)
        {
            _hp += v;
        }
        if (OnHeal != null)
        {
            OnHeal.Invoke();
        }
    }
    public void Die()
    {
        TakeDamage(_hp);
    }
    public bool IsDead()
    {
        return _dead;
    }
    public void ResetDead()
    {
        _hp = _maxHp;
        _dead = false;
    }
}
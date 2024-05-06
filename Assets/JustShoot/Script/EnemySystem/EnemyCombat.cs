using UnityEngine;

//Todo 이름규칙 적용
[System.Serializable]
public class EnemyCombat : Combat
{
    public override void Init(Transform owner, float maxHp)
    {
        base.Init(owner, maxHp);
        OnDamagedWDamage += SpawnDamageNumberUi;
        _invincibleTime = 0f;
    }

    private void SpawnDamageNumberUi(float damage)
    {
        EffectManager.Instance.DamageNumberUiGenerate(_owner.transform , damage);
    }
}
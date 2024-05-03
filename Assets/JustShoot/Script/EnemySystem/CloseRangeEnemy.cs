using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        player = Player.Instance;
        Debug.Assert(player != null);// 플레이어나 널이면 경고
    }

    //적 공격 애니메이션에서 실행됨
    public void OnAimationAttak()
    {
        //DealDamage
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool closeEnogh = distance <= attackDistance;

        Vector3 enemyToPlayerDir = (-transform.position + player.transform.position).normalized;
        //참고 https://www.falstad.com/dotproduct/
        bool inAttackDirection = Vector3.Dot(transform.forward, enemyToPlayerDir) > .8f; // dot product 로 적이 보는 방향과 적의 위치까지의 방향이 비슷하면 데미지

        bool damagable = closeEnogh && inAttackDirection;

        if (damagable)//Todo: 공격 거리 계산을 다시 하고 싶을 수 있음
        {
            //플레이어에게 데미지 추가
            Debug.Log("Player Damaged!!");
        }
    }
}

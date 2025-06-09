using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public GameObject sword;
    public bool canAttack = true;
    public float attackCooldown = 1.0f;

    private void Start()
    {
        sword.SetActive(false);
        canAttack = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (canAttack == true)
            {
                sword.SetActive(true);
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
        canAttack = false;
        Animator anim = sword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        sword.SetActive(false);
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int patience = 3;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float detectionRange = 8f; // how far you can be from the guy before he starts walking towards you
    [SerializeField] float attackRange = 1.25f; // how close he can get to you before he kills you
    [SerializeField] float attackCooldown = 1f;

    private float nextAttackTime;

    void Update()
    {
        if (patience <= 0)
        {
            return;
        }

        Component target = FindNearestTarget();
        if (target == null)
        {
            return;
        }

        Vector3 targetPosition = target.transform.position;
        targetPosition.y = transform.position.y;

        if (Vector3.Distance(transform.position, targetPosition) > attackRange)
        {
            transform.LookAt(targetPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            AttackTarget(target);
        }
    }

    Component FindNearestTarget()
    {
        Component nearest = null;
        float nearestDistance = detectionRange;

        foreach (PlayerMovement player in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= nearestDistance)
            {
                nearest = player;
                nearestDistance = distance;
            }
        }

        foreach (Phantom phantom in FindObjectsByType<Phantom>(FindObjectsSortMode.None))
        {
            float distance = Vector3.Distance(transform.position, phantom.transform.position);
            if (phantom.enabled && distance <= nearestDistance)
            {
                nearest = phantom;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    void AttackTarget(Component target)
    {
        if (target is Phantom phantom)
        {
            phantom.enabled = false;
        }
        else if (target is PlayerMovement player)
        {
            player.ResetPlayer();
        }

        patience--;
        nextAttackTime = Time.time + attackCooldown;
    }
}

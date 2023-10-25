//Controls enemy movement and attack

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    public bool enemyDamageOn;
    private NavMeshAgent navMeshAgent = null;
    bool enemyAttacking = false;
    public bool isConscious = true;

    Animator anim;

    static GameObject[] allPlayers;

    private void Start() {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.radius = UnityEngine.Random.Range(0.5f, 1.0f); //Randomizes movement slightly
        allPlayers = GameObject.FindGameObjectsWithTag("Player");

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas)) {
            transform.position = hit.position;
        }
    }

    //Constant enemy state check
    void Update() {
        if (!navMeshAgent.isStopped) {
            EnemyChase(); //Enemy chases shortest 
        }
        else if (!enemyAttacking) {
            StartCoroutine(EnemySlashAttack());
        }

        anim.SetFloat("Movement Magnitude", navMeshAgent.velocity.magnitude);

        if (navMeshAgent.isStopped) {
            anim.SetFloat("Movement Magnitude", 0);
        }
    }

    //Enemies chase players based on player agro amount and closest position
    void EnemyChase() {
        if (allPlayers.Length > 0 && isConscious) {
            float shortestDistance = Mathf.Infinity;
            float playerDistance;
            GameObject targetPlayer = null;
            float distance = 0;
            foreach (var player in allPlayers) {
                playerDistance = Vector3.Distance(transform.position, player.transform.position) - player.GetComponent<PlayerManager>().agroAmount.Value;
                if (playerDistance < shortestDistance && player.GetComponent<PlayerManager>().AliveState) {
                    targetPlayer = player;
                    shortestDistance = playerDistance;
                    targetPlayer = player;
                }
            }
            //Checks to see if player is within range
            if (targetPlayer != null) {
                distance = Vector3.Distance(targetPlayer.transform.position, transform.position);
            }

            if (distance > 2) {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetPlayer.transform.position);
            }
            else {
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;
            }
        }
    }

    //Enemies attack if they're within striking distance
    IEnumerator EnemySlashAttack() {
        if (isConscious) {
            enemyAttacking = true;
            anim.CrossFade("Attack 1", 0.1f);
            yield return new WaitForSeconds(0.1f);
            if (enemyDamageOn) {
                Collider[] hitPlayers = Physics.OverlapSphere((gameObject.transform.forward * 2) + gameObject.transform.position, 2);
                foreach (Collider hit in hitPlayers) {
                    if (hit.gameObject.tag == "Player") {
                        hit.GetComponent<IHealth>().Damage(new(transform.position, Vector3.zero, GetComponent<EnemyManager>().baseDamage, DamageType.generic), gameObject);
                    }
                }
            }
            yield return new WaitForSeconds(1);
            navMeshAgent.isStopped = false;
            enemyAttacking = false;
        }
    }

    //Enemies can be stunned by external events (i.e. brute charge)
    public void StunInit() {
        StartCoroutine(EnemyStunDuration());
    }
    public IEnumerator EnemyStunDuration() {
        isConscious = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(2);
        navMeshAgent.isStopped = false;
        isConscious = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_bullScript : MonoBehaviour
{
    private Transform player;
    private float movementSpeed = 3f;
    private float changeDirectionInterval = 3f;

    private float directionTimer;
    private bool shouldChasePlayer = false;
    private int currentDirection = 1;
    private float direction = 0;

    public Animator animator;

    private float attackSpeed = 4f; //attack speed
    private float slowdownDuration = 0.3f; 
    private float speedupDuration = 0.3f;
    private float attackCooldown = 1f;

    private bool isAttacking = false;

    public AudioClip myClip;

    void Start()
    {
        directionTimer = changeDirectionInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        if (player != null)
        {
            //checking attack
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            float attackRange = 2f; // ���ù����ľ�����ֵ
            if (distanceToPlayer < attackRange || isAttacking)
            {
                Attack();
            } else
            {
                // ׷�ٳ���
                
                // ��ʱ���ݼ�
                directionTimer -= Time.deltaTime;

                // �жϹ��������Ƿ���ͬһ���߶�
                if (Mathf.Abs(transform.position.y - player.position.y) < 1f)
                {
                    shouldChasePlayer = true;
                }
                else
                {
                    shouldChasePlayer = false;
                }

                // �����Ƿ�׷�����ִ����Ӧ����Ϊ
                if (shouldChasePlayer)
                {
                    // ׷�����
                    transform.position = Vector3.MoveTowards(transform.position, player.position, movementSpeed * Time.deltaTime);
                    currentDirection = Random.Range(-1, 2);

                    // �����ƶ��ٶȺͳ���Ķ�������
                    animator.SetFloat("WalkSpeed", movementSpeed);
                    direction = Mathf.Sign(player.position.x - transform.position.x);
                }
                else
                {
                    // �ڲ�ͬ�߶�ʱ���������˶�
                    if (directionTimer <= 0f)
                    {
                        // �ı䷽��
                        currentDirection *= -1;
                        directionTimer = changeDirectionInterval;
                    }

                    // �����ƶ��ٶȺͳ���Ķ�������
                    animator.SetFloat("WalkSpeed", Mathf.Abs(currentDirection) * movementSpeed);
                    direction = currentDirection;

                    // ���������ƶ�
                    transform.Translate(new Vector3(currentDirection * movementSpeed * Time.deltaTime, 0f, 0f));
                }

                //modify the facing direction
                if (direction > 0)
                {
                    Transform transform = GetComponent<Transform>();
                    Vector3 scale = transform.localScale;
                    scale.x = -3;
                    transform.localScale = scale;
                }
                else if (direction < 0)
                {
                    Transform transform = GetComponent<Transform>();
                    Vector3 scale = transform.localScale;
                    scale.x = 3;
                    transform.localScale = scale;
                }
                
            }

        } else
        {
            animator.SetFloat("WalkSpeed", 0);
        }
    }


    public void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackCoroutine(player.position));
        }
    }

    private IEnumerator AttackCoroutine(Vector3 playerPosition)
    {
        isAttacking = true;
        animator.SetBool("Attack", true);
        float attackDirection = playerPosition.x - transform.position.x;

        if (attackDirection > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -3;
            transform.localScale = scale;
        }
        else if (attackDirection < 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 3;
            transform.localScale = scale;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / speedupDuration;
            float currentSpeed = Mathf.Lerp(0, attackSpeed, t);
            transform.Translate(new Vector3(attackDirection * currentSpeed * Time.deltaTime, 0f, 0f));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slowdownDuration;
            float currentSpeed = Mathf.Lerp(attackSpeed, 0, t);
            transform.Translate(new Vector3(attackDirection * currentSpeed * Time.deltaTime, 0f, 0f));
            yield return null;
        }

        animator.SetBool("Attack", false);
        animator.SetFloat("WalkSpeed", 0);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private int monsterHealth = 3;

    void OnTriggerEnter2D(Collider2D obj)
    {
        string name = obj.gameObject.name;

        // if collided with bullet
        if (name == "player_bullet(Clone)")
        {
            //AudioSource.PlayClipAtPoint(myClip, transform.position);

            // Reduce monster health
            monsterHealth -= 1;
            StartCoroutine(SlowDownMonster());

            // Check if monster is dead
            if (monsterHealth <= 0)
            {
                // Destroy the monster
                Destroy(gameObject);
            }
            else
            {
                // Make the monster blink
                StartCoroutine(BlinkMonster());

                // Play sound effect (optional)
                // AudioSource.PlayClipAtPoint(myClip, transform.position);
            }
        }
    }

    IEnumerator BlinkMonster()
    {
        // Get the original color of the monster
        Color originalColor = GetComponent<SpriteRenderer>().color;

        // Blink the monster for a few times
        for (int i = 0; i < 3; i++)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlowDownMonster()
    {
        movementSpeed = 1f;

        // �ȴ�����ʱ��
        yield return new WaitForSeconds(0.3f);

        movementSpeed = 3f;
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        string name = obj.gameObject.name;

        if (name == "player")
        {
            //AudioSource.PlayClipAtPoint(myClip, transform.position);

            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.BeAttacked();
        }
    }
}



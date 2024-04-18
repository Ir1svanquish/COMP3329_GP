using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour
{
    private int maxLife = 10;
    private int life = 10;
    public AudioClip myClip;
    private float delay = 1.4f; // ��������ֵ<=0ʱ�ȴ����ٶ��������ʱ��

    public GameObject shootPrefab;
    private float shootRotationAngle = 0f;
    private float spawnOffset = 0.6f;
    private float shootSpeed = 1.0f;

    private Rigidbody2D rb;
    private Vector2 v;

    public Animator animator;
    private float jumpForce = 50f;

    private int jumpCount = 0;
    private int maxJumpCount = 2;

    private int defaultLayer;
    private int hittedLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultLayer = LayerMask.NameToLayer("Default");
        hittedLayer = LayerMask.NameToLayer("Hitted player");
    }

    // Update is called once per frame
    void Update()
    {
        v = rb.velocity;
        v.x = Input.GetAxis("Horizontal") * 5;
        rb.velocity = v;

        animator.SetFloat("Horizontal", Math.Abs(v.x));

        if (v.x > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 3;
            transform.localScale = scale;

            shootRotationAngle = 0f;
            spawnOffset = 0.5f;
            shootSpeed = 1.0f;
        }
        else if (v.x < 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -3;
            transform.localScale = scale;

            shootRotationAngle = 180f;
            spawnOffset = -0.5f;
            shootSpeed = -1.0f;
        }

        // ��ȡattack������ֵ
        bool isAttacking = animator.GetBool("Attack");

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (jumpCount == 0 && jumpCount < maxJumpCount)
            {
                Jump();
                jumpCount++;
            } else if (jumpCount == 1 && jumpCount < maxJumpCount)
            {
                SecondJump();
                jumpCount++;
            }
        }

        if (life <= 0)
        {
            animator.SetBool("Alive", false);

            StartCoroutine(DestroyAfterDelay());
        }
    }

    void Attack()
    {
        // ���ù���״̬Ϊ true
        animator.SetBool("Attack", true);

        GameObject shoot = Instantiate(shootPrefab, transform.position, Quaternion.identity);
        shoot.transform.rotation = Quaternion.Euler(0f, 0f, shootRotationAngle);

        // ����shoot��λ�õ���
        Vector3 offset = transform.right * spawnOffset;
        shoot.transform.position += offset;

        Rigidbody2D shootRigidbody = shoot.GetComponent<Rigidbody2D>();
        Vector2 v = shootRigidbody.velocity;
        v.x = shootSpeed * 8;
        shootRigidbody.velocity = v;

        // ������ɺ����ù���״̬Ϊ false
        StartCoroutine(ResetAttackState());
    }

    IEnumerator ResetAttackState()
    {
        // ���蹥������ 1 ����
        yield return new WaitForSeconds(1f);

        // ���ù���״̬Ϊ false
        animator.SetBool("Attack", false);
    }

    public void BeAttacked()
    {
        // ���ñ�����״̬Ϊ true
        if (animator.GetBool("BeAttacked") == false) {
            animator.SetBool("BeAttacked", true);
            life--;

            gameObject.layer = hittedLayer;

            // ��������ɺ�����״̬Ϊ false
            StartCoroutine(ResetBeAttackedState());
            StartCoroutine(BlinkMonster());
        }
    }

    IEnumerator ResetBeAttackedState()
    {
        // ���蹥������ 1 ����
        yield return new WaitForSeconds(1f);

        if (life > 0)
        {
            // ���ù���״̬Ϊ false
            animator.SetBool("BeAttacked", false);

            gameObject.layer = defaultLayer;
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

    private void Jump()
    {
        // ������ʵ����Ծ�߼����������ɫʩ�����ϵ���
        v = rb.velocity;
        v.y = 0f;
        rb.velocity = v;
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("Jump", true);
    }
    private void SecondJump()
    {
        // ������ʵ����Ծ�߼����������ɫʩ�����ϵ���
        v = rb.velocity;
        v.y = 0f;
        rb.velocity = v;
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("SecondJump", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // ��ɫ��½��������Ծ����
            animator.SetBool("Jump", false);
            animator.SetBool("SecondJump", false);
        }
    }
    private IEnumerator DestroyAfterDelay()
    {
        // �ȴ�һ��ʱ��
        yield return new WaitForSeconds(delay);

        // ���ٵ�ǰ����
        Destroy(gameObject);
    }

    public int getMaxLife()
    {
        return maxLife;
    }
    public int getLife()
    {
        return life;
    }

    public void eatHPBottle()
    {
        int HP_up = 2;
        if (life < maxLife)
        {
            if (maxLife - life < HP_up)
            {
                life = maxLife;
            }
            else
            {
                life += HP_up;
            }
        }
    }

}
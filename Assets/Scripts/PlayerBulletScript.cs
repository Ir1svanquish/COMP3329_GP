using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
    public Animator animator;
    public GameObject splash1Prefab;
    public GameObject splash2Prefab;
    public GameObject splash3Prefab;
    private int speed = -6;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Gets called when the object goes out of the screen
    void OnBecameInvisible()
    {
        // Destroy the bullet
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        string tag = obj.gameObject.tag;

        // if collided with bullet
        if (tag == "enemy")
        {
            rb.velocity = Vector2.zero;

            animator.SetBool("hit", true);

            //GameObject splashEffect = Instantiate(splashPrefab, transform.position, Quaternion.identity);
            StartCoroutine(SpawnSplashEffect());

            Destroy(gameObject, 0.4f);
        }
    }

    IEnumerator SpawnSplashEffect()
    {
        yield return new WaitForSeconds(0.3f); // �ӳ�0.5��

        // ���ɽ���Ч��
        int randomIndex = Random.Range(1, 4);
        GameObject splashEffectPrefab;

        // ���������ѡ��Ҫʹ�õĽ���Ч��Ԥ����
        switch (randomIndex)
        {
            case 1:
                splashEffectPrefab = splash1Prefab;
                break;
            case 2:
                splashEffectPrefab = splash2Prefab;
                break;
            case 3:
                splashEffectPrefab = splash3Prefab;
                break;
            default:
                splashEffectPrefab = splash1Prefab;
                break;
        }

        GameObject splashEffect = Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        //Destroy(splashEffect, 1f); // 1������ٽ���Ч����������Ҫ���е�����
    }
}

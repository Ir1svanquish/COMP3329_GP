using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_eagle_bulletScript : MonoBehaviour
{
    private float speed = 3f; // �ӵ��ٶ�
    private float rotationSpeed = 200f; // �����ٶ�

    private Transform player; // ��ҵ�λ��
    private Vector3 direction; // �ӵ��ƶ�����

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        direction = (player.position - transform.position).normalized;
    }

    private void Update()
    {
        // �ӵ��ƶ�
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // �ӵ�����
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        string name = obj.gameObject.name;

        // if collided with bullet
        if (name == "player")
        {
            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.BeAttacked();

            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // Configuration Parameters
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int enemyScore = 50;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;

    [Header("VFX,SFX")]
    [SerializeField] GameObject vfxPrefab;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] [Range(0,1)] float volume = 100f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootVolume = 100f;

    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
        {
            return;
        }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(enemyScore);
        Destroy(gameObject);
        GameObject explosion = Instantiate(vfxPrefab, transform.position, transform.rotation);
        Destroy(explosion, 1f);
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position,volume);
    }
}

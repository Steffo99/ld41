﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareTower : MonoBehaviour {

    public Sprite standardSprite;
    public Sprite alternateSprite;
    public GameObject projectileObject;
    public float maxRange;
    public float maxDamage;

    private AudioSource snareSource;
    private SongData songData;
    private SpriteRenderer spriteRenderer;
    private float cooldown;
    private float cooldownRemaining = 0;

    void Start()
    {
        snareSource = gameObject.GetComponent<AudioSource>();
        songData = GameObject.FindGameObjectWithTag("GameController").GetComponent<SongData>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //The period is 120 / bpm
        cooldown = 44 / songData.bpm;
        //Find next beat
        float nextBeatIn = 180 / songData.bpm - (songData.songTime - (Mathf.Floor(songData.songTime / 120 * songData.bpm) * 120 / songData.bpm));
        InvokeRepeating("OnBeat", nextBeatIn, 120 / songData.bpm);
    }

    void Update () {
        cooldownRemaining -= Time.deltaTime;
        if (cooldownRemaining <= 0)
        {
            cooldownRemaining = 0;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Calculate the power
            float power = Mathf.Pow(Mathf.Abs(Mathf.Sin(songData.songTime * Mathf.PI * songData.bpm / 120)) * ((cooldown - cooldownRemaining) / cooldown), 2);
            //Play the sound
            snareSource.volume = power;
            snareSource.Play();
            //Instantiate the projectile
            GameObject projectile = Instantiate(projectileObject, transform.position, transform.rotation);
            SnareCollision sc = projectile.GetComponent<SnareCollision>();
            sc.maxRange = power * maxRange;
            sc.damage = Mathf.CeilToInt(power * maxDamage);
            projectile.transform.localScale = new Vector3(power, power, 1);
            //Start the cooldown
            cooldownRemaining = cooldown;
            //Change the sprite
            spriteRenderer.sprite = alternateSprite;
            Invoke("OnEndAnimation", Time.fixedDeltaTime * 10);
        }
	}

    void OnEndAnimation()
    {
        spriteRenderer.sprite = standardSprite;
    }

    void OnBeat()
    {
        spriteRenderer.color = Color.yellow;
        Invoke("ResetColor", Time.fixedDeltaTime * 2);
    }

    void ResetColor()
    {
        spriteRenderer.color = Color.white;
    }
}

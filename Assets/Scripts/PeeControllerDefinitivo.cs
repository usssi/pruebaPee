using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class PeeControllerDefinitivo : MonoBehaviour 
{
    [Header("GeneralPee")]
    public float capacidadPee = 100;
    public float velocidadDeHacerPipi = 1;

    [Header("Particulas")]
    public GameObject peertycleSystem;
    public GameObject salpicaduras;
    public GameObject charcoEmissor;
    public GameObject charco;
    public float thresholdPeeRunout = 50f;
    public float speedGravityMultiplier = 1.5f;
    public float maxPartycleMultiplier = 1;
    public float maxPartycle = 10;
    public float targetAlpha = 25;
    public Color targetColor = Color.white.WithAlpha(.25f);

    public float colorChangeMultiplier= 1f;


    [Header("Panza")]
    public GameObject panza;
    public float minPanzaScaleZ = 0.65f;
    public float maxPanzaScaleZ = 0.9f;

    private void Start()
    {
        
    }

    private void Update()
    {
        PeeFeedbackRunningOut();
    }

    void MakePeeGoDown()
    {
        capacidadPee -= velocidadDeHacerPipi * Time.deltaTime;
    }

    public void MakePanzaGoDown()
    {
        MakePeeGoDown();

        float t = capacidadPee / 100f;
        float newScaleZ = Mathf.Lerp(minPanzaScaleZ, maxPanzaScaleZ, t);

        Vector3 newScale = panza.transform.localScale;
        newScale.z = newScaleZ;
        panza.transform.localScale = newScale;
    }

    public void StartPeeingPartycles()
    {

    }

    public void StopPeeingPartycles()
    {

    }

    public void ToggleParticles(bool canParticle)
    {
        if (canParticle)
        {
            peertycleSystem.SetActive(true);
            salpicaduras.SetActive(true);
            charcoEmissor.SetActive(true);
            charco.SetActive(true);
        }
        else
        {
            peertycleSystem.SetActive(false);
            salpicaduras.SetActive(false);
            charcoEmissor.SetActive(false);
            charco.SetActive(false);
        }
    }

    public void TogglePeeNoise(bool canNoise)
    {
        if (canNoise)
        {
            ParticleSystem particleSystem = peertycleSystem.GetComponent<ParticleSystem>();
            ParticleSystem.NoiseModule noiseModule = particleSystem.noise;
            noiseModule.enabled = true;
        }
        else
        {
            ParticleSystem particleSystem = peertycleSystem.GetComponent<ParticleSystem>();
            ParticleSystem.NoiseModule noiseModule = particleSystem.noise;
            noiseModule.enabled = false;
        }
    }

    public void PeeFeedbackRunningOut()
    {
        if (capacidadPee <= thresholdPeeRunout)
        {
            print("Comienza feedback particulas de pee");

            ParticleSystem.MainModule particleSystem = peertycleSystem.GetComponent<ParticleSystem>().main;
            ParticleSystem.MainModule particleSystemSalpicaduras = salpicaduras.GetComponent<ParticleSystem>().main;
            ParticleSystem.MainModule particleSystemCharco = charco.GetComponent<ParticleSystem>().main;

            //gravity
            particleSystem.gravityModifierMultiplier += speedGravityMultiplier * Time.deltaTime;

            //max particles         
            if (maxPartycle>0)
            {
                maxPartycle -= maxPartycleMultiplier * Time.deltaTime;
            }
            else
            {
                maxPartycle = 0;
                particleSystemCharco.startColor = Color.clear;
            }
            particleSystem.maxParticles = (int)maxPartycle;

            //alpha
            Color startColor = particleSystem.startColor.color;

            if (targetColor.a > targetAlpha)
            {
                targetColor.a -= .1f * colorChangeMultiplier;
            }

            particleSystem.startColor = targetColor;
            particleSystemSalpicaduras.startColor = targetColor;
            particleSystemCharco.startColor = targetColor/2;

        }
    }
}
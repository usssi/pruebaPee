using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class ControlGame : MonoBehaviour
{
    [Header("Referencias")]
    public Points pointController;
    public PeeControllerDefinitivo peeController;
    public ControladorDePastillas pastillasControl;
    public runnerController runnerController;

    [Header("General Game Stuff")]
    public string CondicionDeVictoria;
    public bool canLose;
    public bool togglePeeParticles;
    public bool togglePeeNoise;
    public int pointsToWin;

    [Header("Agua De Inodoro Pee")]
    public GameObject pissWater;
    public float minPissWaterY;
    public float maxPissWaterY;

    [Header("Canvas")]
    public Camera cameraLose;
    public Canvas canvasWin;
    public Canvas canvasLose;
    public Canvas canvasPause;
    public Canvas canvasPlay;
    public Canvas canvasEnd;


    [Header("Pastillas")]
    public int metaDisolver;

    [Header("runner")]
    public bool playerKill;

    [Header("audioMixer")]
    public AudioMixer mixer;
    public string musicalevelstopper;
    public string ambientlevelstopper;
    public bool hasWon;



    private void Start()
    {
        NormalAusio();

        canvasPlay.enabled = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        cameraLose.enabled = false;

        pointController.pointsToWin = pointsToWin;

        pastillasControl.metaDisolver = metaDisolver;
        pastillasControl.metaDisolverTexto = metaDisolver;

    }

    private void Update()
    {
        if (canLose)
        {
            DeterminarCondicionDeVictoria();
            peeController.MakePanzaGoDown();
        }
        else
        {
            MakePissWaterGoAccordingToPoints();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePantallaPausa();
        }

        peeController.ToggleParticles(togglePeeParticles);
        peeController.TogglePeeNoise(togglePeeNoise);
    }

    void DeterminarCondicionDeVictoria()
    {
        if (CondicionDeVictoria == "llenarInodoro")
        {
            MakePissWaterGoAccordingToPoints();

            if (pointController.points >= pointsToWin)//cuando la puntuacion llega a puntos para ganar
            {

                PantallaGanada();
            }
            else if (peeController.capacidadPee <= 0)//cuando se te acaba el pis
            {
                PantallaPerdida();
            }
        }

        if (CondicionDeVictoria == "bugs")
        {
            if (pastillasControl.pastillasDisueltas >= metaDisolver)//cuando la puntuacion llega a puntos para ganar
            {

                Invoke("PantallaGanada", 1.5f);
            }
            else if (peeController.capacidadPee <= 0)//cuando se te acaba el pis
            {
                pastillasControl.isLost = true;
                Invoke("PantallaPerdida", 1.5f);
            }
        }

        if (CondicionDeVictoria == "runner")
        {
            runnerController.isDead = playerKill;
            if (playerKill == true)
            {
                Invoke("PantallaEnd", .1f);
            }
        }
    }

    #region region UI
    void PantallaGanada()
    {
        FindObjectOfType<AudioManager>().Stop(musicalevelstopper);
        FindObjectOfType<AudioManager>().Stop(ambientlevelstopper);
        stopPeeSounds();

        if (!hasWon)
        {
            FindObjectOfType<AudioManager>().Play("applause", 1f);
            FindObjectOfType<AudioManager>().Play("cheer", 1f);
            hasWon = true;
        }

        Cursor.visible = true;
        canvasWin.enabled = true;
        cameraLose.enabled = true;
        canvasPlay.enabled = false;

        PausseTime();
    }

    void PausseTime()
    {
        stopPeeSounds();
        Time.timeScale = 0;

    }

    void PantallaPerdida()
    {
        stopPeeSounds();
        DuckAudio();
        Time.timeScale = 0;
        Cursor.visible = true;
        canvasLose.enabled = true;
        cameraLose.enabled = true;
        canvasPlay.enabled = false;
    }

    void PantallaEnd()
    {
        FindObjectOfType<AudioManager>().Stop(musicalevelstopper);
        FindObjectOfType<AudioManager>().Stop(ambientlevelstopper);

        stopPeeSounds();

        if (!hasWon)
        {
            FindObjectOfType<AudioManager>().Play("applause", 1f);
            FindObjectOfType<AudioManager>().Play("cheer", 1f);
            hasWon = true;
        }

        Time.timeScale = 0;
        Cursor.visible = true;
        canvasLose.enabled = false;
        canvasWin.enabled = false;
        canvasPlay.enabled = false;
        canvasEnd.enabled = true;
        cameraLose.enabled = true;
    }

    public void TogglePantallaPausa()
    {

        if (canvasLose.enabled == false && canvasWin.enabled == false && canvasEnd.enabled == false)
        {
            FindObjectOfType<AudioManager>().Play("button2", 1f);

            if (canvasPause.enabled == true)
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                canvasPause.enabled = false;
                cameraLose.enabled = false;

                canvasPlay.enabled = true;
                NormalAusio();
            }
            else
            {
                DuckAudio();
                Time.timeScale = 0;
                Cursor.visible = true;
                canvasPause.enabled = true;
                cameraLose.enabled = true;

                canvasPlay.enabled = false;
            }
        }
    }
    #endregion

    void MakePissWaterGoAccordingToPoints()
    {
        float t = pointController.points / 100f;
        float newY = Mathf.Lerp(minPissWaterY, maxPissWaterY, t);
        Vector3 pos = pissWater.transform.position;
        pos.y = newY;
        pissWater.transform.position = pos;
    }

    void DuckAudio()
    {
        mixer.SetFloat("musicCutoff", 300f);
        mixer.SetFloat("sfxCutoff", 500f);
    }
    void NormalAusio()
    {
        mixer.SetFloat("musicCutoff", 22000);
        mixer.SetFloat("sfxCutoff", 22000);
    }

    void stopPeeSounds()
    {
        FindObjectOfType<AudioManager>().Stop("pis2");
        FindObjectOfType<AudioManager>().Stop("pisSUELO");
        FindObjectOfType<AudioManager>().Stop("pisborde");

    }

}
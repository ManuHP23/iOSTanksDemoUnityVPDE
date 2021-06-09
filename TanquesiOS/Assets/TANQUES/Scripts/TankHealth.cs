using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;


public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f; //nivel de salud inicial para el tanque
    public Slider m_Slider; //referencia al slider
    public Image m_FillImage; // ref image
    public Color m_FullHealthColor = Color.green; // color vida llena
    public Color m_ZeroHealthColor = Color.red; // color vida baja
    public GameObject m_ExplosionPrefab; //prefab de explosion del tanque: efecto de particulas + audio

    private AudioSource m_ExplosionAudio; //ref audio
    private ParticleSystem m_ExplosionParticles; // ref sist part
    private float m_CurrentHealth; //al inicio vale lo mismo que la variable publica
    private bool m_Dead; //variable bool para el evento muerte. Los tanque se desactivan al morir y se activan al inicio de la partida

    public Camera m_MainCamera; //camara del jugador



    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>(); //se instancia el prefab de la explosion
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>(); 

        m_ExplosionParticles.gameObject.SetActive(false); //una vez activado se desactiva
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth; //salud inicial la misma que hemos puesto
        m_Dead = false; //tanque vivo

        SetHealthUI(); //actualizar barra de salud del tanque
    }

    public void TakeDamage(float amount) //metodo para recibir daño. 
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount; //Se resta a la vida la cantidad de daño.

        SetHealthUI(); //actualizar la barra de vida
        if (m_CurrentHealth <= 0f && !m_Dead) //comprobar si el tanque ha muerto
        {
            OnDeath(); //metodo muerte
        }
    }


    private void SetHealthUI() //actualizar barra de vida y color
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth); //cambio de color gradual 
    }


    private void OnDeath() //metodo muerte
    {

        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true; //muerte es true
        m_ExplosionParticles.transform.position = transform.position; //explosíon en la posicion del tanque
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        m_MainCamera.transform.parent = null; //cuando se muere la camara deja de ser parent del tanque y así no se desactiva

        //Handheld.Vibrate(); //Vibracion
        Vibration.Init();

        Vibration.VibrateNope();

        gameObject.SetActive(false);

        Invoke("LoadMenu", 3f);
        
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
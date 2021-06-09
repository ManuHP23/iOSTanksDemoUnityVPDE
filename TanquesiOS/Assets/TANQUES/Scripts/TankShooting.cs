using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.CrossPlatformInput;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;   //numero del jugador q esta manejando el tanque. Se necesita saber que jugador lo controla para los inputs (fire).
    public Rigidbody m_Shell;   //ref al rb del proyectil q dispara el tanque         
    public Transform m_FireTransform;   //ref al objeto desde donde salen los proyectiles 
    public Slider m_AimSlider;    //ref al slider de carga       
    public AudioSource m_ShootingAudio;  //ref al audiosource de disparo
    public AudioClip m_ChargingClip; //ref al sonido de carga
    public AudioClip m_FireClip;   //ref al sonido de disparo  
    public float m_MinLaunchForce = 15f; //fuerza min de disparo
    public float m_MaxLaunchForce = 30f; //fuerza max de disparo
    public float m_MaxChargeTime = 0.75f; //max tiempo de carga de disparo

    private string m_FireButton; //eje de disparo del disparo del jugador (fire1)        
    private float m_CurrentLaunchForce;  //fuerza de disparo actual. se ire incrementando cada fotograma que se sontenga la tecla de disparo
    private float m_ChargeSpeed; // velocidad con la q se carga la energia del disparo      
    private bool m_Fired; //para saber si ya se ha disparado el proyectil             


    private void OnEnable() //cuando el tanque se activa se inicializa la fuerza actual de disparo a la min y el slider al minimo
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; //obtener el nombre del eje que se usa para disparar

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime; //calculo de velocidad de carga de la energia (diferencia entre max y min entre el tiempo de carga)
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce; //inicializar el valr de slider a la fuerza minima

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) //ya hemos cargado la energía maxima para disparar?
        {   
            // at max charge, not yet fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        } else if(CrossPlatformInputManager.GetButtonDown(m_FireButton)){
            //else if(Input.GetButtonDown(m_FireButton)){
            // have we pressed fire for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();

        } else if(CrossPlatformInputManager.GetButton(m_FireButton) && !m_Fired){
            //else if(Input.GetButton(m_FireButton) && !m_Fired){
            // Holding the fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;

        } else if(CrossPlatformInputManager.GetButtonUp(m_FireButton) && !m_Fired){
            //else if(Input.GetButtonUp(m_FireButton) && !m_Fired){
            // we released the button, having not fired yet
            Fire();
        }//el disparo ocurre cuando se llega a la fuerz amax o cuando se deja de pulsar
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true; //hemos disparado

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody; //instanciar el proyectil en la posicion y rotacion del firetransform
        
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; //establecer la velocidad del rb
        
        m_ShootingAudio.clip = m_FireClip; 
        m_ShootingAudio.Play();
        
        m_CurrentLaunchForce = m_MinLaunchForce; //volver a fuerza minima de disparo una vez disparado

    }
}
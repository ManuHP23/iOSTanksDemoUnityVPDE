using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;   //color del tanque del jugador         
    public Transform m_SpawnPoint;     //punto de inicio del jugador    
    [HideInInspector] public int m_PlayerNumber;    //numero del jugador         
    [HideInInspector] public string m_ColoredPlayerText; //texto del jugador con color
    [HideInInspector] public GameObject m_Instance;     // ref a la instancia del tanque en la escena     
    [HideInInspector] public int m_Wins;   //numero de victorias del tanque                  


    private TankMovement m_Movement;    // ref del script movimiento  
    private TankShooting m_Shooting;  //ref script disparo
    private GameObject m_CanvasGameObject; //ref al canvas q contiene la barra de energía y vida. 


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>(); //guardamos la instancia a los componentes del tanque
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;  //se incluyen a las variables el numero del jugador
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>"; //texto con el jugador y su color

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>(); //pintar el tanque con el color de la variable color del jugador

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl() //desactiva control sobre movimiento y disparo y desactiva canvas
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl() //activa control sobre movimiento y disparo y desactiva canvas
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset() //resetar el numero de tanques despues de desactivas los que quedan activos
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput; //para el cambio de plataforma

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1; //numero del jugador que controla el tanque. m_ para saber que la variable es un miembro de la clase. En csharp no hace falta.         
    public float m_Speed = 12f; // velocidad en unidades por segundos          
    public float m_TurnSpeed = 180f;      //velocidad en grados por segundo 
    public AudioSource m_MovementAudio;  //variable del componente audiosource que controla el sonido del motor  
    public AudioClip m_EngineIdling;    //referencia al sonido del motor del tanque estatico  
    public AudioClip m_EngineDriving;    //referencia al sonido del motor del tanque en movimiento
    public float m_PitchRange = 0.2f;  //rango en el que varia el tono del sonido del motor (pitch). Como el pitch inicial es 1 y el rango vale 0.2f, el tono varia desde 0.8f a 1.2f.


    private string m_MovementAxisName;    //nombre del eje que controla el movimiento del tanque. El movimiento es en el eje vertical.
                                          //Hay definidos 2 en el Input Manager: uno con las teclas W S, y otro con las teclas Up Down.

    private string m_TurnAxisName;         // nombre del eje que controla el giro del tanque. El movimiento es en el eje horizontal.
                                           // Hay definidos 2 en el imput manager: uno con las teclas A D, y otro con las teclas Left Right.
    private Rigidbody m_Rigidbody;         // referencia al rigidbody del tanque. Para aplicar las físicas (no se mueve con el transform).
    private float m_MovementInputValue;    // valor del eje vertical de movimiento. Los valores van desde -1 (al pulsar S o Down) hasta 1 (pulsar W o Up) pasando por 0 (no puldar nada).
    private float m_TurnInputValue;        // valor del eje horizontal de giro. Los valores van desde -1 (al pulsar A o Left) hasta 1 (pulsar D o Right) pasando por 0 (no puldar nada).
    private float m_OriginalPitch;         // valor original del tono del motor.


    private void Awake() //referencia a los componentes
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable() //se ejecuta cuando el objeto se activa en la escena
                            // cuando el tanaque se active en la escena, su rigbody deja de ser kinematic y se inicializan sus valores de movimiento y giro a cero.
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable() //se ejecuta cuando el objeto se desactiva en la escena
    {
        m_Rigidbody.isKinematic = true; //cuando el tanque se desactiva no hace falta su rb y no sera afectado por fisicas
    }


    private void Start() //ahora mismo el juego está configarado para un máximo de 2 tanques. Para un 3 habria que añadir nuevos inputs. Solo usare para esta tarea 1 tanque.
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }


    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        //m_MovementInputValue = Input.GetAxis(m_MovementAxisName); //almacena valor de movimiento vertical
        m_MovementInputValue = CrossPlatformInputManager.GetAxis(m_MovementAxisName); //cambio de plataforma

        //m_TurnInputValue = Input.GetAxis(m_TurnAxisName); //almacena valor de movimiento horizontal
        m_TurnInputValue = CrossPlatformInputManager.GetAxis(m_TurnAxisName); 

        EngineAudio(); //llamaremos al método de sonido del motor (que reproduce segun el valor del movimiento)
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate() //En el update obtenemos los valores de movimiento y giro y en el FixedUpdate (para físicas) los aplicamos.
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime; //definimos el vector de movimiento. Forward es el vector unitario qeu apunta a +Z, hacia adelante.
                                                                                                // la longitud del vector varia segun la velocidad. 

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement); //se le añade el desplazamiento a la posicion inicial
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime; //obtener los grados en los que gira el tanque. Calculado por el valor de giro y velocidad por fotograma (sin Time.deltaTime seria por segundo). 
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f); //unity almacena los giros en un valor quaternion (solo gira en el eje Y). Esta es la rotacion relativa.
        m_Rigidbody.MoveRotation((m_Rigidbody.rotation * turnRotation)); //Rotacion absoluta con el movimiento
    }
}
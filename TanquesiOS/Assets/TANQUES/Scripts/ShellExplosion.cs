using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask; //guarda la referencia a la capa (layer) players porque solo afectara a los objetos en esta capa
    public ParticleSystem m_ExplosionParticles;    // referencia al sistema de particulas   
    public AudioSource m_ExplosionAudio;  //referencia al audiosource            
    public float m_MaxDamage = 100f;    //daño máximo de la bala si el proyectil da en el centro del objeto           
    public float m_ExplosionForce = 1000f;    // fuerza de la explosion        
    public float m_MaxLifeTime = 2f;    //tiempo de vida del proyectil              
    public float m_ExplosionRadius = 5f;     //radio de la explosion         


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime); //el objeto se destruye al finalizar su tiempo de vida
    }


    private void OnTriggerEnter(Collider other) // metodo que se llama al colisionar el collider trigger de la bala con el objeto de la capa player
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); //simula una esfera definida por una posicion y un radio. solo se buscan colliders dentro de la capa (mask)

        for(int i = 0; i < colliders.Length; i++){ //recorremos todos los colliders
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); //definimos los rb de los objetos

            if (!targetRigidbody)
                continue; //no es un tanque

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius); //aplicamos fuerza de explosion

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>(); //obtenemos la salud del tanque
            
            if (!targetHealth)
                continue;
            
            float damage = CalculateDamage(targetRigidbody.position); //calculamos el daño en base a la colision

            targetHealth.TakeDamage(damage); //le aplicamos el daño
        }
        //tanbto si colisionamos con un tanque como si no
        m_ExplosionParticles.transform.parent = null; //hacer que el efecto de particulas deje de ser hijo del proyectil para que el efecto de particulas continue y no sea destruido
        
        m_ExplosionParticles.Play(); //efecto de particulas

        m_ExplosionAudio.Play(); //sonido explosion

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration); //retrasamos la destruccion del efecto de partículas

        Destroy(gameObject);// destruimos el objeto
    }


    private float CalculateDamage(Vector3 targetPosition) //metodo para calcular el daño
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position; //calcular un vector entre la posicion del proyectil y el tanque

        float explosionDistance = explosionToTarget.magnitude;//distancia entre proyectil y bala, magnitud del vector

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius; //la distancia entre el tanque y el proyectil no debe ser mayor al radio de explosion

        float damage = relativeDistance * m_MaxDamage; //porcentaje de daño relativo

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Spaceship : MonoBehaviour
{
    private Rigidbody rigidBody;
    private AudioSource audioSource;

    [SerializeField]
    private float rcsThrust = 50f;

    [SerializeField]
    private float mainThrust = 750f;

    [SerializeField]
    private float levelLoadDelay = 2f;

    [SerializeField]
    private AudioClip mainEngine;

    [SerializeField]
    private AudioClip explosion;

    [SerializeField]
    private AudioClip successfulLanding;

    [SerializeField]
    private ParticleSystem mainEngineParticles;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private ParticleSystem successfulLandingParticles;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    /// <summary>
    /// Initialisation
    /// </summary>    
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>    
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    /// <summary>
    /// Handles play-testing options for debug purposes
    /// </summary>
    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    /// <summary>
    /// Handles player input for thrust
    /// </summary>
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    /// <summary>
    /// Applies thrust
    /// </summary>
    private void ApplyThrust()
    {
        float frameThrust = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * frameThrust);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        mainEngineParticles.Play();
    }

    /// <summary>
    /// Stops applying thrust
    /// </summary>
    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    /// <summary>
    /// Handles player input for rotation
    /// </summary>
    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;   // remove physics engine applied rotation

        float frameRotation = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * frameRotation);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * frameRotation);
        }
    }

    /// <summary>
    /// Handles collisions
    /// </summary>
    /// <param name="collision">The collision</param>
    private void OnCollisionEnter(Collision collision)
    {
        // ignore collisions when loading levels or when dead
        if (isTransitioning || collisionsDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag.ToUpper())
        {
            case "FRIENDLY":
                // do nothing
                break;
            case "FINISH":
                SuccessfulLanding();
                break;
            default:
                Crashed();
                break;
        }
    }

    /// <summary>
    /// Handles the player crashing
    /// </summary>
    private void Crashed()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        explosionParticles.Play();
        Invoke("LoadFirstScene", levelLoadDelay);
    }

    /// <summary>
    /// Handles the player successfully landing
    /// </summary>
    private void SuccessfulLanding()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(successfulLanding);
        successfulLandingParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    /// <summary>
    /// Loads the first scene
    /// </summary>
    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Loads the next scene
    /// </summary>
    private void LoadNextScene()
    {
        int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneBuildIndex = activeSceneBuildIndex + 1;

        if (nextSceneBuildIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneBuildIndex = 0;
        }

        SceneManager.LoadScene(nextSceneBuildIndex);
    }
}

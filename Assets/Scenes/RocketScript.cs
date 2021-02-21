using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour{
    // Rocket member enum declarations
    enum RocketState {Alive, Dying, Transcending}

    // Member variable declarations
    // [SerializeField] allows values to be set from Unity inspector
    [SerializeField] float rcsThrust = 100;
    [SerializeField] float mainThrust = 100;
    [SerializeField] AudioClip mainEngineTone;
    [SerializeField] AudioClip successTone;
    [SerializeField] AudioClip deathTone;
    [SerializeField] ParticleSystem thrusterParticleSys;
    [SerializeField] ParticleSystem successParticleSys;
    [SerializeField] ParticleSystem deathParticleSys;
    [SerializeField] ParticleSystem leftParticles;
    [SerializeField] ParticleSystem rightParticles;
    [SerializeField] GameObject floor;


    [SerializeField] float levelLoadDelayTime = 2f;

    Rigidbody rigidBody;
    AudioSource audiosource;
    RocketState state = RocketState.Alive;
    int sceneIndex = 0;

    // Start is called before the first frame update
    void Start() {
        // Gets a reference to the Rocket's RigidBody
        rigidBody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxSceneIndex = SceneManager.sceneCountInBuildSettings;
        print("Current index: " + sceneIndex + ", Max index: " + maxSceneIndex);
    }

    // Update is called once per frame
    void Update() {
        ProcessInput();
    }

    /// Called whenever the rocket contacts another collision box
    void OnCollisionEnter(Collision collision) {
        // Does not allow rockets to hit anything while 
        // transcending or dying.
        if (state != RocketState.Alive) {
            return;
        }

        switch (collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    /// Plays the success tone and goes to the next level.
    private void StartSuccessSequence() {
        state = RocketState.Transcending;
        audiosource.Stop();
        audiosource.PlayOneShot(successTone);
        thrusterParticleSys.Stop();
        successParticleSys.Play();
        Invoke("LoadNextScene", levelLoadDelayTime);
    }

    /// Plays death tone and resets the current level.
    private void StartDeathSequence() {
        state = RocketState.Dying;
        audiosource.PlayOneShot(deathTone);
        audiosource.Stop();
        thrusterParticleSys.Stop();
        leftParticles.Stop();
        rightParticles.Stop();
        deathParticleSys.Play();
        Invoke("ResetScene", levelLoadDelayTime);
    }

    /// Advances the player to the next level upon level completion.
    private void LoadNextScene() {
        int maxSceneIndex = SceneManager.sceneCountInBuildSettings;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        print("Current index: " + sceneIndex + ", Max index: " + maxSceneIndex);
        if (sceneIndex == (maxSceneIndex) - 1) {
            SceneManager.LoadScene(0);
        } else {
            SceneManager.LoadScene(sceneIndex + 1);
        }
    }

    /// Resets the current level upon player death.
    private void ResetScene() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    /// Recieves and deciphers input from the player.
    void ProcessInput() {
        if (state == RocketState.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    /// Handles rocket thrust logic.
    void RespondToThrustInput() {
        // Applies force to move the rocket as the space bar is depressed.
        if (Input.GetKey(KeyCode.Space)){
            ApplyThrust();
        } else {
            // Turns off the engine sound effect.
            audiosource.Stop();
            thrusterParticleSys.Stop();
        }
    }

    private void ApplyThrust() {
        if (Input.GetKey(KeyCode.Space)) {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);

            // If the engine sound isn't playing, play it.
            if (!audiosource.isPlaying) {
                audiosource.PlayOneShot(mainEngineTone);
            }

            thrusterParticleSys.Play();
        }
    }

    /// Handles rocket rotate logic.
    void RespondToRotateInput() {
        // Pauses the engine's rotation effects.
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            leftParticles.Play();
            rightParticles.Play();
        } else if (Input.GetKey(KeyCode.A)) {
            rightParticles.Play();
            transform.Rotate(-Vector3.forward * rotationThisFrame, Space.World);
        } else if (Input.GetKey(KeyCode.D)) {
            leftParticles.Play();
            transform.Rotate(Vector3.forward * rotationThisFrame, Space.World);
        } else {
            rightParticles.Stop();
            leftParticles.Stop();
        }

        HandleZRotation();
        // Resumes the engine's rotation effects.
        rigidBody.freezeRotation = false;
    }

    /// Rotates the ship so the cockpit is always facing upwards
    /// Cosmetic function.
    void HandleZRotation() {
        var zRotation = transform.rotation.eulerAngles.z;
        var yRotation = transform.rotation.eulerAngles.y;

        if (zRotation < 90) {
            transform.Rotate(0f, -5f, 0f, Space.Self);

        }
        print("z: " + zRotation);
        print("y: " + yRotation);


    }
}

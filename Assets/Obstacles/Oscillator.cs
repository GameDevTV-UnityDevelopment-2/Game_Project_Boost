using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField]
    private Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [SerializeField]
    private float period = 1f;

    private float movementFactor;   // 0 for not moved, 1 for fully moved
    private Vector3 startingPos;    // must be stored for absolute movement

    /// <summary>
    /// Initialisation
    /// </summary>
    void Start()
    {
        startingPos = gameObject.transform.position;
    }

    /// <summary>
    ///  Update is called once per frame
    /// </summary>
    void Update()
    {
        // protect against divide by zero
        if (period <= Mathf.Epsilon)
        {
            return;
        }

        float cycles = Time.time / period;          // grows continually from 0
        const float tau = Mathf.PI * 2;             // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // ranges between -1 and +1

        movementFactor = (rawSinWave / 2f) + 0.5f;

        Vector3 offSet = movementVector * movementFactor;
        gameObject.transform.position = startingPos + offSet;
    }
}

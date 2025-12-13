using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position, startPosition;
    [SerializeField] private float maxRandForcVal, startRollingForce;
    float forceX, forceY, forceZ;
    public string diceFaceNum;
    public bool isLanded = false;
    public bool firstThrow = false;
    private Transform[] diceFaces;


    void Awake()
    {
        startPosition = transform.position;

        diceFaces = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            diceFaces[i] = transform.GetChild(i);
        }

        Initialize();
    }

    private void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        rBody.useGravity = false;
        position = transform.position;
        isLanded = true;

        transform.rotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f));
    }

    public void RollDice()
    {
        if (!firstThrow)
        {
            firstThrow = true;
        }

        isLanded = false;
        diceFaceNum = "?";
        rBody.isKinematic = false;
        rBody.useGravity = true;

        float upForce = Random.Range(800, startRollingForce);
        forceX = Random.Range(0, maxRandForcVal);
        forceY = Random.Range(0, maxRandForcVal);
        forceZ = Random.Range(0, maxRandForcVal);

        rBody.AddForce(Vector3.up * upForce);
        rBody.AddTorque(forceX, forceY, forceZ);

        Debug.Log($"Кубик брошен! Сила: {upForce}, Вращение: ({forceX}, {forceY}, {forceZ}), Gravity: {rBody.useGravity}, Kinematic: {rBody.isKinematic}");
    }

    void FixedUpdate()
    {
        if (!isLanded && rBody != null && rBody.linearVelocity.magnitude < 0.1f && rBody.angularVelocity.magnitude < 0.1f)
        {
            Invoke(nameof(CheckDiceFace), 0.5f);
        }
    }

    private void CheckDiceFace()
    {
        if (rBody.linearVelocity.magnitude < 0.1f && rBody.angularVelocity.magnitude < 0.1f)
        {
            isLanded = true;

            float maxY = float.MinValue;
            Transform topFace = null;

            foreach (Transform face in diceFaces)
            {
                if (face.position.y > maxY)
                {
                    maxY = face.position.y;
                    topFace = face;
                }
            }

            if (topFace != null)
            {
                diceFaceNum = topFace.name;
                Debug.Log($"Выпало число: {diceFaceNum}");
            }
        }
    }

    public void ResetDice()
    {
        transform.position = startPosition;
        firstThrow = false;
        diceFaceNum = "?";
        CancelInvoke(nameof(CheckDiceFace)); 
        Initialize();
        isLanded = true;
        Debug.Log("Кубик сброшен и готов к броску");
    }


    void Update()
    {
        if (rBody != null)
        {
            bool canClick = isLanded || !firstThrow;

            if (Input.GetMouseButtonDown(0) && canClick)
            {
                if (GameController.Instance != null && GameController.Instance.IsCurrentPlayerBot())
                {
                    Debug.Log("Сейчас ход бота - ждите...");
                    return;
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                        {
                            firstThrow = true;
                        }
                        Debug.Log("Игрок кликнул на кубик!");
                        RollDice();
                    }
                }
            }
        }
    }
}
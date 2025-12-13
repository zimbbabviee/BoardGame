using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    TextMeshPro tMP;
    private void Awake()
    {
        tMP = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
        tMP.text = name;
        tMP.color = new Color32(
            (byte)Random.Range(0, 256), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }
}
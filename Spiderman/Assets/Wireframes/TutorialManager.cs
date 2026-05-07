using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] consejos;
    private int index = 0;

    void Start()
    {
        MostrarConsejo(index);
    }

    public void SiguienteConsejo()
    {
        consejos[index].SetActive(false);

        index++;
        if (index >= consejos.Length)
            index = 0;

        MostrarConsejo(index);
    }

    void MostrarConsejo(int i)
    {
        consejos[i].SetActive(true);
    }
}
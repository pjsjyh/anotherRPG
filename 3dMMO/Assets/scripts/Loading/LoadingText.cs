using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI dotText;
    int dotCount = 3;
    private void Awake()
    {
        dotText = this.GetComponent<TextMeshProUGUI>();
        StartCoroutine(textWord());
    }

    IEnumerator textWord() // 로딩 . . .
    {
        while (true)
        {
            dotCount = (dotCount % 3) + 1; 
            dotText.text = new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }
}

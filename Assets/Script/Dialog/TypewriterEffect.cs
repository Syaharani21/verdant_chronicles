using System.Collections;
using System.Collections.Generic; // Make sure to include this for Dictionary
using TMPro; // Include this for TMP_Text
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f; 

    public bool IsRunning{get; private set;}

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation (new HashSet<char>() { '.', '!', '?' }, 0.6f ),
        new Punctuation (new HashSet<char>() { ',', ';', ':' }, 0.3f)
    };

    private Coroutine typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel)
    {
        typingCoroutine = StartCoroutine(TypeTextCoroutine(textToType, textLabel));
    }

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeTextCoroutine(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
        textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t); // Calculate the number of characters to display
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length); 

            for (int i = lastCharIndex; i < charIndex; i++)
            { 
                bool isLast = i >= textToType.Length - 1; // Corrected to check bounds
                textLabel.text = textToType.Substring(0, i + 1); // Corrected substring call
                
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && i + 1 < textToType.Length && IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            yield return null; 
        }

        IsRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations)
        {
            if (punctuationCategory.Key.Contains(character))
            {
                waitTime = punctuationCategory.Value;
                return true;
            }
        }
        waitTime = default;
        return false;
    }

    private readonly struct Punctuation 
    {
        public readonly HasSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HasSet<char> punctuations, float waitTime )
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
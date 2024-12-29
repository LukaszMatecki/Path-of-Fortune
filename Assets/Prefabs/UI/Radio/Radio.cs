using UnityEngine;
using TMPro;

public class Radio : MonoBehaviour
{
    public TextMeshProUGUI songNameText; // TextMeshPro do wyœwietlania nazwy utworu
    public AudioClip[] songs;           // Tablica utworów
    private AudioSource audioSource;    // AudioSource z MusicManager
    private int currentSongIndex = 0;   // Indeks obecnego utworu
    private bool isPaused = false;      // Czy muzyka jest wstrzymana

    void Start()
    {
        // ZnajdŸ AudioSource na obiekcie MusicManager
        GameObject musicManager = GameObject.Find("MusicManager");
        if (musicManager != null)
        {
            audioSource = musicManager.GetComponent<AudioSource>();
        }
        if (audioSource == null)
        {
            Debug.LogError("Nie znaleziono komponentu AudioSource na MusicManager.");
            return;
        }

        // Zainicjuj pierwszy utwór
        PlaySong(currentSongIndex);
    }

    // Funkcja odtwarzaj¹ca utwór na podstawie indeksu
    public void PlaySong(int index)
    {
        if (index >= 0 && index < songs.Length)
        {
            audioSource.clip = songs[index];
            audioSource.Play();
            songNameText.text = songs[index].name; // Wyœwietl nazwê utworu
            currentSongIndex = index;
            isPaused = false; // Resetuj status pauzy
        }
    }

    // Funkcja do przejœcia do nastêpnego utworu
    public void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Length;
        PlaySong(currentSongIndex);
    }

    // Funkcja do przejœcia do poprzedniego utworu
    public void PreviousSong()
    {
        currentSongIndex = (currentSongIndex - 1 + songs.Length) % songs.Length;
        PlaySong(currentSongIndex);
    }

    // Funkcja do zatrzymania lub wznowienia muzyki
    public void TogglePauseResume()
    {
        if (isPaused)
        {
            // Wznów muzykê
            audioSource.Play();
            isPaused = false;
        }
        else
        {
            // Wstrzymaj muzykê
            audioSource.Pause();
            isPaused = true;
        }
    }
}
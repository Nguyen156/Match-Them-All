using UnityEngine;

public class AudioManager : MonoBehaviour, IGameStateListener
{
    public static AudioManager instance;

    [Header(" Elements ")]
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        PlayBGM(0);
    }

    public void PlayBGM(int bgmToPlay)
    {
        if (bgm.Length <= 0)
            return;

        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }

        bgm[bgmToPlay].Play();
    }

    public void StopBGM()
    {
        foreach(AudioSource source in bgm)
            source.Stop();
    }

    public void PlaySFX(int sfxToPlay)
    {
        if (sfxToPlay >= sfx.Length)
            return;

        sfx[sfxToPlay].Play();
    }

    public void StopSFX(int sfxToStop) => sfx[sfxToStop].Stop();

    public void GameStateChangedCallback(EGameState gameState)
    {
        if(gameState == EGameState.LEVELCOMPLETE || gameState == EGameState.GAMEOVER)
            StopBGM();
    }
}

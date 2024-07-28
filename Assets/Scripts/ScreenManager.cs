using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameScreen
{
    MainMenu,
    Settings,
    Pause,
    Tutorial,
    Play,
    GameOver
}

public class ScreenManager : MonoBehaviour
{
    public Dictionary<string, GameObject> unityObjects;
    private Stack<GameScreen> screens_stack;
    private GameScreen currScreen;
    public Spawner spawner;
    public StatSO stat;

    float timer;
    float seconds;
    float minutes;
    Coroutine coroutine;

    private AudioSource music;
    private AudioSource sfx;

    static ScreenManager instance;

    private bool isPaused;

    public static ScreenManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("Manager").GetComponent<ScreenManager>();

            return instance;
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Btn_PauseLogic()
    {
        ChangeScreen(GameScreen.Pause);
        Time.timeScale = 0;
        isPaused = true;
    }

    public void GameOverLogic()
    {
        StopCoroutine(coroutine);
        ChangeScreen(GameScreen.GameOver);
        Time.timeScale = 0;
    }

    public void Btn_PlayLogic()
    {
        ChangeScreen(GameScreen.Play);
        Time.timeScale = 1;
        if (!isPaused)
        {
            GameObject[] _objs = GameObject.FindGameObjectsWithTag("Object");
            if (_objs.Length > 0)
            {
                foreach (GameObject g in _objs)
                {
                    spawner.DestroyPool(g);
                }
            }
            spawner.Init();
            stat.InitDef();
            unityObjects["Health"].GetComponent<TextMeshProUGUI>().text
                    = stat.health.ToString();
            unityObjects["Point"].GetComponent<TextMeshProUGUI>().text
                    = stat.point.ToString();
            if (coroutine == null)
            {
                coroutine = StartCoroutine(StopWatchCalc(0f));
            }
            else
            {
                StopCoroutine(coroutine);
                coroutine = StartCoroutine(StopWatchCalc(0f));
            }
        }
        isPaused = false;
    }

    public void Btn_TutorialLogic()
    {
        ChangeScreen(GameScreen.Tutorial);
    }

    public void Btn_BackToMenu()
    {
        if(isPaused)
            isPaused = false;
        ChangeScreen(GameScreen.MainMenu);
        if (GameObject.Find("Screen_GameOver") != null)
            GameObject.Find("Screen_GameOver").SetActive(false);
    }

    public void Btn_SettingsLogic()
    {
        ChangeScreen(GameScreen.Settings);
        isPaused = false;
    }

    public void Btn_BackLogic()
    {
        GameScreen tempScreen = screens_stack.Pop();
        unityObjects["Screen_" + tempScreen].SetActive(true);
        unityObjects["Screen_" + currScreen].SetActive(false);
        currScreen = tempScreen;
    }

    private void Init()
    {
        screens_stack = new Stack<GameScreen>();
        currScreen = GameScreen.MainMenu;

        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _objs = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _objs)
            unityObjects.Add(g.name, g);
        Time.timeScale = 0;

        unityObjects["Screen_Play"].SetActive(false);
        unityObjects["Screen_Settings"].SetActive(false);
        unityObjects["Screen_GameOver"].SetActive(false);
        unityObjects["Screen_Tutorial"].SetActive(false);
        unityObjects["Screen_Pause"].SetActive(false);

        AudioSource[] sources = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>();
        music = sources[0];
        sfx = sources[1];
        unityObjects["Slider_MusicVolume"].GetComponent<Slider>().onValueChanged.AddListener(SetMusicVol);
        unityObjects["Slider_SfxVolume"].GetComponent<Slider>().onValueChanged.AddListener(SetSFXVol);

        unityObjects["Slider_MusicVolumeP"].GetComponent<Slider>().onValueChanged.AddListener(SetMusicVol);
        unityObjects["Slider_SfxVolumeP"].GetComponent<Slider>().onValueChanged.AddListener(SetSFXVol);
    }

    private void ChangeScreen(GameScreen _newScreen)
    {
        // If the screen has changed, do stack logic
        if (currScreen != _newScreen)
        {
            unityObjects["Screen_" + _newScreen].SetActive(true);
            unityObjects["Screen_" + currScreen].SetActive(false);
            screens_stack.Push(currScreen);
            currScreen = _newScreen;
        }
    }

    IEnumerator StopWatchCalc(float time)
    {
        timer = time;
        while (true)
        {
            timer += Time.deltaTime;
            seconds = (int)timer % 60;
            minutes = (int)timer / 60;
            if (seconds < 10)
            {
                unityObjects["Timer"].GetComponent<TextMeshProUGUI>().text = minutes.ToString() + ":0" + seconds.ToString();
            }
            else
            {
                unityObjects["Timer"].GetComponent<TextMeshProUGUI>().text = minutes.ToString() + ":" + seconds.ToString();
            }
            yield return null;
        }
    }

    public void Slider_MusicVolumeLogic()
    {
        unityObjects["MusicValue"].GetComponent<TextMeshProUGUI>().text
            = "Music: " + (int)unityObjects["Slider_MusicVolume"].GetComponent<Slider>().value;
    }

    public void Slider_SfxVolumeLogic()
    {
        unityObjects["SfxValue"].GetComponent<TextMeshProUGUI>().text
            = "SFX: " + (int)unityObjects["Slider_SfxVolume"].GetComponent<Slider>().value;
    }

    public void Slider_MusicVolumeLogicP()
    {
        unityObjects["MusicValueP"].GetComponent<TextMeshProUGUI>().text
            = "Music: " + (int)unityObjects["Slider_MusicVolumeP"].GetComponent<Slider>().value;
    }

    public void Slider_SfxVolumeLogicP()
    {
        unityObjects["SfxValueP"].GetComponent<TextMeshProUGUI>().text
            = "SFX: " + (int)unityObjects["Slider_SfxVolumeP"].GetComponent<Slider>().value;
    }

    private void SetMusicVol(float value)
    {
        music.volume = value / 10f;
    }

    private void SetSFXVol(float value)
    {
        sfx.volume = value / 10f;
    }

    public void PlayeSFX()
    {
        sfx.Play();
    }

    private void OnEnable()
    {
        stat.HealthChanged += isAlive;
        stat.PointChanged += printPoint;
    }

    private void OnDisable()
    {
        stat.HealthChanged -= isAlive;
        stat.PointChanged -= printPoint;
    }

    private void isAlive(int health)
    {
        if (health == 0)
        {
            GameOverLogic();
            unityObjects["GameOverPoint"].GetComponent<TextMeshProUGUI>().text
                = "Your Points is " + stat.point.ToString();
        }
        unityObjects["Health"].GetComponent<TextMeshProUGUI>().text
                = stat.health.ToString();
    }

    public void printPoint(int point)
    {
        unityObjects["Point"].GetComponent<TextMeshProUGUI>().text
                = stat.point.ToString();
    }

    public void AddHealth(int health)
    {
        stat.health += health;
    }

    public void AddPoint(int point)
    {
        stat.point += point;
    }
}

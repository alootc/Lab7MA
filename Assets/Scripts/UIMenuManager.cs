using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [Header("Información del Jugador")]
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private TMP_Text skillPointsText;
    [SerializeField] private TMP_Text totalClicksText;

    [Header("Barra de Experiencia")]
    [SerializeField] private Slider experienceBar;
    [SerializeField] private TMP_Text experienceBarText;

    [Header("Estadísticas")]
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text agilityText;

    [Header("Botones de Acción")]
    [SerializeField] private Button gainExperienceButton;
    [SerializeField] private Button addStrengthButton;
    [SerializeField] private Button addDefenseButton;
    [SerializeField] private Button addAgilityButton;
    [SerializeField] private Button resetDataButton;
    [SerializeField] private Button logoutButton;

    [Header("Editar Nombre")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button updateNameButton;

    [Header("Efectos de Nivel")]
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private AudioSource levelUpSound;

    private void Start()
    {
        SetupButtons();
        UpdateUI();

        PlayerDataManager.Instance.OnDataUpdated += UpdateUI;
    }

    private void OnDestroy()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.OnDataUpdated -= UpdateUI;
        }
    }

    private void SetupButtons()
    {
        gainExperienceButton.onClick.AddListener(GainExperience);
        addStrengthButton.onClick.AddListener(() => AddStatPoint("strength"));
        addDefenseButton.onClick.AddListener(() => AddStatPoint("defense"));
        addAgilityButton.onClick.AddListener(() => AddStatPoint("agility"));
        updateNameButton.onClick.AddListener(UpdatePlayerName);
        resetDataButton.onClick.AddListener(ResetPlayerData);
        logoutButton.onClick.AddListener(Logout);
    }

    private void GainExperience()
    {
        PlayerDataManager.Instance.AddExperience(25); 

        gainExperienceButton.transform.localScale = Vector3.one * 0.9f;
        Invoke(nameof(ResetButtonScale), 0.1f);
    }

    private void ResetButtonScale()
    {
        gainExperienceButton.transform.localScale = Vector3.one;
    }

    private void AddStatPoint(string stat)
    {
        PlayerDataManager.Instance.UseSkillPoint(stat);
    }

    private void UpdatePlayerName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            PlayerDataManager.Instance.UpdatePlayerName(nameInputField.text);
            nameInputField.text = "";
        }
    }

    private void ResetPlayerData()
    {
        PlayerDataManager.Instance.ResetPlayerData();
    }

    private void Logout()
    {
        UnityPlayerAuth.Instance.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    private void UpdateUI()
    {
        if (!PlayerDataManager.Instance.IsDataLoaded) return;

        var data = PlayerDataManager.Instance.CurrentPlayerData;

        playerNameText.text = data.playerName;
        levelText.text = $"Nivel {data.level}";
        experienceText.text = $"{data.experience} / {data.ExperienceRequiredForNextLevel} EXP";
        skillPointsText.text = $"{data.availableSkillPoints} Puntos";
        totalClicksText.text = $"Clicks: {data.totalClicks}";

        experienceBar.value = data.ExperienceProgress;
        experienceBarText.text = $"{Mathf.Round(data.ExperienceProgress * 100)}%";

        strengthText.text = $"Fuerza: {data.strength}";
        defenseText.text = $"Defensa: {data.defense}";
        agilityText.text = $"Agilidad: {data.agility}";

        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        var data = PlayerDataManager.Instance.CurrentPlayerData;

        bool hasSkillPoints = data.availableSkillPoints > 0;

        addStrengthButton.interactable = hasSkillPoints;
        addDefenseButton.interactable = hasSkillPoints;
        addAgilityButton.interactable = hasSkillPoints;

        Color enabledColor = hasSkillPoints ? Color.green : Color.gray;
        addStrengthButton.image.color = enabledColor;
        addDefenseButton.image.color = enabledColor;
        addAgilityButton.image.color = enabledColor;
    }

    public void PlayLevelUpEffect()
    {
        if (levelUpEffect != null)
        {
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        }

        if (levelUpSound != null)
        {
            levelUpSound.Play();
        }
    }
}
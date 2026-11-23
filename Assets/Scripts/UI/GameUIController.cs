using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("Элементы интерфейса")]
    [Tooltip("Слайдер здоровья игрока")]
    public Slider HealthSlider;

    [Tooltip("Текстовое поле для вывода числового значения HP")]
    public Text HealthText;

    [Tooltip("Текстовое поле для номера текущей волны")]
    public Text WaveText;

    private EntityManager _entityManager;
    private EntityQuery _playerQuery;
    private EntityQuery _waveQuery;

    private void Start()
    {
        // Получаем доступ к миру ECS по умолчанию
        if (World.DefaultGameObjectInjectionWorld != null)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // Создаем быстрые запросы для поиска сущности игрока и синглтона волн
            _playerQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerTag>(),
                ComponentType.ReadOnly<Health>()
            );

            _waveQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<WaveState>()
            );
        }
    }

    private void Update()
    {
        if (World.DefaultGameObjectInjectionWorld == null)
            return;

        if (!_playerQuery.IsEmpty)
        {
            Entity playerEntity = _playerQuery.GetSingletonEntity();
            // Считываем компонент Health через EntityManager
            Health playerHealth = _entityManager.GetComponentData<Health>(playerEntity);

            if (HealthSlider != null)
            {
                HealthSlider.maxValue = playerHealth.Max;
                HealthSlider.value = Mathf.Max(0, playerHealth.Current);
            }

            if (HealthText != null)
            {
                HealthText.text = $"HP: {Mathf.CeilToInt(Mathf.Max(0, playerHealth.Current))} / {playerHealth.Max}";
            }
        }
        else
        {
            // Если игрок уничтожен — показываем 0 HP
            if (HealthSlider != null) HealthSlider.value = 0;
            if (HealthText != null) HealthText.text = "HP: 0";
        }

        if (!_waveQuery.IsEmpty)
        {
            Entity waveEntity = _waveQuery.GetSingletonEntity();
            WaveState waveState = _entityManager.GetComponentData<WaveState>(waveEntity);

            if (WaveText != null)
            {
                WaveText.text = $"Волна: {waveState.CurrentWaveIndex + 1}";
            }
        }
    }
}
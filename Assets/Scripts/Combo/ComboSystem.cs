using System.Collections;
using System.Collections.Generic;
using Combo;
using UnityEngine;
using System.Linq;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] private ComboSequence comboSequence;
    [SerializeField] private PlayerController player;

    private int CountCombo { get; set; } = 0;
    private float _timer = 0f;
    private Queue<AttackDataSO> _attackQueue;
    private Dictionary<string, AttackDataSO> _attackDictionary;
    private Dictionary<CharacterState, List<ComboChain>> _combosByState;
    private Dictionary<string, float> _lastInputTime;

    private void Start()
    {
        InitializeDictionaries();
        // Группируем комбо по состояниям
        foreach (var combo in comboSequence.combos)
        {
            if (!_combosByState.ContainsKey(combo.requiredState))
            {
                _combosByState[combo.requiredState] = new List<ComboChain>();
            }
            _combosByState[combo.requiredState].Add(combo);
        }

        // Инициализируем словарь атак по идентификаторам
        InitializeAttackDataById();
    }

    private void InitializeDictionaries()
    {
        _attackQueue = new Queue<AttackDataSO>();
        _combosByState = new Dictionary<CharacterState, List<ComboChain>>();
        _lastInputTime = new Dictionary<string, float>();
        _attackDictionary = new Dictionary<string, AttackDataSO>();
    }

    private void InitializeAttackDataById()
    {
        var attackDatas = Resources.LoadAll<AttackDataSO>("PathToAttackData"); // Замените на реальный путь
        foreach (var attackData in attackDatas)
        {
            _attackDictionary[attackData.name] = attackData; // Предполагается, что имя используется как идентификатор
        }
    }

    public void Attack(AttackDataSO dataSo)
    {
        // Задержка перед ударом
        if (Time.time < _timer + dataSo.cooldown)
            return;

        _timer = Time.time;

        string currentAttackTypeId = dataSo.name; // Предполагается, что имя используется как идентификатор
        float bufferTimer = dataSo.bufferTime;
        // Реализация счетчика и его сбороса по таймеру
        if (Time.time <= _lastInputTime[currentAttackTypeId] + bufferTimer)
        {
            Debug.Log($"Type Attack {currentAttackTypeId} and value cooldown {bufferTimer}");
            _attackQueue.Enqueue(dataSo);

            Debug.Log(_attackQueue.Count);
            CheckForValidCombo();
        }
        else
        {
            _attackQueue.Clear();
            _attackQueue.Enqueue(dataSo);
        }

        _lastInputTime[currentAttackTypeId] = Time.time;
    }

    private void CheckForValidCombo()
    {
        if (!_combosByState.TryGetValue(player.CurrentState, out var stateCombos))
            return;

        foreach (var combo in stateCombos)
        {
            if (!CheckComboConditions(combo))
                continue;
            // Проверка чтобы в очереди было достаточно аттак для исполнения комбо
            if (_attackQueue.Count < combo.attackDateIds.Length)
                continue;

            // выражение убирает прошлые попытки сделать комбо - это нужно чтобы учитывались только последние исполнения
            var recentlyCombo = _attackQueue.Skip(_attackQueue.Count - combo.attackDateIds.Length).ToArray();
            var isComboValid = true;

            // Проверяется типы если они совпадают по рецепту то комбо воспроизводиться
            for (int i = 0; i < combo.attackDateIds.Length; i++)
            {
                if (recentlyCombo[i].name != combo.attackDateIds[i]) // Используем идентификатор для сравнения
                {
                    isComboValid = false;
                    break;
                }
            }

            // Воспроизведение комбо
            if (isComboValid)
            {
                Debug.Log("Combo valid");
                ExecuteCombo(combo);
                return;
            }

            if (combo.possibleBranches != null)
            {
                foreach (var branch in combo.possibleBranches)
                {
                    if (branch.branchCondition.CheckCondition(player, this))
                    {
                        ExecuteCombo(branch);
                    }
                }
            }
        }
    }

    private void ExecuteCombo(ComboChain combo)
    {
        var lastAttackId = combo.attackDateIds.Last();
        var lastAttack = _attackDictionary[lastAttackId];
        player.CurrentState = GetAttackState(lastAttack.attackTypeSo);
        Debug.Log($"Combo: {combo.requiredState} execute damage multiplier: {combo.damageMultiplier}");
        _attackQueue.Clear();
        CountCombo = 0;

        StartCoroutine(ResetAfterAnimation(lastAttack.animationLength));
    }

    private CharacterState GetAttackState(AttackTypeSO attackType)
    {
        return attackType.name.Contains("Foot") ? CharacterState.AttackFoot : CharacterState.AttackLight;
    }

    private bool CheckComboConditions(ComboChain combo)
    {
        if (combo.possibleBranches == null)
            return true;

        foreach (var branch in combo.possibleBranches)
        {
            if (!branch.branchCondition.CheckCondition(player, this))
                return false;
        }

        return true;
    }

    private IEnumerator ResetAfterAnimation(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);

        player.CurrentState = player.IsGrounded()
            ? (player.GetInputController().GetDirection().x != 0 ? CharacterState.Move : CharacterState.Idle)
            : CharacterState.Jumping;
    }

    public bool CanAttack(AttackTypeSO attackTypeSo, CharacterState currentState)
    {
        if (!_combosByState.TryGetValue(currentState, out var combos))
            return false;

        return combos.Any(c =>
            c.attackDateIds.Length > 0 && _attackDictionary[c.attackDateIds[0]].attackTypeSo == attackTypeSo);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ComboSequence comboSequence;
    [SerializeField] private ComboSystem comboSystem;
    private Dictionary<string, AttackDataSO> _attackDictionary;

    [SerializeField] private AttackInputBinding[] attackInputBindings;

    private void Start()
    {
        InitializationAttackDictionary();
        SetupInputHandler();
    }

    private void SetupInputHandler()
    {
        foreach (var binding in attackInputBindings)
        {
            binding.inputAction.action.performed += _ =>
            {
                var attackData = _attackDictionary[binding.attackType.name]; // Используем имя как идентификатор
                if (comboSystem.CanAttack(binding.attackType, playerController.CurrentState))
                {
                    comboSystem.Attack(attackData);
                }
            };
        }
    }

    private void InitializationAttackDictionary()
    {
        _attackDictionary = new Dictionary<string, AttackDataSO>();
        // Объединяем коллекции и убираем дубликаты
        var allAttackTypes = comboSequence.combos.SelectMany(combo => combo.attackDateIds).Distinct();
        // Загружаем AttackDataSO по идентификаторам
        var attackDatas = Resources.LoadAll<AttackDataSO>("PathToAttackData"); // Замените на реальный путь
        foreach (var attackData in attackDatas)
        {
            _attackDictionary[attackData.name] = attackData; // Предполагается, что имя используется как идентификатор
        }
    }
}


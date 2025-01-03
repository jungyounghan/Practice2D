using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stat
{
    //�ɷ�ġ ������ �з��ϴ� ������
    public enum Category: byte
    {
        BaseDamage,
        ExtensionDamage,
        AttackSpeed,
        BaseLife,
        GatherLife,
        ReduceDamage,
        BaseMana,
        GatherMana,
        MoveSpeed,
        JumpPower,
        End
    }

    //�����ϴ� ��ü�� �ɷ�ġ
    [Serializable]
    public struct Attacker
    {
        //�⺻ ���ݷ�
        public int baseDamage;
        //Ȯ�� ���ݷ�
        public uint extensionDamage;
        //���� �ӵ�
        public float attackSpeed;

        public Attacker(int baseDamage, uint extensionDamage, float attackSpeed)
        {
            this.baseDamage = baseDamage;
            this.extensionDamage = extensionDamage;
            this.attackSpeed = attackSpeed;
        }

        public Attacker(Dictionary<Category, float> dictionary)
        {
            baseDamage = 0;
            extensionDamage = 0;
            attackSpeed = 0;
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(Category.BaseDamage) == true)
                {
                    baseDamage = (int)Mathf.Clamp(dictionary[Category.BaseDamage], int.MinValue, int.MaxValue);
                }
                if (dictionary.ContainsKey(Category.ExtensionDamage) == true)
                {
                    extensionDamage = (uint)Mathf.Clamp(dictionary[Category.ExtensionDamage], uint.MinValue, uint.MaxValue);
                }
                if (dictionary.ContainsKey(Category.AttackSpeed) == true)
                {
                    attackSpeed = dictionary[Category.AttackSpeed];
                }
            }
        }

        public static Attacker operator + (Attacker a, Attacker b)
        {
            float baseDamage = Mathf.Clamp(a.baseDamage + b.baseDamage, int.MinValue, int.MaxValue);
            float extensionDamage = Mathf.Clamp(a.extensionDamage + b.extensionDamage, uint.MinValue, uint.MaxValue);
            float attackSpeed = a.attackSpeed + b.attackSpeed;
            return new Attacker((int)baseDamage, (uint)extensionDamage, attackSpeed);
        }
    }

    //����ϴ� ��ü�� �ɷ�ġ
    [Serializable]
    public struct Defencer
    {
        //�⺻ ü��
        public int baseLife;
        //���̰ų� ������ ü��
        public int gatherLife;
        //���ݷ� �氨��
        public int reduceDamage;

        public Defencer(int baseLife, int gatherLife, int reduceDamage)
        {
            this.baseLife = baseLife;
            this.gatherLife = gatherLife;
            this.reduceDamage = reduceDamage;
        }

        public Defencer(Dictionary<Category, float> dictionary)
        {
            baseLife = 0;
            gatherLife = 0;
            reduceDamage = 0;
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(Category.BaseLife) == true)
                {
                    baseLife = (int)Mathf.Clamp(dictionary[Category.BaseLife], int.MinValue, int.MaxValue);
                }
                if (dictionary.ContainsKey(Category.GatherLife) == true)
                {
                    gatherLife = (int)Mathf.Clamp(dictionary[Category.GatherLife], int.MinValue, int.MaxValue);
                }
                if (dictionary.ContainsKey(Category.ReduceDamage) == true)
                {
                    reduceDamage = (int)Mathf.Clamp(dictionary[Category.ReduceDamage], int.MinValue, int.MaxValue);
                }
            }
        }

        public static Defencer operator +(Defencer a, Defencer b)
        {
            float baseLife = Mathf.Clamp(a.baseLife + b.baseLife, int.MinValue, int.MaxValue);
            float gatherLife = Mathf.Clamp(a.gatherLife + b.gatherLife, int.MinValue, int.MaxValue);
            float reduceDamage = Mathf.Clamp(a.reduceDamage + b.reduceDamage, int.MinValue, int.MaxValue);
            return new Defencer((int)baseLife, (int)gatherLife, (int)reduceDamage);
        }
    }

    //�������� ��ü�� �ɷ�ġ
    public struct Enchanter
    {
        //�⺻ ����
        public int baseMana;
        //���̰ų� ������ ����
        public int gatherMana;

        public Enchanter(int baseMana, int gatherMana)
        {
            this.baseMana = baseMana;
            this.gatherMana = gatherMana;
        }

        public Enchanter(Dictionary<Category, float> dictionary)
        {
            baseMana = 0;
            gatherMana = 0;
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(Category.BaseMana) == true)
                {
                    baseMana = (int)Mathf.Clamp(dictionary[Category.BaseMana], int.MinValue, int.MaxValue);
                }
                if (dictionary.ContainsKey(Category.GatherMana) == true)
                {
                    gatherMana = (int)Mathf.Clamp(dictionary[Category.GatherMana], int.MinValue, int.MaxValue);
                }
            }
        }

        public static Enchanter operator +(Enchanter a, Enchanter b)
        {
            float baseMana = Mathf.Clamp(a.baseMana + b.baseMana, int.MinValue, int.MaxValue);
            float gatherMana = Mathf.Clamp(a.gatherMana + b.gatherMana, int.MinValue, int.MaxValue);
            return new Enchanter((int)baseMana, (int)gatherMana);
        }
    }

    //�÷��̾� ��ü�� �ɷ�ġ
    [Serializable]
    public struct Player
    {
        public Attacker attacker;
        public Defencer defencer;
        public Enchanter enchanter;
        public float moveSpeed;
        public float jumpPower;

        public Player(Attacker attacker, Defencer defencer, Enchanter enchanter, float moveSpeed, float jumpPower)
        {
            this.attacker = attacker;
            this.defencer = defencer;
            this.enchanter = enchanter;
            this.moveSpeed = moveSpeed;
            this.jumpPower = jumpPower;
        }

        public static Player operator +(Player a, Player b)
        {
            Attacker attacker = a.attacker + b.attacker;
            Defencer defencer = a.defencer + b.defencer;
            Enchanter enchanter = a.enchanter + b.enchanter;
            float moveSpeed = a.moveSpeed + b.moveSpeed;
            float jumpPower = a.jumpPower + b.jumpPower;
            return new Player(attacker, defencer, enchanter, moveSpeed, jumpPower);
        }
    }
}